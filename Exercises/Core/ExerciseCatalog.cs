using GoFPatternGuide.Core;

namespace GoFPatternGuide.Exercises.Core;

/// <summary>An exercise paired with its reference solution.</summary>
public sealed record ExerciseEntry(IExercise Exercise, IExerciseSolution Solution)
{
    /// <summary>
    /// The exercise's own hints followed by two reveal hints: first the pattern
    /// family, then the pattern name and the lesson to re-read.
    /// </summary>
    public IReadOnlyList<string> AllHints
    {
        get
        {
            var lesson = PatternCatalog.FindByName(Solution.Pattern);
            var lessonNote = lesson is null
                ? string.Empty
                : $" Re-read lesson {PatternCatalog.GetNumber(lesson)} if you need a refresher.";

            return
            [
                .. Exercise.Hints,
                $"This is a {Solution.Category} pattern problem.",
                $"The pattern is {Solution.Pattern}.{lessonNote}"
            ];
        }
    }

    /// <summary>Where the learner edits the starter code, relative to the project root.</summary>
    public string StarterFile => $"Exercises/Problems/{Exercise.GetType().Name}.cs";

    /// <summary>Where the reference solution lives, relative to the project root.</summary>
    public string SolutionFile => $"Solutions/{Solution.Category}/{Solution.GetType().Name}.cs";
}

/// <summary>
/// One explicit list of every practice problem.
///
/// The order follows the README's learning roadmap (common patterns first), not
/// the lesson order, so an exercise's number does not give away its pattern.
/// </summary>
public static class ExerciseCatalog
{
    public static IReadOnlyList<ExerciseEntry> All { get; } =
    [
        new(new Problems.SlaDeadlines.SlaDeadlinesExercise(), new Solutions.Strategy.StrategySolution())
    ];

    /// <summary>Finds an exercise by its 1-based number.</summary>
    public static ExerciseEntry? FindByNumber(int number) =>
        number >= 1 && number <= All.Count ? All[number - 1] : null;

    /// <summary>Returns the stable 1-based number of an exercise.</summary>
    public static int GetNumber(ExerciseEntry entry)
    {
        for (var index = 0; index < All.Count; index++)
        {
            if (ReferenceEquals(All[index], entry))
            {
                return index + 1;
            }
        }

        return -1;
    }
}
