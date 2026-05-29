using System;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: dotnet run -- [1|2|4]");
            Console.WriteLine("  1 = Del_1_Opgave_1");
            Console.WriteLine("  2 = Del_1_Opgave_2_Og_3");
            Console.WriteLine("  4 = Del_2_Opgave_4_Og_5");
            return;
        }

        switch (args[0])
        {
            case "1":
                Opgave1.Run();
                break;
            case "2":
                Opgave2og3.Run();
                break;
            case "3":
                Opgave4og5.Run();
                break;
            default:
                Console.WriteLine($"Unknown option: {args[0]}");
                Console.WriteLine("Usage: dotnet run -- [1|2|3]");
                break;
        }
    }
}
