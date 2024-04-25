namespace TestFile;

public static class ControlFlowTesting
{
    public static void TestForLoop()
    {
        var total = 0;
        for (var i = 0; i < 15; i++)
        {
            total += i;
        }
        Console.WriteLine(total == 105 ? "Control Flow: for loop passed" : "Control Flow: for loop failed");
    }

    public static void TestIfStatements()
    {
        var i = new Random().Next(5, 5);
        if (i == 5)
        {
            i += 10;
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (i == 5)
        {
            Console.WriteLine("Control Flow: if statement failed");
        }

        if (i == 15)
        {
            Console.WriteLine("Control Flow: if statement passed");
            return;
        }
        
        Console.WriteLine("Control Flow: if statement failed");
    }

    public static void TestExceptionHandling()
    {
        var i = 0;

        try
        {
            i = 15;

            // ReSharper disable once IntDivisionByZero
            i /= 0;
        }
        catch (ArithmeticException)
        {
            try
            {
                i += 10;
                throw new Exception();
            }
            catch (ArithmeticException)
            {
                Console.WriteLine("Control Flow: exception handling failed");
            }
            catch
            {
                i -= 3;
            }
        }
        
        Console.WriteLine(i == 22 ? "Control Flow: exception handling passed" : "Control Flow: exception handling failed");
    }
}