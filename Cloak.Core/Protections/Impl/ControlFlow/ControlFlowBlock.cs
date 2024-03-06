using AsmResolver.PE.DotNet.Cil;
using Echo.ControlFlow;

namespace Cloak.Core.Protections.Impl.ControlFlow;

internal class ControlFlowBlock(ControlFlowNode<CilInstruction> node, int sequence, int key)
{
    internal ControlFlowNode<CilInstruction> ControlFlowNode { get; } = node;
    internal int Sequence { get; } = sequence;
    internal int Key { get; } = key;
}