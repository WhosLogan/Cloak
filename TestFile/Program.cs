namespace TestFile;

internal static class Program
{
    internal static void Main(string[] args)
    {
        // Test a call and a string (for things like string encryption)
        Console.WriteLine("Console write successful");
        
        // Compute two integers and compare them, also test a ternary operation
        // ReSharper disable once ConditionalTernaryEqualBranch
        Console.WriteLine(5 - 4 == int.MaxValue - 0x7fffffff + 1 ? "Integer test passed" : "Integer test failed");
        
        // Test calling a static method
        MyTestClass.InvokeStaticMethod();
        
        // Create an instance of a type (helps test things like call proxying)
        var testClass = new MyTestClass();
        testClass.InvokeInstanceMethod();
        
        // Test invoking an overridden method (helps with rename obfuscation and call proxying)
        testClass.InvokeBaseMethod();
        
        // Run basic control flow tests
        ControlFlowTesting.TestForLoop();
        ControlFlowTesting.TestIfStatements();
        ControlFlowTesting.TestExceptionHandling();
        
        // Keep the app open
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}