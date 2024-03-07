using System.Reflection;

namespace Cloak.Cli;

internal static class Program
{
    internal static void Main(string[] args)
    {
        // Set the console title
        Console.Title = "Cloak Obfuscator";
        
        // Attempt to grab the file path from args
        var file = args.Length == 0 ? "" : args[0];
        if (args.Length == 0)
        {
            Console.Write("Enter Executable Path (.exe/.dll): ");
            file = Console.ReadLine()!;
        }
        
        // New instance of Cloak
        var cloak = new Core.Cloak();
        
        // Enable protections
        foreach (var protection in cloak.Protections)
        {
            Console.Write($"Would you like to enable {protection.Name} (Y/N): ");
            var answer = Console.ReadLine()!.ToLower().Trim();
            if (answer != "y" && answer != "n")
            {
                Console.WriteLine("Invalid option, press any key to exit");
                Console.ReadKey();
                Environment.Exit(-1);
            }

            protection.Enabled = answer == "y";
        }

        // Try to grab the target destination from the args
        var target = args.Length > 1 ? args[1] : "";

        if (args.Length < 2)
        {
            Console.Write("Enter target output path: ");
            target = Console.ReadLine()!;
        }
        
        // Protect the file
        cloak.Protect(file, target);
        
        // Output a message to signal protection is done
        Console.WriteLine("Target successfully protected, press any key to exit");
        Console.ReadKey();
    }
}