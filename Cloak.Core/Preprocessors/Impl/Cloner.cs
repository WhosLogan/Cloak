using AsmResolver.DotNet.Cloning;

namespace Cloak.Core.Preprocessors.Impl;

internal class Cloner : IPreprocessor
{
    public void Execute(Cloak cloak)
    {
        var cloner = new MemberCloner(cloak.Module, ctx => new CustomImporter(ctx));
        foreach (var type in cloak.RuntimeModule.GetAllTypes().Where(t => t.Name is not null && cloak.Protections.Any(p => p.RequiredTypes.Contains(t.Name))))
        {
            cloner.Include(type);
        }

        var result = cloner.Clone();
        foreach (var type in result.ClonedTopLevelTypes)
        {
            cloak.ClonedTypes.Add($"{type.Namespace}.{type.Name}", type);
            cloak.Module.TopLevelTypes.Add(type);
        }

        foreach (var member in result.ClonedMembers)
        {
            if (member.DeclaringType == null) continue;
            cloak.ClonedMembers.Add($"{member.DeclaringType.Namespace}.{member.DeclaringType.Name}.{member.Name}", member);
        }
    }
}