using GoFPatternGuide.Core;

namespace GoFPatternGuide.Creational;

/// <summary>
/// ABSTRACT FACTORY
///
/// Intent: create families of related objects without making client code depend
/// on their concrete classes.
///
/// In this demo a user-interface renderer receives an IUiFactory. It can draw a
/// complete light theme or a complete dark theme without ever writing an if
/// statement such as "if (darkMode) use DarkButton". Every product in a family
/// comes from the same factory, so mismatched combinations are prevented.
/// </summary>
public sealed class AbstractFactoryPattern : IPatternDemo
{
    public string Name => "Abstract Factory";

    public string Category => PatternCategories.Creational;

    public string Summary =>
        "Creates a whole family of related objects through one interchangeable factory interface.";

    public string Explanation => """
        WHAT IT IS
        Abstract Factory is a creational pattern that provides an interface for
        creating a family of related objects. The client works only with the
        interfaces for those objects. A concrete factory decides which specific
        versions to instantiate.

        A useful phrase to remember is "a factory of factories." Each concrete
        factory produces several products that are designed to work together.

        WHY IT EXISTS
        Programmers often need to support multiple versions of the same system:
        a light and dark user interface, Windows and macOS controls, or a modern
        and legacy database driver. Without a plan, this leads to long conditional
        statements spread throughout the codebase. Every new variant requires
        finding and editing dozens of places.

        Abstract Factory pulls those decisions into one place: a factory object.
        The rest of the program asks for the objects it needs and remains unaware
        of the concrete classes being used.

        THE PROBLEM IT SOLVES
        Imagine a screen that creates a light button by default. A new dark theme
        arrives, so someone adds an if statement around the button creation. Later
        the screen also creates a checkbox, a text box, and a menu. The condition
        is repeated everywhere. Worse, a developer can accidentally combine a
        light button with a dark checkbox, producing an inconsistent interface.

        Abstract Factory solves both issues. The factory creates a matching family,
        and the client does not contain variant-specific branching.

        WHEN TO USE IT
        - Your code must create several related objects that should be used
          together as a family.
        - The system needs to support multiple interchangeable product families.
        - You want client code to depend on interfaces, not concrete classes.
        - A product family may be selected once at startup or by configuration.

        ADVANTAGES
        - Keeps related products consistent.
        - Removes conditional family-selection logic from client code.
        - Makes swapping an entire family a one-line change.
        - Supports the open/closed principle when introducing a new family.
        - Makes dependencies and product boundaries explicit.

        DISADVANTAGES
        - Adds more interfaces and classes than a simple switch statement.
        - Adding a new product type (for example, a slider) means updating every
          factory and every product interface.
        - It can be overkill when there is only one family or only one product.
        - New developers must learn the indirection before they can trace creation.

        REAL-WORLD EXAMPLES
        - Cross-platform UI toolkits that create matching buttons, windows, and
          scrollbars for each operating system.
        - Cloud SDKs that create a related set of storage, queue, and database
          clients for AWS, Azure, or another provider.
        - Game engines with low, medium, and high graphics asset families.
        - Theme systems that provide a consistent set of colors, fonts, and controls.
        - Document exporters that create matching headers, tables, and footers for
          PDF versus HTML output.

        HOW THE DEMO WORKS
        1. IButton and ICheckbox are product interfaces.
        2. IUiFactory is the abstract factory.
        3. LightThemeFactory and DarkThemeFactory each create a consistent family.
        4. RenderScreen knows only the interfaces, so it works with either family.

        A GOOD RULE OF THUMB
        Use Abstract Factory when the word "family" matters. If you only need to
        create one kind of object, Factory Method is usually a better fit.
        """;

    public void Run()
    {
        Console.WriteLine("Building two screens from two families of controls...\n");

        // The client receives a factory and is completely unaware of the
        // concrete control classes behind it.
        RenderScreen("Light theme", new LightThemeFactory());
        Console.WriteLine();
        RenderScreen("Dark theme", new DarkThemeFactory());

        Console.WriteLine("\nNotice that RenderScreen never asks which theme it received.");
    }

    // This is the client code. It depends only on abstractions. We could add a
    // high-contrast theme by creating another factory, without editing this method.
    private static void RenderScreen(string screenName, IUiFactory factory)
    {
        IButton button = factory.CreateButton();
        ICheckbox checkbox = factory.CreateCheckbox();

        Console.WriteLine($"--- {screenName} ---");
        Console.WriteLine(button.Paint());
        Console.WriteLine(checkbox.Render());
    }

    // PRODUCT A
    private interface IButton
    {
        string Paint();
    }

    // PRODUCT B
    private interface ICheckbox
    {
        string Render();
    }

    // ABSTRACT FACTORY
    // Every concrete factory must be able to create both products.
    private interface IUiFactory
    {
        IButton CreateButton();

        ICheckbox CreateCheckbox();
    }

    // CONCRETE PRODUCTS FOR THE LIGHT FAMILY
    private sealed class LightButton : IButton
    {
        public string Paint() => "[ Light button: white background, dark text ]";
    }

    private sealed class LightCheckbox : ICheckbox
    {
        public string Render() => "[x] Light checkbox: white box, dark check mark";
    }

    // CONCRETE PRODUCTS FOR THE DARK FAMILY
    private sealed class DarkButton : IButton
    {
        public string Paint() => "[ Dark button: charcoal background, light text ]";
    }

    private sealed class DarkCheckbox : ICheckbox
    {
        public string Render() => "[x] Dark checkbox: charcoal box, light check mark";
    }

    // CONCRETE FACTORY 1
    private sealed class LightThemeFactory : IUiFactory
    {
        public IButton CreateButton() => new LightButton();

        public ICheckbox CreateCheckbox() => new LightCheckbox();
    }

    // CONCRETE FACTORY 2
    private sealed class DarkThemeFactory : IUiFactory
    {
        public IButton CreateButton() => new DarkButton();

        public ICheckbox CreateCheckbox() => new DarkCheckbox();
    }
}
