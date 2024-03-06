using AsmResolver.PE.DotNet.Cil;
using Echo.ControlFlow;

namespace Cloak.Core.Protections.Impl.ControlFlow;

internal class ControlFlowBlock(IEnumerable<CilInstruction> instructions, int sequence, int key)
{
    internal IEnumerable<CilInstruction> Instructions { get; } = instructions;
    internal int Sequence { get; } = sequence;
    internal int Key { get; } = key;
}