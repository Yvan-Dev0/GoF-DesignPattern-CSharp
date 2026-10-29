namespace GoFPatternGuide.Core;

/// <summary>
/// The small contract every lesson in this guide implements.
///
/// Keeping this interface tiny makes the menu easy to build: it can list every
/// pattern, show its written lesson, and then execute its runnable example
/// without knowing anything about the pattern itself.
/// </summary>
public interface IPatternDemo
{
    /// <summary>The common name of the pattern, for example "Adapter".</summary>
    string Name { get; }

    /// <summary>One of the three Gang of Four categories.</summary>
    string Category { get; }

    /// <summary>A one-sentence description used in menus and summary tables.</summary>
    string Summary { get; }

    /// <summary>
    /// The beginner-friendly lesson. It explains the problem, the trade-offs,
    /// and where the pattern appears in real software before the demo runs.
    /// </summary>
    string Explanation { get; }

    /// <summary>
    /// Runs a small, self-contained example. Demos only write to the console and
    /// never ask for additional input, so they are safe to run from the menu or
    /// from the command line.
    /// </summary>
    void Run();
}
