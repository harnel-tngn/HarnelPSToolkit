using CommandLine;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mono.Cecil;
using System.Text;

namespace ProblemSolving.Templates.Merger
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var parseResult = Parser.Default.ParseArguments<MergerOption>(args);
            if (parseResult.Errors.Any())
            {
                foreach (var err in parseResult.Errors)
                    Console.WriteLine(err);

                throw new InvalidOperationException();
            }

            var option = parseResult.Value;
            Directory.SetCurrentDirectory(option.DllDirectory);
            var module = ModuleDefinition.ReadModule(option.DllPath);

            var entrypoint = module.EntryPoint;

            var referencedTypes = new HashSet<TypeDefinition>();

            var methodDefQueue = new Queue<MethodDefinition>();
            var methodDefInqueue = new HashSet<MethodDefinition>();
            methodDefQueue.Enqueue(entrypoint);
            methodDefInqueue.Add(entrypoint);

            void EnqueueTypeDefs(TypeReference typeRef)
            {
                var typeDef = typeRef.Resolve();
                if (typeDef == null)
                {
                    //Console.WriteLine($"Unable to resolve {typeRef.FullName}");
                    return;
                }

                foreach (var methodDef in typeDef.Methods)
                    if (methodDefInqueue.Add(methodDef))
                        methodDefQueue.Enqueue(methodDef);

                // Already handled
                if (!referencedTypes.Add(typeDef))
                    return;

                if (typeDef.BaseType != null)
                    EnqueueTypeDefs(typeRef);

                foreach (var interfaceRef in typeDef.Interfaces)
                    EnqueueTypeDefs(interfaceRef.InterfaceType);

                if (typeRef is GenericInstanceType genTypeRef)
                {
                    var args = genTypeRef.GenericArguments;
                    if (args != null && args.Any())
                    {
                        foreach (var arg in args)
                            EnqueueTypeDefs(arg);
                    }
                }
            }

            while (methodDefQueue.TryDequeue(out var curr))
            {
                if (curr.Body == null)
                {
                    //Console.WriteLine($"{curr.FullName} has no body");
                    continue;
                }

                foreach (var param in curr.Parameters)
                    EnqueueTypeDefs(param.ParameterType);

                EnqueueTypeDefs(curr.ReturnType);

                foreach (var inst in curr.Body.Instructions)
                {
                    if (inst.Operand is MethodReference methodRef)
                    {
                        EnqueueTypeDefs(methodRef.DeclaringType);

                        var resolved = methodRef.Resolve();
                        if (resolved != null && methodDefInqueue.Add(resolved))
                            methodDefQueue.Enqueue(resolved);
                    }
                    if (inst.Operand is TypeReference typeRef)
                    {
                        EnqueueTypeDefs(typeRef);
                    }
                    if (inst.Operand is MemberReference memberRef)
                    {
                        if (memberRef.DeclaringType != null)
                            EnqueueTypeDefs(memberRef.DeclaringType);
                    }
                }
            }

            var mergeTargets = new HashSet<string>();
            mergeTargets.Add(option.EntrypointPath);

            foreach (var type in referencedTypes)
            {
                var attr = type.CustomAttributes.FirstOrDefault(attr => attr.AttributeType.Name == "IncludeIfReferenced");
                if (attr == null)
                    continue;

                var arg0 = attr.ConstructorArguments[0].Value as string;

                if (arg0 != null)
                    mergeTargets.Add(arg0);
            }

            var merged = MergeSources(mergeTargets);
            File.WriteAllText(option.StandalonePath, merged);
        }

        public static string MergeSources(IEnumerable<string> paths)
        {
            var trees = new List<SyntaxTree>();
            foreach (var path in paths)
            {
                var text = File.ReadAllText(path);
                var root = CSharpSyntaxTree.ParseText(text);
                trees.Add(root);
            }

            var usings = new SortedSet<string>();
            var usingAlias = new SortedSet<(string alias, string ns)>();

            foreach (var tree in trees)
            {
                var comp = tree.GetRoot() as CompilationUnitSyntax;
                if (comp == null)
                    throw new InvalidOperationException();

                foreach (var u in comp.Usings)
                {
                    if (u.Alias == null)
                        usings.Add(u.Name.ToFullString().Trim());
                    else
                        usingAlias.Add((u.Alias.Name.ToFullString().Trim(), u.Name.ToFullString().Trim()));
                }
            }

            var namespaceToMemberDeclList = new Dictionary<string, List<MemberDeclarationSyntax>>();

            foreach (var tree in trees)
            {
                var comp = tree.GetRoot() as CompilationUnitSyntax;
                if (comp == null)
                    throw new InvalidOperationException();

                comp = comp.WithUsings([]);

                foreach (var nsDecl in comp.DescendantNodes().OfType<FileScopedNamespaceDeclarationSyntax>())
                {
                    var ns = nsDecl.Name.ToFullString();
                    if (!namespaceToMemberDeclList.ContainsKey(ns))
                        namespaceToMemberDeclList[ns] = new List<MemberDeclarationSyntax>();

                    namespaceToMemberDeclList[ns].AddRange(nsDecl.Members);
                }
            }

            var sb = new StringBuilder();
            foreach (var u in usings)
                sb.AppendLine($"using {u};");
            foreach (var (a, u) in usingAlias)
                sb.AppendLine($"using {a} = {u};");
            foreach (var u in usings)
                sb.AppendLine($"namespace {u} {{}}");
            sb.AppendLine();

            foreach (var (ns, list) in namespaceToMemberDeclList.OrderBy(kvp => kvp.Key))
            {
                sb.AppendLine($"namespace {ns}");
                sb.AppendLine("{");
                var isFirst = true;
                foreach (var decl in list)
                {
                    var noAttr = decl.WithAttributeLists([]);

                    if (!isFirst)
                        sb.AppendLine();

                    isFirst = false;
                    var lines = noAttr.ToFullString().Split('\n')
                        .SkipWhile(line => String.IsNullOrWhiteSpace(line))
                        .Reverse()
                        .SkipWhile(line => String.IsNullOrWhiteSpace(line))
                        .Reverse();

                    foreach (var line in lines)
                        sb.AppendLine($"    {line.TrimEnd()}");
                }

                sb.AppendLine("}");
                sb.AppendLine();
            }

            sb.AppendLine($"// This is source code merged w/ template");
            sb.AppendLine($"// Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss} UTC+9");

            return sb.ToString();
        }
    }
}
