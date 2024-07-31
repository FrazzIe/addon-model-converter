namespace addon_model_converter;

public static class ConsoleHelper
{
    public static void Write(this ConsoleColor color, string text)
    {
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ResetColor();
    }

    public static void WriteHighlighted(this ConsoleColor color, string text, HashSet<char> highlightChars)
    {
        foreach (var c in text)
        {
            if (highlightChars.Contains(c))
                Console.ForegroundColor = color;
            else
                Console.ResetColor();
            Console.Write(c);
        }

        Console.ResetColor();
    }
}