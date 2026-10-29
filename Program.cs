using System.Text;
using GoFPatternGuide.Core;

namespace GoFPatternGuide;

/// <summary>
/// Entry point for the learning guide.
///
/// Running with no arguments opens the interactive course menu. Optional command
/// line switches are included for quick classroom demos and automated checks;
/// they are documented in README.md.
/// </summary>
internal static class Program
{
    private const int PatternCount = 23;

    public static int Main(string[] args)
    {
        // UTF-8 lets the console render the occasional non-ASCII character in
        // the explanations on every modern platform.
        Console.OutputEncoding = Encoding.UTF8;

        if (args.Length > 0)
        {
            return RunCommandLine(args);
        }

        RunMainMenu();
        return 0;
    }

    private static void RunMainMenu()
    {
        while (true)
        {
            ConsoleUi.Clear();
            ConsoleUi.Header("Gang of Four Design Patterns");
            ConsoleUi.Info("A beginner-friendly, self-study course with 23 runnable examples.");
            ConsoleUi.Muted("Read the lesson first, then run the small demo to see the idea in action.");
            Console.WriteLine();
            ConsoleUi.Success("1. Browse patterns by category");
            ConsoleUi.Success("2. View all 23 patterns at a glance");
            ConsoleUi.Success("3. Open a pattern by number");
            ConsoleUi.Success("4. About this guide and how to study");
            ConsoleUi.Success("0. Exit");
            ConsoleUi.Rule();

            var choice = ConsoleUi.ReadInt("Choose an option: ", 0, 4);

            switch (choice)
            {
                case -1:
                case 0:
                    Console.WriteLine();
                    ConsoleUi.Success("Keep learning, and remember: patterns are tools, not goals.");
                    return;
                case 1:
                    BrowseCategories();
                    break;
                case 2:
                    ViewAllPatterns();
                    break;
                case 3:
                    OpenPatternByNumber();
                    break;
                case 4:
                    ShowStudyGuide();
                    break;
            }
        }
    }

    /// <summary>
    /// Shows the three families first, then the patterns inside the selected
    /// family. This mirrors the way the source folders are organized.
    /// </summary>
    private static void BrowseCategories()
    {
        while (true)
        {
            ConsoleUi.Clear();
            ConsoleUi.Header("Pattern Categories");
            ConsoleUi.Info("The Gang of Four divided patterns by the kind of design problem they solve.");
            Console.WriteLine();

            for (var index = 0; index < PatternCategories.All.Count; index++)
            {
                var category = PatternCategories.All[index];
                var count = PatternCatalog.InCategory(category).Count();

                ConsoleUi.WriteLine(
                    $"{index + 1}. {category} ({count} patterns)",
                    ConsoleColor.Cyan);
                ConsoleUi.Muted($"   {PatternCategories.GetDescription(category)}");
                Console.WriteLine();
            }

            ConsoleUi.Success("0. Back to the main menu");
            var choice = ConsoleUi.ReadInt("Choose a category: ", 0, PatternCategories.All.Count);

            if (choice <= 0)
            {
                return;
            }

            BrowseCategory(PatternCategories.All[choice - 1]);
        }
    }

    private static void BrowseCategory(string category)
    {
        var patterns = PatternCatalog.InCategory(category).ToList();

        while (true)
        {
            ConsoleUi.Clear();
            ConsoleUi.Header($"{category} Patterns");
            ConsoleUi.Info(PatternCategories.GetDescription(category));
            Console.WriteLine();

            for (var index = 0; index < patterns.Count; index++)
            {
                var pattern = patterns[index];
                ConsoleUi.WriteLine(
                    $"{index + 1,2}. {pattern.Name}",
                    ConsoleColor.Cyan);
                ConsoleUi.Muted($"    {pattern.Summary}");
                Console.WriteLine();
            }

            ConsoleUi.Success(" 0. Back to categories");
            var choice = ConsoleUi.ReadInt(
                "Choose a pattern to study: ",
                0,
                patterns.Count);

            if (choice <= 0)
            {
                return;
            }

            ShowPattern(patterns[choice - 1]);
        }
    }

    private static void ViewAllPatterns()
    {
        while (true)
        {
            ConsoleUi.Clear();
            ConsoleUi.Header("All 23 Gang of Four Patterns");
            PrintPatternTable();
            ConsoleUi.Muted("Tip: choose 0 to return, or use option 3 on the main menu to open one directly.");

            var choice = ConsoleUi.ReadInt(
                "Enter a pattern number to open it, or 0 to go back: ",
                0,
                PatternCount);

            if (choice <= 0)
            {
                return;
            }

            var pattern = PatternCatalog.FindByNumber(choice);
            if (pattern is not null)
            {
                ShowPattern(pattern);
            }
        }
    }

    private static void OpenPatternByNumber()
    {
        ConsoleUi.Clear();
        ConsoleUi.Header("Open a Pattern by Number");
        PrintPatternTable();

        var choice = ConsoleUi.ReadInt(
            "Pattern number (0 to cancel): ",
            0,
            PatternCount);

        if (choice <= 0)
        {
            return;
        }

        var pattern = PatternCatalog.FindByNumber(choice);
        if (pattern is not null)
        {
            ShowPattern(pattern);
        }
    }

    private static void PrintPatternTable()
    {
        Console.WriteLine();
        ConsoleUi.WriteLine($"{"#",3}  {"Category",-11}  Pattern", ConsoleColor.DarkCyan);
        ConsoleUi.WriteLine($"{"---",3}  {"-----------",-11}  -------", ConsoleColor.DarkGray);

        foreach (var pattern in PatternCatalog.All)
        {
            var number = PatternCatalog.GetNumber(pattern);
            ConsoleUi.WriteLine(
                $"{number,3}  {pattern.Category,-11}  {pattern.Name}",
                ConsoleColor.Gray);
        }

        Console.WriteLine();
    }

    /// <summary>
    /// The heart of the course flow: explanation first, then the executable
    /// example. Nothing runs until the learner presses Enter.
    /// </summary>
    private static void ShowPattern(IPatternDemo pattern)
    {
        ConsoleUi.Clear();
        var number = PatternCatalog.GetNumber(pattern);
        ConsoleUi.Header($"Pattern {number} of {PatternCount}: {pattern.Name}");
        ConsoleUi.WriteLine($"Category: {pattern.Category}", ConsoleColor.Cyan);
        ConsoleUi.Info(pattern.Summary);
        ConsoleUi.Rule();
        Console.WriteLine();
        Console.WriteLine(pattern.Explanation);
        ConsoleUi.Rule();
        Console.WriteLine();
        Console.Write("Press Enter to run the demo (or type B and press Enter to go back): ");

        var answer = Console.ReadLine();
        if (string.Equals(answer?.Trim(), "b", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        RunDemo(pattern);

        Console.WriteLine();
        Console.Write("Press Enter to return to the pattern list...");
        if (Console.ReadLine() is null)
        {
            return;
        }
    }

    private static void RunDemo(IPatternDemo pattern)
    {
        Console.WriteLine();
        ConsoleUi.WriteLine("-------------------- LIVE DEMO --------------------", ConsoleColor.Yellow);

        try
        {
            pattern.Run();
        }
        catch (Exception exception)
        {
            // A teaching app should not crash because one example failed. The
            // error is shown so a learner can still inspect what happened.
            ConsoleUi.Error($"The demo stopped unexpectedly: {exception.Message}");
        }

        ConsoleUi.WriteLine("------------------ END OF DEMO --------------------", ConsoleColor.Yellow);
    }

    private static void ShowStudyGuide()
    {
        ConsoleUi.Clear();
        ConsoleUi.Header("How to Study Design Patterns");

        ConsoleUi.SubHeader("A simple learning loop");
        Console.WriteLine("""
            1. Read the problem before looking at the solution.
            2. Say the pattern's one-sentence purpose out loud in your own words.
            3. Run the demo and follow the console output.
            4. Open the source file and trace the same flow in code.
            5. Change the example: add a new type, rule, or add-on.
            6. Try to recognize the pattern in a library or app you already use.
            """);

        ConsoleUi.SubHeader("Suggested order");
        Console.WriteLine("""
            Start with Factory Method, Builder, Adapter, Strategy, and Observer.
            They appear frequently and are easy to spot in ordinary code.

            Then study Composite, Decorator, Command, and State, which show how
            objects can be composed or can change behavior cleanly.

            Finish with Abstract Factory, Bridge, Flyweight, Interpreter, and
            Visitor. They are powerful, but usually make more sense after the
            smaller patterns are familiar.
            """);

        ConsoleUi.SubHeader("The most important warning");
        Console.WriteLine("""
            Do not force patterns into every program. A pattern is a name for a
            proven solution to a recurring problem, not a badge of quality.
            Prefer the simplest design that stays clear and can grow when needed.
            """);

        ConsoleUi.PressEnter("Press Enter to return to the main menu...");
    }

    private static int RunCommandLine(string[] args)
    {
        var command = args[0].ToLowerInvariant();

        switch (command)
        {
            case "--list":
                PrintPatternTable();
                return 0;

            case "--run":
                return RunOneFromCommandLine(args);

            case "--run-all":
                foreach (var pattern in PatternCatalog.All)
                {
                    ConsoleUi.Header($"{PatternCatalog.GetNumber(pattern)}. {pattern.Name}");
                    RunDemo(pattern);
                }

                return 0;

            case "--help":
            case "-h":
            case "/?":
                PrintCommandLineHelp();
                return 0;

            default:
                ConsoleUi.Error($"Unknown command: {args[0]}");
                PrintCommandLineHelp();
                return 1;
        }
    }

    private static int RunOneFromCommandLine(string[] args)
    {
        if (args.Length < 2)
        {
            ConsoleUi.Error("The --run command needs a pattern number or name.");
            PrintCommandLineHelp();
            return 1;
        }

        var requested = string.Join(' ', args.Skip(1));
        IPatternDemo? pattern;

        if (int.TryParse(requested, out var number))
        {
            pattern = PatternCatalog.FindByNumber(number);
        }
        else
        {
            pattern = PatternCatalog.FindByName(requested);
        }

        if (pattern is null)
        {
            ConsoleUi.Error($"No pattern matched '{requested}'.");
            ConsoleUi.Info("Use --list to see every available number and name.");
            return 1;
        }

        ConsoleUi.Header($"{PatternCatalog.GetNumber(pattern)}. {pattern.Name}");
        ConsoleUi.WriteLine($"Category: {pattern.Category}", ConsoleColor.Cyan);
        Console.WriteLine();
        Console.WriteLine(pattern.Explanation);
        RunDemo(pattern);
        return 0;
    }

    private static void PrintCommandLineHelp()
    {
        Console.WriteLine("""
            GoFPatternGuide - a beginner's tour of the 23 Gang of Four patterns

            Usage:
              dotnet run                    Start the interactive course menu
              dotnet run -- --list          List all patterns and their numbers
              dotnet run -- --run 7         Show lesson 7 and run its demo
              dotnet run -- --run Adapter   Find a pattern by name
              dotnet run -- --run-all       Run every demo without the explanations
              dotnet run -- --help          Show this help
            """);
    }
}
