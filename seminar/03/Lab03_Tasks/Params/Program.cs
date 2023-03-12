namespace Params;

/// <summary>
/// Ukol: Implementace metody s promenym poctem parametru
/// 
/// I. Doplnte argument metody ConcatStrings, tak aby prijimal libovolny pocet promennych typu string (vyuzijte klicove slovo params) 
///
/// II. Navratova hodnota metody ConcatStrings bude spojeni vsech stringu
///  
/// III. Zavolejte tuto metodu s pripravenymi promennymi a, b, c
///
/// Hint: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/params
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        const string how = "How ";
        string are = "are ";
        var you = "you ";
        var doing = "doing ";
        var questionMark = "?";

        var result = ConcatStrings(/* TODO III*/);
        Console.WriteLine(result);
    }

    private static string ConcatStrings()
    {
        // TODO II a II
        throw new NotImplementedException();
    }
}