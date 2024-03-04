using System.Reflection;
using AsmResolver.DotNet;

namespace Cloak.Core;

public sealed class Cloak(string file)
{
    internal ModuleDefinition Module { get; } = ModuleDefinition.FromFile(file);

    internal ModuleDefinition RuntimeModule { get; } =
        ModuleDefinition.FromModule(Assembly.GetCallingAssembly().GetModule("Cloak.Runtime") ??
                                    throw new DllNotFoundException("Runtime module not found"));
    public List<Protection> Protections { get; } = [];

    public void Protect(string outputDestination)
    {
        Protections.ForEach(p => p.Execute(this));
        Module.Write(outputDestination);
    }
}