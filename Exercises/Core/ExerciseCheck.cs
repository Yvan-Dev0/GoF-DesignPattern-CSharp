using System.Globalization;
using GoFPatternGuide.Core;

namespace GoFPatternGuide.Exercises.Core;

/// <summary>
/// A tiny assertion helper for exercises.
///
/// It is not a test framework. It prints one line per check and keeps a tally:
///   PASS - the behavior is correct.
///   TODO - the code still throws NotImplementedException (not started yet).
///   FAIL - the code ran but produced the wrong result or threw another error.
///
/// Every check takes a lambda so an exception in one check never stops the rest.
/// </summary>
public sealed class ExerciseCheck
{
    private readonly bool _quiet;

    /// <param name="quiet">True to keep the tally without printing each line.</param>
    public ExerciseCheck(bool quiet = false)
    {
        _quiet = quiet;
    }

    public int Passed { get; private set; }

    public int Failed { get; private set; }

    public int Todo { get; private set; }

    public bool AllPassed => Passed > 0 && Failed == 0 && Todo == 0;

    /// <summary>Passes when the condition returns true.</summary>
    public void That(string description, Func<bool> condition) =>
        Record(description, () => condition() ? null : "the condition was false");

    /// <summary>Passes when the actual value equals the expected value.</summary>
    public void Equal<T>(string description, T expected, Func<T> actual) =>
        Record(description, () =>
        {
            var value = actual();
            return EqualityComparer<T>.Default.Equals(value, expected)
                ? null
                : $"expected {Format(expected)}, but got {Format(value)}";
        });

    /// <summary>Passes when both sequences contain the same items in the same order.</summary>
    public void SequenceEqual<T>(string description, IEnumerable<T> expected, Func<IEnumerable<T>> actual) =>
        Record(description, () =>
        {
            var expectedList = expected.ToList();
            var actualList = actual().ToList();
            return expectedList.SequenceEqual(actualList)
                ? null
                : $"expected [{string.Join(", ", expectedList.Select(item => Format(item)))}], " +
                  $"but got [{string.Join(", ", actualList.Select(item => Format(item)))}]";
        });

    /// <summary>Passes when the action throws the given exception type.</summary>
    public void Throws<TException>(string description, Action action)
        where TException : Exception =>
        Record(description, () =>
        {
            try
            {
                action();
                return $"expected {typeof(TException).Name}, but nothing was thrown";
            }
            catch (TException exception) when (exception is not NotImplementedException)
            {
                return null;
            }
        });

    /// <summary>Prints the tally and a one-line verdict.</summary>
    public void PrintSummary()
    {
        Console.WriteLine();
        ConsoleUi.Info($"Passed: {Passed}   Failed: {Failed}   Not implemented yet: {Todo}");

        if (AllPassed)
        {
            ConsoleUi.Success("All checks passed. Now answer the self-review questions honestly.");
        }
        else if (Failed == 0 && Passed == 0)
        {
            ConsoleUi.Warning("Nothing is implemented yet. Read the task and the starter code, then begin.");
        }
        else
        {
            ConsoleUi.Warning("Not finished yet. Fix the FAIL and TODO lines above.");
        }
    }

    private void Record(string description, Func<string?> evaluate)
    {
        string? failure;

        try
        {
            failure = evaluate();
        }
        catch (NotImplementedException)
        {
            Todo++;
            Write($"  TODO  {description}", ConsoleColor.Yellow);
            return;
        }
        catch (Exception exception)
        {
            failure = $"threw {exception.GetType().Name}: {exception.Message}";
        }

        if (failure is null)
        {
            Passed++;
            Write($"  PASS  {description}", ConsoleColor.Green);
        }
        else
        {
            Failed++;
            Write($"  FAIL  {description}", ConsoleColor.Red);
            Write($"        {failure}", ConsoleColor.DarkGray);
        }
    }

    private void Write(string message, ConsoleColor color)
    {
        if (!_quiet)
        {
            ConsoleUi.WriteLine(message, color);
        }
    }

    private static string Format(object? value) => value switch
    {
        null => "null",
        string text => $"\"{text}\"",
        DateTime time => time.ToString("ddd yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
        decimal number => number.ToString("0.00", CultureInfo.InvariantCulture),
        _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? "null"
    };
}
