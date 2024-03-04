namespace Cloak.Cli;

internal static class Program
{
    internal static void Main(string[] args)
    {
        Console.Title = "Cloak Obfuscator";
        Console.Write("Enter Executable Path (.exe/.dll): ");
        var file = Console.ReadLine()!;
        var cloak = new Core.Cloak(file);
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

        Console.Write("Enter target output path: ");
        var target = Console.ReadLine()!;
        cloak.Protect(target);
        Console.WriteLine("Target successfully protected, press any key to exit");
        Console.ReadKey();
    }
}