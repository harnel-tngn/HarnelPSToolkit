using System;
using System.Runtime.CompilerServices;

namespace HarnelPSToolkit.Library;

[IncludeIfReferenced]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Enum)]
internal sealed class IncludeIfReferenced : Attribute
{
    public string CallerPath { get; }

    public IncludeIfReferenced([CallerFilePath] string callerPath = default!)
    {
        CallerPath = callerPath;
    }
}
