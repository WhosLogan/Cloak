using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using AsmResolver.PE.DotNet.Cil;
using Echo.Platforms.AsmResolver;

namespace Cloak.Core.Protections.Impl.ControlFlow;

internal static class ControlFlowBlockParser
{
    internal static List<ControlFlowBlock> ParseMethod(MethodDefinition method, Generator generator, bool shuffle = false)
    {
        // Construct control flow graphs
        var cfg = method.CilMethodBody!.ConstructStaticFlowGraph();

        // Empty block list
        var blocks = new List<ControlFlowBlock>();

        // Instruction list
        var instructions = new List<CilInstruction>();
        
        // The current stack count
        var stackCount = 0;

        // Iterate through all nodes, adding blocks on all branches
        foreach (var node in cfg.Nodes)
        {
            foreach (var instruction in node.Contents.Instructions)
            {
                stackCount += instruction.GetStackPushCount();
                stackCount -= instruction.GetStackPopCount(method.CilMethodBody);
                instructions.Add(instruction);
            }
            if (!node.Contents.Footer.IsConditionalBranch() && !node.Contents.Footer.IsUnconditionalBranch()) continue;
            if (node.GetParentExceptionHandler() != null) continue;
            if (stackCount != 0) continue;
            blocks.Add(new ControlFlowBlock([..instructions], blocks.Count, generator.GenerateInt()));
            instructions.Clear();
        }

        // Add any leftover instructions to the blocks
        if (instructions.Count != 0)
        {
            blocks.Add(new ControlFlowBlock([..instructions], blocks.Count, generator.GenerateInt()));
        }

        if (!shuffle) return blocks;
        var random = new Random();
        return blocks.OrderBy(_ => random.Next()).ToList();
    }
}