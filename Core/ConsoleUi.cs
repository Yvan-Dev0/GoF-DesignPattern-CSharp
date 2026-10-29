namespace GoFPatternGuide.Core;

/// <summary>
/// Small console helpers shared by the menu and the lessons.
/// The goal is consistency: headings, colors, and prompts look the same
/// everywhere, while the design pattern examples stay focused on the pattern.
/// </summary>
internal static class ConsoleUi
{
    private const int Width = 74;

    public static void Clear()
    {
        // Console.Clear throws when output is redirected (for example, when the
        // app is run by a test script). In that case we simply continue printing.
        if (!Console.IsOutputRedirected)
        {
            Console.Clear();
        }
    }

    public static void Header(string title)
    {
        Console.WriteLine();
        WriteLine(new string('=', Width), ConsoleColor.Cyan);
        WriteLine(Center(title.ToUpperInvariant()), ConsoleColor.Cyan);
        WriteLine(new string('=', Width), ConsoleColor.Cyan);
    }

    public static void SubHeader(string title)
    {
        Console.WriteLine();
        WriteLine(title, ConsoleColor.Yellow);
        WriteLine(new string('-', Math.Min(title.Length, Width)), ConsoleColor.DarkGray);
    }

    public static void Rule()
    {
        WriteLine(new string('-', Width), ConsoleColor.DarkGray);
    }

    public static void Info(string message) => WriteLine(message, ConsoleColor.Gray);

    public static void Success(string message) => WriteLine(message, ConsoleColor.Green);

    public static void Warning(string message) => WriteLine(message, ConsoleColor.Yellow);

    public static void Error(string message) => WriteLine(message, ConsoleColor.Red);

    public static void Muted(string message) => WriteLine(message, ConsoleColor.DarkGray);

    public static void WriteLine(string message, ConsoleColor color)
    {
        var previous = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = previous;
    }

    public static void PressEnter(string prompt = "Press Enter to continue...")
    {
        Console.WriteLine();
        Console.Write(prompt);
        Console.ReadLine();
    }

    /// <summary>
    /// Reads a whole number in the inclusive range. Returns -1 when input ends,
    /// which lets the program exit cleanly when it is being piped input.
    /// </summary>
    public static int ReadInt(string prompt, int minimum, int maximum)
    {
        while (true)
        {
            Console.WriteLine();
            Console.Write(prompt);
            var input = Console.ReadLine();

            if (input is null)
            {
                return -1;
            }

            if (int.TryParse(input.Trim(), out var value) &&
                value >= minimum &&
                value <= maximum)
            {
                return value;
            }

            Error($"Please enter a number from {minimum} to {maximum}.");
        }
    }

    private static string Center(string text)
    {
        if (text.Length >= Width)
        {
            return text;
        }

        var padding = (Width - text.Length) / 2;
        return new string(' ', padding) + text;
    }
}
