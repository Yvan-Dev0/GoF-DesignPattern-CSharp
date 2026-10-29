namespace GoFPatternGuide.Exercises.Core;

/// <summary>
/// The reference solution for one exercise.
///
/// A solution runs the exact same checks as its exercise, so a learner can
/// compare their result with a known-good design. It is one good answer, not
/// the only one.
/// </summary>
public interface IExerciseSolution
{
    /// <summary>The pattern this solution applies, for example "Strategy".</summary>
    string Pattern { get; }

    /// <summary>One of the three Gang of Four categories.</summary>
    string Category { get; }

    /// <summary>Why this pattern fits the problem, and what it costs.</summary>
    string WhyThisPattern { get; }

    /// <summary>Runs the exercise's acceptance checks against the reference design.</summary>
    void Check(ExerciseCheck check);
}
