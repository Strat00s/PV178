using System.Diagnostics;
using System.Text;

namespace Params_04;

/// <summary>
/// Ukol: Implementace metody s promenym poctem parametru
/// 
/// I. Doplnte argument metody ConcatStrings, tak aby prijimal libovolny pocet promennych typu string (vyuzijte klicove slovo params) 
///
/// II. Navratova hodnota metody ConcatStrings bude spojeni vsech stringu
///  
/// III. Zavolejte tuto metodu s pripravenymi promennymi
///
/// Hint: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/params
/// </summary>
public static class Solution
{
    public static void Task()
    {
        const string how = "How ";
        string are = "are ";
        var you = "you ";
        var doing = "doing ";
        var questionMark = "?";

        var result1 = ConcatStrings(how, are, you, doing, questionMark);
        var result2 = ConcatStringsBetter(how, are, you, doing, questionMark);
        var result3 = ConcatStringsBest(how, are, you, doing, questionMark);

        Console.WriteLine(result1);
        Console.WriteLine(result2);
        Console.WriteLine(result3);
    }

    // DO NOT DO IT LIKE THIS - NEVER EVER !
    private static string ConcatStrings(params string[] strings)
    {
        var result = "";

        foreach (var s in strings)
        {
            result += s;
        }

        return result;
    }

    private static string ConcatStringsBetter(params string[] strings)
    {
        var stringBuilder1 = new StringBuilder();

        foreach (var s in strings)
        {
            stringBuilder1.Append(s);
        }

        return stringBuilder1.ToString();
    }

    private static string ConcatStringsBest(params string[] strings)
    {
        return string.Join("", strings);
    }


    public static void PerformanceTest()
    {
        Measure(ConcatStringsBest, "Best");
        Measure(ConcatStringsBetter, "Better");
        Measure(ConcatStrings, "Worst");
    }

    private static void Measure(Func<string[], string> f, string text)
    {
        // 50 K slov
        var words = new List<string>();
        for (var i = 0; i < 50_000; i++)
        {
            words.Add("test");
        }

        var wordsArray = words.ToArray();

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        f(wordsArray);
        stopwatch.Stop();

        Console.WriteLine($"{text}: {stopwatch.ElapsedMilliseconds} ms");
    }
}