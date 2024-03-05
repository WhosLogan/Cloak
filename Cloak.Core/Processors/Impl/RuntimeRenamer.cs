namespace Cloak.Core.Processors.Impl;

/// <summary>
/// Class <c>RuntimeRenamer</c> Renames runtime injected types and members even when renamer is disabled
/// </summary>
internal class RuntimeRenamer() : Processor
{
    internal override void PreProcess(Cloak cloak)
    {
        foreach (var (_, type) in cloak.ClonedTypes)
        {
            if (type is { IsModuleType: false, Name: not null })
            {
                type.Namespace = "";
                type.Name = cloak.Generator.GenerateName(type.Name);
            }

            foreach (var method in type.Methods)
            {
                if (method.IsConstructor || method.IsSpecialName || method.Name is null) continue;
                method.Name = cloak.Generator.GenerateName(method.Name);
            }

            foreach (var field in type.Fields)
            {
                if (field.IsSpecialName || field.Name is null) continue;
                field.Name = cloak.Generator.GenerateName(field.Name);
            }

            foreach (var property in type.Properties)
            {
                if (property.IsSpecialName || property.Name is null) continue;
                property.Name = cloak.Generator.GenerateName(property.Name);
            }
        }
    }
}