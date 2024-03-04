using System.Text;
using AsmResolver;
using AsmResolver.DotNet;
using AsmResolver.PE.DotNet.Cil;
using AsmResolver.PE.DotNet.Metadata.Tables.Rows;

namespace Cloak.Core.Protections.Impl;

public class StringEncryption() : Protection("String Encryption", "Encrypts all string constants")
{
    internal override List<string> RequiredTypes { get; } =
    [
        "StringDecryptor"
    ];

    internal override void Execute(Cloak cloak)
    {
        // Get injected methods and types
        var decryptor = cloak.ClonedMembers["Cloak.Runtime.StringDecryptor.DecryptString"] as IMethodDescriptor;
        var decryptorType = cloak.ClonedTypes["Cloak.Runtime.StringDecryptor"];
        
        // Blank dictionary to store encrypted strings
        var encryptedStrings = new Dictionary<int, byte[]>();
        
        // Iterate through all types except the string decryptor type
        foreach (var type in cloak.Module.GetAllTypes().Where(t => t != decryptorType))
        {
            // Iterate all methods that have a cil method body
            foreach (var method in type.Methods.Where(m => m.CilMethodBody is not null))
            {
                // Iterate through all of the instructions
                for (var i = 0; i < method.CilMethodBody!.Instructions.Count; i++)
                {
                    // Match all strings within the body
                    if (method.CilMethodBody.Instructions[i].OpCode != CilOpCodes.Ldstr) continue;

                    // Generate the string decryption key
                    var key = cloak.Generator.GenerateInt();

                    // Capture the string from the instruction
                    var str = (string?)method.CilMethodBody.Instructions[i].Operand;
                    if (str is null) continue;
                    
                    // Replace the string with the string decryption key and add a call to the decryptor
                    method.CilMethodBody.Instructions[i].ReplaceWith(CilOpCodes.Ldc_I4, key);
                    method.CilMethodBody.Instructions.Insert(i + 1, CilOpCodes.Call, decryptor!);
                    
                    // Encrypt the string and append it to the dictionary
                    encryptedStrings.Add(key, EncryptString(str, key, method.MetadataToken.ToInt32()));
                }
                
                // Optimize macros and instructions
                method.CilMethodBody.Instructions.OptimizeMacros();
            }
        }
        
        // Serialize the encrypted strings and add them to a resource
        var encryptedData = SerializeEncryptedData(encryptedStrings);
        
        // Add the encrypted data to a resource
        cloak.Module.Resources.Add(new ManifestResource("CloakLovesYou.Strings", 
            ManifestResourceAttributes.Public, new DataSegment(encryptedData)));
    }

    private static byte[] EncryptString(string str, int key, int mdToken)
    {
        // Get the bytes of the string
        var decoded = Encoding.Default.GetBytes(str);
        
        for (var i = 0; i < decoded.Length; i++)
        {
            // Xor the string with the key and metadata token of the method
            decoded[i] = (byte)(BitConverter.GetBytes(key ^ mdToken)[i % sizeof(int)] ^ decoded[i]);
        }

        // Return the encrypted string
        return decoded;
    }

    private static byte[] SerializeEncryptedData(Dictionary<int, byte[]> encryptedStrings)
    {
        // Serialized data
        var serializedBuffer = new List<byte>();
        
        // Total number of encrypted strings
        var stringCount = BitConverter.GetBytes(encryptedStrings.Count);
        serializedBuffer.AddRange(stringCount);

        foreach (var (key, data) in encryptedStrings)
        {
            // Add the key, size, and encrypted data to the serialized buffer
            serializedBuffer.AddRange(BitConverter.GetBytes(key));
            serializedBuffer.AddRange(BitConverter.GetBytes(data.Length));
            serializedBuffer.AddRange(data);
        }

        // Return the serialized encrypted data
        return serializedBuffer.ToArray();
    }
}