namespace Overloading_02;

public class PrettyPrinter
{
    private readonly ConsoleColor defaultBackgroundColor = Console.BackgroundColor;
    private readonly ConsoleColor defaultForegroundColor = Console.ForegroundColor;

    public void Print(string text)
    {
        Console.WriteLine(text);
        SetBackDefaults();
    }

    public void Print(string text, ConsoleColor foregroundColor)
    {
        Console.ForegroundColor = foregroundColor;
        Console.WriteLine(text);
        SetBackDefaults();
    }

    public void Print(string text, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
    {
        Console.ForegroundColor = foregroundColor;
        Console.BackgroundColor = backgroundColor;
        Console.WriteLine(text);
        SetBackDefaults();
    }

    public void Print(string text, string decoratingText)
    {
        Console.Write(decoratingText);
        Console.Write(text);
        Console.WriteLine(decoratingText);
        SetBackDefaults();
    }

    private void SetBackDefaults()
    {
        Console.BackgroundColor = defaultBackgroundColor;
        Console.ForegroundColor = defaultForegroundColor;
    }
}