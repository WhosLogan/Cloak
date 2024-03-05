using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using AsmResolver.DotNet.Signatures;
using AsmResolver.PE.DotNet.Cil;
using AsmResolver.PE.DotNet.Metadata.Tables.Rows;

namespace Cloak.Core.Protections.Impl;

public class IntEncryption() : Protection("Int Encryption", "Encrypts integer constants")
{
    internal override void Execute(Cloak cloak)
    {
        // Iterate through all classes
        foreach (var type in cloak.Module.GetAllTypes().Where(t => t.IsClass))
        {
            // Create a dictionary of the original ints and keys
            var typeInts = new Dictionary<int, int>();
            
            // Create int decryption method
            var decryptMethod = new MethodDefinition(cloak.Generator.GenerateName(),
                MethodAttributes.Private | MethodAttributes.Static,
                new MethodSignature(CallingConventionAttributes.Default, cloak.Module.CorLibTypeFactory.Int32,
                    new[] { cloak.Module.CorLibTypeFactory.Int32 }));
            
            foreach (var method in type.Methods.Where(m => m.CilMethodBody is not null))
            {
                // Expand all macro instructions
                method.CilMethodBody!.Instructions.ExpandMacros();
                for (var i = 0; i < method.CilMethodBody.Instructions.Count; i++)
                {
                    // Only target ldc_i4 (load 4 byte constant)
                    if (method.CilMethodBody.Instructions[i].OpCode != CilOpCodes.Ldc_I4)
                        continue;
                    
                    // Get the original integer from the instruction operand
                    var originalInt = (int)method.CilMethodBody.Instructions[i].Operand!;
                    
                    // Get a random value for the integer
                    if (!typeInts.TryGetValue(originalInt, out var randomKey))
                    {
                        randomKey = cloak.Generator.GenerateInt();
                        typeInts.Add(originalInt, randomKey);
                    }

                    // Set the operand to the random key and insert a call to the decryptor
                    method.CilMethodBody.Instructions[i].Operand = randomKey;
                    method.CilMethodBody.Instructions.Insert(i + 1, CilOpCodes.Call, decryptMethod);
                }
                
                // Optimize all instructions
                method.CilMethodBody.Instructions.OptimizeMacros();
            }
            
            if (typeInts.Count == 0)
                continue;
            
            CreateIntMethodBody(typeInts, decryptMethod, cloak.Generator);
            type.Methods.Add(decryptMethod);
        }
    }

    private static void CreateIntMethodBody(Dictionary<int, int> ints, MethodDefinition method, Generator generator)
    {
        // Create the int decryption method body
        method.CilMethodBody = new CilMethodBody(method);

        foreach (var (original, key) in ints)
        {
            // Get the "key" for the int
            method.CilMethodBody.Instructions.Add(CilOpCodes.Ldarg_0);
            
            // Compare the parameter with the key
            method.CilMethodBody.Instructions.Add(CilOpCodes.Ldc_I4, key);
            method.CilMethodBody.Instructions.Add(CilOpCodes.Ceq);
            
            // Branch if it's false to the next set of instructions
            var label = new CilInstructionLabel();
            method.CilMethodBody.Instructions.Add(CilOpCodes.Brfalse, label);
            
            // Return the original int xor'd with another randomly generated key
            var randomValue = generator.GenerateInt();
            method.CilMethodBody.Instructions.Add(CilOpCodes.Ldc_I4, randomValue);
            method.CilMethodBody.Instructions.Add(CilOpCodes.Ldc_I4, original ^ randomValue);
            method.CilMethodBody.Instructions.Add(CilOpCodes.Xor);
            method.CilMethodBody.Instructions.Add(CilOpCodes.Ret);
            
            // Nop instruction for the earlier branch
            label.Instruction = method.CilMethodBody.Instructions.Add(CilOpCodes.Nop);
        }
        
        // Create a blank return for if nothing above matches
        method.CilMethodBody.Instructions.Add(CilOpCodes.Ldc_I4_0);
        method.CilMethodBody.Instructions.Add(CilOpCodes.Ret);
        
        // Optimize all instruction macros
        method.CilMethodBody.Instructions.OptimizeMacros();
    }
}