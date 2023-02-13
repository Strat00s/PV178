//exponentiation
using System;

class Expo {
    static void Main(string[] args) {
        if (args.Length > 1) {
            Console.WriteLine(Math.Pow(Int32.Parse(args[0]), Int32.Parse(args[1])));
            return;
        }
        string line = Console.ReadLine();
        var splits = line.Split(' ');
        if (splits.Length < 1)
            return;
        if (splits.Length > 1) {
            Console.WriteLine(Math.Pow(Int32.Parse(splits[0]), Int32.Parse(splits[1])));
            return;
        }
        int base_num = Int32.Parse(splits[0]);
        int exp = Int32.Parse(Console.ReadLine());
        Console.WriteLine(Math.Pow(base_num, exp));
    }
}