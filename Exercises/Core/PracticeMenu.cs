using GoFPatternGuide.Core;

namespace GoFPatternGuide.Exercises.Core;

/// <summary>
/// The console screens for practice problems: the list, one exercise with its
/// hints and checks, and the reference solution. Program.cs calls into this
/// class from the main menu and from the command-line switches.
/// </summary>
internal static class PracticeMenu
{
    public static void Browse()
    {
        while (true)
        {
            ConsoleUi.Clear();
            ConsoleUi.Header("Practice Problems");
            ConsoleUi.Info("Every problem happens in HelpDesk Pro, a small support-ticket system.");
            ConsoleUi.Muted("The problems do not name their pattern. Deciding which one fits is part of the exercise.");
            PrintList();

            var choice = ConsoleUi.ReadInt(
                "Choose a problem, or 0 to go back: ",
                0,
                ExerciseCatalog.All.Count);

            if (choice <= 0)
            {
                return;
            }

            Show(ExerciseCatalog.All[choice - 1]);
        }
    }

    public static void PrintList()
    {
        Console.WriteLine();
        ConsoleUi.WriteLine($"{"#",3}  Problem", ConsoleColor.DarkCyan);
        ConsoleUi.WriteLine($"{"---",3}  -------", ConsoleColor.DarkGray);

        foreach (var entry in ExerciseCatalog.All)
        {
            ConsoleUi.WriteLine(
                $"{ExerciseCatalog.GetNumber(entry),3}  {entry.Exercise.Title}",
                ConsoleColor.Gray);
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Shows the scenario and task, then lets the learner run the checks, reveal
    /// hints one at a time, read the self-review questions, or open the solution.
    /// </summary>
    public static void Show(ExerciseEntry entry)
    {
        var hintsShown = 0;
        var hints = entry.AllHints;

        ConsoleUi.Clear();
        PrintProblem(entry);

        while (true)
        {
            ConsoleUi.Rule();
            ConsoleUi.Success("1. Check my code");
            ConsoleUi.Success($"2. Show a hint ({hintsShown} of {hints.Count} shown)");
            ConsoleUi.Success("3. Self-review questions");
            ConsoleUi.Success("4. Show the reference solution");
            ConsoleUi.Success("5. Show the problem again");
            ConsoleUi.Success("0. Back to the problem list");

            var choice = ConsoleUi.ReadInt("Choose an option: ", 0, 5);

            switch (choice)
            {
                case -1:
                case 0:
                    return;
                case 1:
                    RunExercise(entry);
                    break;
                case 2:
                    if (hintsShown < hints.Count)
                    {
                        hintsShown++;
                    }

                    ConsoleUi.SubHeader("Hints");
                    for (var index = 0; index < hintsShown; index++)
                    {
                        ConsoleUi.Info($"{index + 1}. {hints[index]}");
                    }

                    break;
                case 3:
                    ConsoleUi.SubHeader("Self-review: checks cannot judge design, so ask yourself");
                    foreach (var question in entry.Exercise.SelfReview)
                    {
                        ConsoleUi.Info($"- {question}");
                    }

                    break;
                case 4:
                    Console.WriteLine();
                    Console.Write("Try the problem first. Show the solution anyway? (y/N): ");
                    if (string.Equals(Console.ReadLine()?.Trim(), "y", StringComparison.OrdinalIgnoreCase))
                    {
                        ShowSolution(entry);
                    }

                    break;
                case 5:
                    ConsoleUi.Clear();
                    PrintProblem(entry);
                    break;
            }
        }
    }

    public static void PrintProblem(ExerciseEntry entry)
    {
        var number = ExerciseCatalog.GetNumber(entry);
        ConsoleUi.Header($"Problem {number} of {ExerciseCatalog.All.Count}: {entry.Exercise.Title}");
        ConsoleUi.SubHeader("The situation");
        Console.WriteLine(entry.Exercise.Scenario);
        ConsoleUi.SubHeader("Your task");
        Console.WriteLine(entry.Exercise.Task);
        ConsoleUi.Muted($"Edit the starter code in {entry.StarterFile}, then choose 'Check my code'.");
    }

    /// <summary>Runs the learner's starter code against the checks.</summary>
    public static bool RunExercise(ExerciseEntry entry)
    {
        ConsoleUi.SubHeader($"Checking {entry.StarterFile}");
        var check = new ExerciseCheck();
        entry.Exercise.Check(check);
        check.PrintSummary();
        return check.AllPassed;
    }

    public static void ShowSolution(ExerciseEntry entry)
    {
        ConsoleUi.SubHeader($"Reference solution: {entry.Solution.Pattern} ({entry.Solution.Category})");
        Console.WriteLine(entry.Solution.WhyThisPattern);
        ConsoleUi.Muted($"Read the code in {entry.SolutionFile}. The same checks, run against it:");
        Console.WriteLine();

        var check = new ExerciseCheck();
        entry.Solution.Check(check);
        check.PrintSummary();
    }

    /// <summary>
    /// Runs every exercise and every solution quietly and prints one row each.
    /// Returns a non-zero exit code if any reference solution fails, which is
    /// useful for checking the guide itself after an edit.
    /// </summary>
    public static int CheckAll()
    {
        Console.WriteLine();
        ConsoleUi.WriteLine($"{"#",3}  {"Problem",-34}  {"Your code",-22}  Solution", ConsoleColor.DarkCyan);
        ConsoleUi.WriteLine($"{"---",3}  {new string('-', 34)}  {new string('-', 22)}  --------", ConsoleColor.DarkGray);

        var solutionFailures = 0;

        foreach (var entry in ExerciseCatalog.All)
        {
            var mine = new ExerciseCheck(quiet: true);
            entry.Exercise.Check(mine);

            var reference = new ExerciseCheck(quiet: true);
            entry.Solution.Check(reference);

            if (!reference.AllPassed)
            {
                solutionFailures++;
            }

            var mineStatus = mine.AllPassed
                ? "DONE"
                : $"{mine.Passed} pass, {mine.Failed} fail, {mine.Todo} todo";
            var referenceStatus = reference.AllPassed
                ? "PASS"
                : $"BROKEN ({reference.Failed} fail, {reference.Todo} todo)";

            ConsoleUi.WriteLine(
                $"{ExerciseCatalog.GetNumber(entry),3}  {entry.Exercise.Title,-34}  {mineStatus,-22}  {referenceStatus}",
                reference.AllPassed ? ConsoleColor.Gray : ConsoleColor.Red);
        }

        Console.WriteLine();
        if (solutionFailures == 0)
        {
            ConsoleUi.Success("Every reference solution passes its checks.");
            return 0;
        }

        ConsoleUi.Error($"{solutionFailures} reference solution(s) fail their checks.");
        return 1;
    }
}
