namespace GoFPatternGuide.Exercises.Core;

/// <summary>
/// A practice problem for the learner.
///
/// Every exercise takes place in the same small application, "HelpDesk Pro", a
/// ticketing system for a support team. The starter code works for today's
/// requirements but has a design problem that makes the next requirement
/// painful. The learner refactors the starter code until every check passes.
///
/// An exercise deliberately does not name its pattern. Recognizing which
/// pattern fits is half of the skill; the name is only revealed by the last hint.
/// </summary>
public interface IExercise
{
    /// <summary>A short, pattern-neutral title, for example "SLA deadlines".</summary>
    string Title { get; }

    /// <summary>The story: what the code does today and what the business wants next.</summary>
    string Scenario { get; }

    /// <summary>What the learner must change, plus the acceptance criteria.</summary>
    string Task { get; }

    /// <summary>Hints from gentle to specific. The pattern name is added by the catalog.</summary>
    IReadOnlyList<string> Hints { get; }

    /// <summary>
    /// Questions about the design that automated checks cannot answer, for
    /// example "Could a new rule be added without editing an existing class?"
    /// </summary>
    IReadOnlyList<string> SelfReview { get; }

    /// <summary>Runs the acceptance checks against the learner's starter code.</summary>
    void Check(ExerciseCheck check);
}
