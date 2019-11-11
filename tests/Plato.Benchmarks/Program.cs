﻿using System.Reflection;
using BenchmarkDotNet.Running;

namespace Plato.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new BenchmarkSwitcher(typeof(Program).GetTypeInfo().Assembly).Run(args);
        }

    }
}
