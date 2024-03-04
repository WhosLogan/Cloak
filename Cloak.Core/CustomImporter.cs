using AsmResolver.DotNet;
using AsmResolver.DotNet.Cloning;
using AsmResolver.DotNet.Signatures;

namespace Cloak.Core;

internal class CustomImporter(MemberCloneContext memberCloneContext)
    : CloneContextAwareReferenceImporter(memberCloneContext)
{
    private static readonly SignatureComparer Comparer = new();

    // Credit: https://docs.washi.dev/asmresolver/guides/dotnet/cloning.html#custom-reference-importers
    public override IMethodDefOrRef ImportMethod(IMethodDefOrRef method)
    {
        // Check if the method is from a type defined in the System.Runtime.CompilerServices namespace.
        if (method.DeclaringType is not { Namespace.Value: "System.Runtime.CompilerServices" } type)
            return base.ImportMethod(method);

        // We might already have a type and method defined in the target module (e.g., NullableAttribute::.ctor(int32)).
        // Try find it in the target module.
        var existingMethod = Context.Module
            .TopLevelTypes.FirstOrDefault(t => t.IsTypeOf(type.Namespace, type.Name))?
            .Methods.FirstOrDefault(m => method.Name == m.Name && Comparer.Equals(m.Signature, method.Signature));

        // If we found a matching definition, then return it instead of importing the reference.
        return existingMethod ?? base.ImportMethod(method);
    }
}