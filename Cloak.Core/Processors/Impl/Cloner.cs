using AsmResolver.DotNet.Cloning;

namespace Cloak.Core.Processors.Impl;

internal class Cloner : Processor
{
    internal override void PreProcess(Cloak cloak)
    {
        // Create a cloner with our custom importer
        var cloner = new MemberCloner(cloak.Module, ctx => new CustomImporter(ctx));
        
        // Include every type from the runtime module that has a reference in any protection
        foreach (var type in cloak.RuntimeModule.GetAllTypes().Where(t =>
                     t.Name is not null && cloak.Protections.Any(p => p.Enabled && p.RequiredTypes.Contains(t.Name))))
            cloner.Include(type);

        // Clone the types
        var result = cloner.Clone();
        
        // Add cloned types to the module and the Cloak context
        foreach (var type in result.ClonedTopLevelTypes)
        {
            // Add the type to the context's cloned types
            cloak.ClonedTypes.Add($"{type.Namespace}.{type.Name}", type);
            
            // Add the type to the module
            cloak.Module.TopLevelTypes.Add(type);
        }

        // Add cloned members to the Cloak context
        foreach (var member in result.ClonedMembers)
        {
            if (member.DeclaringType == null) continue;
            cloak.ClonedMembers.Add($"{member.DeclaringType.Namespace}.{member.DeclaringType.Name}.{member.Name}",
                member);
        }
    }
}