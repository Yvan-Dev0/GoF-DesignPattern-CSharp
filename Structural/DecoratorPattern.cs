using GoFPatternGuide.Core;

namespace GoFPatternGuide.Structural;

/// <summary>
/// DECORATOR
///
/// Intent: add responsibilities to an object at runtime by wrapping it in an
/// object that has the same interface.
///
/// The demo starts with a plain coffee and wraps it in milk, caramel, and an
/// extra espresso shot. Each wrapper adds to the description and the price.
/// </summary>
public sealed class DecoratorPattern : IPatternDemo
{
    public string Name => "Decorator";

    public string Category => PatternCategories.Structural;

    public string Summary =>
        "Adds behavior to one object at runtime by wrapping it in layers with the same interface.";

    public string Explanation => """
        WHAT IT IS
        Decorator is a structural pattern that adds responsibilities to an object
        without changing its class. A decorator implements the same interface as
        the object it wraps, holds a reference to that object, and adds behavior
        before or after delegating to it.

        Because the decorator has the same interface, it can be used anywhere the
        original object was used. Decorators can also wrap other decorators,
        creating a flexible chain of features.

        WHY IT EXISTS
        Inheritance adds behavior at compile time and applies to every instance of
        a subclass. That works for stable categories, but it becomes clumsy when
        features are optional and combinable.

        Coffee can have milk, sugar, caramel, an extra shot, or none of those. If
        each combination becomes a subclass, the class count grows exponentially.
        Decorator lets us attach only the features an individual order needs.

        THE PROBLEM IT SOLVES
        Imagine a base Coffee class. A MilkCoffee subclass adds milk. A
        MilkAndCaramelCoffee subclass adds two features. A MilkCaramelExtraShotCoffee
        adds three. Every new add-on multiplies the number of classes, and the
        client must choose the correct combination in advance.

        With Decorator, the client creates a SimpleCoffee and wraps it as needed.
        The core coffee class stays small, and each add-on is a separate, reusable
        class.

        WHEN TO USE IT
        - Responsibilities should be added to individual objects, not an entire class.
        - Features are optional and can be combined in different orders.
        - Subclassing would create too many combinations.
        - You want to add behavior without modifying existing code.
        - You need to layer cross-cutting behavior around an object.

        ADVANTAGES
        - Adds behavior at runtime.
        - Avoids a large inheritance explosion.
        - Each decorator has one focused responsibility.
        - Decorators can be combined in different ways.
        - Existing classes stay closed for modification but open for extension.
        - The wrapped object can be replaced or tested independently.

        DISADVANTAGES
        - Produces many small objects, which can make debugging harder.
        - Long wrapping chains are difficult to read and trace.
        - Decorators depend on the wrapped interface, so they cannot easily remove
          behavior that is baked into the core object.
        - Order can matter, especially when decorators transform data.
        - Identity checks and concrete type checks may fail because the outer object
          is a wrapper rather than the original type.
        - Initialization code can become awkward because each layer is constructed
          around the previous layer.

        REAL-WORLD EXAMPLES
        - Coffee orders with optional add-ons.
        - .NET streams: a FileStream wrapped by a BufferedStream and then a
          GZipStream.
        - Web middleware that wraps a request pipeline with logging,
          authentication, compression, and caching.
        - UI controls with optional borders, scrolling, and shadows.
        - Text formatting with bold, italic, and underline layers.
        - Encryption or compression applied around a data source.

        HOW THE DEMO WORKS
        1. ICoffee is the component interface, with Description and Cost.
        2. SimpleCoffee is the concrete component.
        3. CoffeeDecorator is the base decorator. It stores an inner ICoffee and
           forwards the interface members.
        4. MilkDecorator, CaramelDecorator, and ExtraShotDecorator each add a small
           amount of description and cost.
        5. The demo creates one plain coffee and one layered coffee. The orders are
           independent, so the plain coffee is unaffected by the other order.

        DECORATOR VS INHERITANCE
        Inheritance is still useful when a subtype is a permanent, meaningful kind
        of the base type. Decorator is better for optional, removable, or
        stackable features. Ask whether the new behavior is part of the object's
        identity or something attached to it.

        DECORATOR VS PROXY
        Both wrap an object behind the same interface. A proxy usually controls
        access to the object (lazy loading, permissions, logging, remote calls).
        A decorator adds responsibilities. The structural code can look similar,
        so intent and naming matter.

        A GOOD RULE OF THUMB
        If you would describe a feature with the word "with" ("a coffee with milk"),
        Decorator may be a natural fit.
        """;

    public void Run()
    {
        Console.WriteLine("Creating two independent coffee orders...\n");

        // A plain coffee. No wrappers are required.
        ICoffee plain = new SimpleCoffee();
        PrintOrder(plain);

        // Decorators wrap the previous object. Read the constructor calls from
        // the inside out: SimpleCoffee, then milk, then caramel, then a shot.
        ICoffee fancy = new ExtraShotDecorator(
            new CaramelDecorator(
                new MilkDecorator(
                    new SimpleCoffee())));

        PrintOrder(fancy);

        Console.WriteLine("\nEach add-on is a separate class, and the core coffee never changed.");
    }

    private static void PrintOrder(ICoffee coffee)
    {
        Console.WriteLine($"Order: {coffee.Description}");
        Console.WriteLine($"Price: ${coffee.Cost:F2}");
        Console.WriteLine();
    }

    // COMPONENT
    private interface ICoffee
    {
        string Description { get; }

        decimal Cost { get; }
    }

    // CONCRETE COMPONENT
    private sealed class SimpleCoffee : ICoffee
    {
        public string Description => "Simple coffee";

        public decimal Cost => 3.00m;
    }

    // BASE DECORATOR
    // It implements the component interface and keeps a reference to another
    // component. Subclasses override the members to add their feature.
    private abstract class CoffeeDecorator : ICoffee
    {
        protected readonly ICoffee Inner;

        protected CoffeeDecorator(ICoffee inner)
        {
            Inner = inner;
        }

        public virtual string Description => Inner.Description;

        public virtual decimal Cost => Inner.Cost;
    }

    // CONCRETE DECORATOR 1
    private sealed class MilkDecorator : CoffeeDecorator
    {
        public MilkDecorator(ICoffee inner)
            : base(inner)
        {
        }

        public override string Description => $"{Inner.Description}, milk";

        public override decimal Cost => Inner.Cost + 0.50m;
    }

    // CONCRETE DECORATOR 2
    private sealed class CaramelDecorator : CoffeeDecorator
    {
        public CaramelDecorator(ICoffee inner)
            : base(inner)
        {
        }

        public override string Description => $"{Inner.Description}, caramel";

        public override decimal Cost => Inner.Cost + 0.75m;
    }

    // CONCRETE DECORATOR 3
    private sealed class ExtraShotDecorator : CoffeeDecorator
    {
        public ExtraShotDecorator(ICoffee inner)
            : base(inner)
        {
        }

        public override string Description => $"{Inner.Description}, extra espresso shot";

        public override decimal Cost => Inner.Cost + 1.20m;
    }
}
