using System.Reflection;
using AsmResolver.DotNet;
using AsmResolver.DotNet.Builder;
using Cloak.Core.Preprocessors;
using Cloak.Core.Preprocessors.Impl;
using Cloak.Core.Protections;
using Cloak.Core.Protections.Impl;

namespace Cloak.Core;

public sealed class Cloak(string file)
{
    private readonly List<IPreprocessor> _preprocessors =
    [
        new Cloner()
    ];

    internal ModuleDefinition Module { get; } = ModuleDefinition.FromFile(file);

    internal ModuleDefinition RuntimeModule { get; } = ModuleDefinition.FromFile("Cloak.Runtime.dll");

    internal Dictionary<string, IMemberDescriptor> ClonedMembers { get; } = new();
    internal Dictionary<string, TypeDefinition> ClonedTypes { get; } = new();

    internal Generator Generator { get; } = new();

    public List<Protection> Protections { get; } = [
        new StringEncryption()
    ];

    public void Protect(string outputDestination)
    {
        // Execute every preprocessor
        _preprocessors.ForEach(p => p.Execute(this));
        
        // Execute every enabled protection
        foreach (var protection in Protections.Where(p => p.Enabled))
        {
            protection.Execute(this);
        }
        
        // Write the module to the target destination
        Module.Write(outputDestination, new ManagedPEImageBuilder(MetadataBuilderFlags.PreserveAll));
    }
}