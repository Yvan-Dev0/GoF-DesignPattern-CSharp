namespace GoFPatternGuide.Core;

/// <summary>
/// Names and short descriptions of the three Gang of Four pattern families.
/// These constants prevent spelling mistakes in menus, catalog filters, and
/// pattern classes.
/// </summary>
public static class PatternCategories
{
    public const string Creational = "Creational";
    public const string Structural = "Structural";
    public const string Behavioral = "Behavioral";

    /// <summary>The categories in the recommended learning order.</summary>
    public static IReadOnlyList<string> All { get; } =
    [
        Creational,
        Structural,
        Behavioral
    ];

    /// <summary>Returns a beginner-friendly description of a category.</summary>
    public static string GetDescription(string category) => category switch
    {
        Creational =>
            "How objects are created. These patterns hide construction details and make creation flexible.",
        Structural =>
            "How objects are connected and composed. These patterns help classes work together without tight coupling.",
        Behavioral =>
            "How objects communicate and share responsibility. These patterns organize algorithms, events, and workflows.",
        _ => "An unknown design pattern category."
    };
}
