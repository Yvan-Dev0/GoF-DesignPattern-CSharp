using GoFPatternGuide.Behavioral;
using GoFPatternGuide.Creational;
using GoFPatternGuide.Structural;

namespace GoFPatternGuide.Core;

/// <summary>
/// A single, explicit list of all 23 Gang of Four lessons.
///
/// The catalog deliberately uses ordinary constructor calls instead of
/// reflection. Reflection would be clever, but an explicit list is easier for a
/// beginner to read, debug, and extend.
/// </summary>
public static class PatternCatalog
{
    public static IReadOnlyList<IPatternDemo> All { get; } =
    [
        // Creational patterns: 1-5
        new AbstractFactoryPattern(),
        new BuilderPattern(),
        new FactoryMethodPattern(),
        new PrototypePattern(),
        new SingletonPattern(),

        // Structural patterns: 6-12
        new AdapterPattern(),
        new BridgePattern(),
        new CompositePattern(),
        new DecoratorPattern(),
        new FacadePattern(),
        new FlyweightPattern(),
        new ProxyPattern(),

        // Behavioral patterns: 13-23
        new ChainOfResponsibilityPattern(),
        new CommandPattern(),
        new InterpreterPattern(),
        new IteratorPattern(),
        new MediatorPattern(),
        new MementoPattern(),
        new ObserverPattern(),
        new StatePattern(),
        new StrategyPattern(),
        new TemplateMethodPattern(),
        new VisitorPattern()
    ];

    /// <summary>Returns every pattern in a category, preserving catalog order.</summary>
    public static IEnumerable<IPatternDemo> InCategory(string category) =>
        All.Where(pattern => pattern.Category == category);

    /// <summary>Returns the stable, menu-friendly number for a pattern (1-23).</summary>
    public static int GetNumber(IPatternDemo pattern)
    {
        for (var index = 0; index < All.Count; index++)
        {
            if (ReferenceEquals(All[index], pattern))
            {
                return index + 1;
            }
        }

        return -1;
    }

    /// <summary>Finds a pattern by its 1-based catalog number.</summary>
    public static IPatternDemo? FindByNumber(int number) =>
        number >= 1 && number <= All.Count ? All[number - 1] : null;

    /// <summary>Finds a pattern by name, ignoring case and surrounding spaces.</summary>
    public static IPatternDemo? FindByName(string name) =>
        All.FirstOrDefault(pattern =>
            string.Equals(pattern.Name, name.Trim(), StringComparison.OrdinalIgnoreCase));
}
