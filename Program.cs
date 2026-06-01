using System;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: dotnet run -- [1|2|3|6|7|8]");
            Console.WriteLine("  1 = Del_1_Opgave_1");
            Console.WriteLine("  2 = Del_1_Opgave_2_Og_3");
            Console.WriteLine("  3 = Del_2_Opgave_4_Og_5");
            Console.WriteLine("  6 = Del_2_Opgave_6");
            Console.WriteLine("  7 = Del_2_Opgave_7");
            Console.WriteLine("  8 = Del_2_Opgave_8");
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
            case "6":
                Opgave6.Run();
                break;
            case "7":
                Opgave7.Run();
                break;
            case "8":
                Opgave8.Run();
                break;
            default:
                Console.WriteLine($"Unknown option: {args[0]}");
                Console.WriteLine("Usage: dotnet run -- [1|2|3|6|7|8]");
                break;
        }
    }
}
