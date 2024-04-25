namespace TestFile;

public class MyTestClass : MyBaseClass
{
    public void InvokeInstanceMethod()
        => Console.WriteLine("Instance method invoked successfully");

    public static void InvokeStaticMethod() => Console.WriteLine("Invoked static method successfully");
}