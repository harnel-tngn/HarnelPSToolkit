using System;
using System.IO;
namespace System {}
namespace System.IO {}

namespace HarnelPSToolkit
{
    public class Program
    {
        public static void Main()
        {
            using var sr = new StreamReader(Console.OpenStandardInput(), bufferSize: 65536);
            using var sw = new StreamWriter(Console.OpenStandardOutput(), bufferSize: 65536);
    
            Solve(sr, sw);
        }
    
        public static void Solve(StreamReader sr, StreamWriter sw)
        {
        }
    }
}

// This is source code merged w/ template
// Timestamp: 2026-02-28 21:48:49 UTC+9
