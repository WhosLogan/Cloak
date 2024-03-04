using System.Reflection;
using AsmResolver.DotNet;
using Cloak.Core.Preprocessors;
using Cloak.Core.Preprocessors.Impl;
using Cloak.Core.Protections;

namespace Cloak.Core;

public sealed class Cloak(string file)
{
    internal ModuleDefinition Module { get; } = ModuleDefinition.FromFile(file);

    internal ModuleDefinition RuntimeModule { get; } =
        ModuleDefinition.FromModule(Assembly.GetCallingAssembly().GetModule("Cloak.Runtime") ??
                                    throw new DllNotFoundException("Runtime module not found"));

    internal Dictionary<string, IMemberDescriptor> ClonedMembers { get; } = new();
    internal Dictionary<string, TypeDefinition> ClonedTypes { get; } = new();
    
    public List<Protection> Protections { get; } = [];

    private readonly List<IPreprocessor> _preprocessors = [
        new Cloner()
    ];

    public void Protect(string outputDestination)
    {
        _preprocessors.ForEach(p => p.Execute(this));
        Protections.ForEach(p => p.Execute(this));
        Module.Write(outputDestination);
    }
}