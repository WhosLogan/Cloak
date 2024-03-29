using AsmResolver.DotNet.Code.Cil;
using AsmResolver.PE.DotNet.Cil;

namespace Cloak.Core.Protections.Impl.ControlFlow;

public class ControlFlow() : Protection("Control Flow", "Mangles the control flow of methods")
{
    internal override void Execute(Cloak cloak)
    {
        foreach (var type in cloak.Module.GetAllTypes())
        {
            foreach (var method in type.Methods.Where(m => m.CilMethodBody is not null))
            {
                // Parse control flow blocks from the method
                var blocks = ControlFlowBlockParser.ParseMethod(method, cloak.Generator, true);
                
                // Clear the method body
                method.CilMethodBody!.Instructions.Clear();
                
                // Create and assign a local with the first blocks key
                var controlLocal = new CilLocalVariable(cloak.Module.CorLibTypeFactory.Int32);
                method.CilMethodBody.LocalVariables.Add(controlLocal);
                method.CilMethodBody.Instructions.Add(CilOpCodes.Ldc_I4, blocks.First(b => b.Sequence == 0).Key);
                method.CilMethodBody.Instructions.Add(CilOpCodes.Stloc, controlLocal);
                
                // Create the start of the loop
                var start = method.CilMethodBody.Instructions.Add(CilOpCodes.Nop);
                var startLabel = new CilInstructionLabel(start);
                
                // The return label to go to the return later
                var retLabel = new CilInstructionLabel();
                
                // Random end value
                var endKey = cloak.Generator.GenerateInt();
                
                // Add blocks in
                foreach (var block in blocks)
                {
                    // Check if the control local is equal to the key and branch to end if it's not
                    method.CilMethodBody.Instructions.Add(CilOpCodes.Ldloc, controlLocal);
                    method.CilMethodBody.Instructions.Add(CilOpCodes.Ldc_I4, block.Key);
                    method.CilMethodBody.Instructions.Add(CilOpCodes.Ceq);
                    var endLabel = new CilInstructionLabel();
                    method.CilMethodBody.Instructions.Add(CilOpCodes.Brfalse, endLabel);
                    
                    // Insert the instructions
                    method.CilMethodBody.Instructions.AddRange(block.Instructions);
                    
                    // Set the key to the next block
                    var nextBlock = blocks.FirstOrDefault(b => b.Sequence == block.Sequence + 1);
                    var nextKey = nextBlock?.Key ?? endKey;
                    method.CilMethodBody.Instructions.Add(CilOpCodes.Ldc_I4, nextKey);
                    method.CilMethodBody.Instructions.Add(CilOpCodes.Stloc, controlLocal);
                    
                    // Create empty nop to be able to jump to (endLabel)
                    endLabel.Instruction = method.CilMethodBody.Instructions.Add(CilOpCodes.Nop);
                }
                
                // Branch back to the top
                method.CilMethodBody.Instructions.Add(CilOpCodes.Br, startLabel);
                
                // Create an end and a ret
                retLabel.Instruction = method.CilMethodBody.Instructions.Add(CilOpCodes.Nop);
                method.CilMethodBody.Instructions.Add(CilOpCodes.Ret);
                
                // Optimize the macros
                method.CilMethodBody.Instructions.OptimizeMacros();
            }
        }
    }
}