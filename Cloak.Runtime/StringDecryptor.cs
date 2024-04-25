using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Cloak.Runtime;

public static class StringDecryptor
{
    private static readonly Dictionary<int, byte[]> EncryptedStrings = new();
    
    static StringDecryptor()
    {
        // Attempt to get the string encryption resource
        var resource = Assembly.GetCallingAssembly().GetManifestResourceStream("CloakLovesYou.Strings");
        if (resource == null) throw new Exception("This application is corrupted");
        
        // Get the data from the resource
        var encryptedData = new List<byte>();
        while (true)
        {
            var b = resource.ReadByte();
            if (b == -1) break;
            encryptedData.Add((byte)b);
        }
        
        // Get the total string count and remove it from the buffer
        var strCount = BitConverter.ToInt32(encryptedData.GetRange(0, sizeof(int)).ToArray(), 0);
        encryptedData.RemoveRange(0, sizeof(int));

        // Add all encrypted strings to the dictionary
        for (var i = 0; i < strCount; i++)
        {
            // Get the key and remove it from the encrypted data
            var key = BitConverter.ToInt32(encryptedData.GetRange(0, sizeof(int)).ToArray(), 0);
            encryptedData.RemoveRange(0, sizeof(int));

            // Get the size and remove it from the encrypted data
            var size = BitConverter.ToInt32(encryptedData.GetRange(0, sizeof(int)).ToArray(), 0);
            encryptedData.RemoveRange(0, sizeof(int));
            
            // Get the encrypted string and remove it from the encrypted data
            var str = encryptedData.GetRange(0, size);
            encryptedData.RemoveRange(0, size);
            
            // Add the data to the dictionary
            EncryptedStrings.Add(key, str.ToArray());
        }
    }
    
    public static string DecryptString(int key)
    {
        // Get the encrypted string from the dictionary
        var encStr = EncryptedStrings[key];
        
        // Copy the encrypted string to a new array
        var str = new byte[encStr.Length];
        Array.Copy(encStr, str, encStr.Length);
        
        // Get the metadata token from the stacktrace
        var mdToken = new StackTrace().GetFrame(1)!.GetMethod()!.MetadataToken;
        
        // Decrypt the encrypted string
        for (var i = 0; i < str.Length; i++)
        {
            str[i] = (byte)(BitConverter.GetBytes(key ^ mdToken)[i % sizeof(int)] ^ str[i]);
        }

        // Return the encrypted string
        return Encoding.Unicode.GetString(str);
    }
}