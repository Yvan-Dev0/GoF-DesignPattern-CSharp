using GoFPatternGuide.Core;

namespace GoFPatternGuide.Creational;

/// <summary>
/// BUILDER
///
/// Intent: construct a complex object step by step so the construction process
/// is readable and the finished object can validate itself.
///
/// The demo builds a pizza with a fluent nested Builder. Instead of a
/// constructor with many positional parameters, each choice has a named method.
/// </summary>
public sealed class BuilderPattern : IPatternDemo
{
    public string Name => "Builder";

    public string Category => PatternCategories.Creational;

    public string Summary =>
        "Builds a complex object step by step with readable, named steps.";

    public string Explanation => """
        WHAT IT IS
        Builder is a creational pattern that separates the construction of a
        complex object from its final representation. Client code calls a series
        of small steps, such as WithSize, WithCrust, and AddTopping, then asks the
        builder for the finished object.

        The pattern is often implemented with a fluent interface: each method
        returns the builder itself, allowing calls to be chained together.

        WHY IT EXISTS
        Some objects have many optional parts. A pizza can have a size, crust,
        sauce, cheese, and any number of toppings. Passing all of those values to
        one constructor creates a painful signature: booleans with unclear
        meanings, long parameter lists, and accidental argument-order mistakes.

        Builder gives every choice a name and lets the object be assembled in
        stages. It also creates a natural place to validate the final result.

        THE PROBLEM IT SOLVES
        Consider this unfriendly constructor:

            new Pizza("Large", true, false, true, 3, "thin", null, true)

        A reader cannot tell what the booleans mean. Optional values still need to
        be supplied. Adding another option changes the constructor and can break
        callers. If the object is created in an invalid state, the mistake may not
        be discovered until much later.

        Builder replaces positional data with an intention-revealing script and
        checks the assembled object before returning it.

        WHEN TO USE IT
        - An object has many optional configuration values.
        - Construction needs several ordered or conditional steps.
        - You want to produce different representations from the same process.
        - You want validation at the end of construction.
        - A constructor would otherwise have a long list of parameters.

        ADVANTAGES
        - Construction code reads almost like a description of the result.
        - Optional parts can be omitted without confusing null arguments.
        - The final Build method can validate the complete object.
        - The same builder steps can support different output formats.
        - Objects can be made immutable after Build is called.

        DISADVANTAGES
        - Introduces another class and more code.
        - A builder is unnecessary for small objects with two or three values.
        - Fluent chains can become long and hide expensive work.
        - If the builder is reused carelessly, old state can leak into the next build.
        - The pattern does not automatically make an object thread-safe.

        REAL-WORLD EXAMPLES
        - Building an HTTP request with method, URL, headers, timeout, and body.
        - Creating a meal in a restaurant ordering system.
        - Configuring a database connection string or client.
        - Assembling a document with a title, sections, tables, and footer.
        - Constructing a complex query with optional filters.
        - Fluent APIs such as many test-data and configuration libraries.

        HOW THE DEMO WORKS
        1. Pizza is the product. Its constructor is private, so callers must use
           the builder instead of creating an incomplete pizza directly.
        2. Pizza.Builder collects choices with named methods.
        3. Build performs a final validation and returns an immutable Pizza.
        4. The demo creates a vegetarian pizza and a meat lovers pizza.

        DIRECTOR? 
        The original pattern often includes a Director class that knows a recipe
        and calls the builder steps in a fixed order. A Director is optional. For
        a beginner example, a fluent builder is often clearer. In real systems a
        Director can be useful when the same multi-step recipe must be reused with
        different builders.

        A GOOD RULE OF THUMB
        If constructing an object requires a paragraph of explanation, Builder
        can turn that paragraph into named, checkable steps.
        """;

    public void Run()
    {
        Console.WriteLine("Ordering two pizzas with a fluent builder...\n");

        // Each method name explains the choice. There are no mystery booleans
        // and no need to provide values for toppings we do not want.
        Pizza vegetarian = new Pizza.Builder()
            .WithSize("Large")
            .WithCrust("Thin")
            .AddTopping("Mushrooms")
            .AddTopping("Peppers")
            .AddTopping("Olives")
            .Build();

        Pizza meatLovers = new Pizza.Builder()
            .WithSize("Medium")
            .WithCrust("Stuffed")
            .AddTopping("Pepperoni")
            .AddTopping("Sausage")
            .Build();

        vegetarian.Describe();
        Console.WriteLine();
        meatLovers.Describe();

        Console.WriteLine("\nThe builder also prevents an invalid pizza:");

        try
        {
            _ = new Pizza.Builder()
                .WithSize("Huge") // Not one of the supported sizes.
                .Build();
        }
        catch (InvalidOperationException exception)
        {
            Console.WriteLine($"Rejected as expected: {exception.Message}");
        }
    }

    /// <summary>
    /// The product. It is immutable from the outside: once Build returns, the
    /// size, crust, and topping list cannot be changed.
    /// </summary>
    private sealed class Pizza
    {
        private static readonly string[] AllowedSizes = ["Small", "Medium", "Large"];
        private static readonly string[] AllowedCrusts = ["Thin", "Classic", "Stuffed"];

        private Pizza(string size, string crust, IReadOnlyList<string> toppings)
        {
            Size = size;
            Crust = crust;
            Toppings = toppings;
        }

        public string Size { get; }

        public string Crust { get; }

        public IReadOnlyList<string> Toppings { get; }

        public void Describe()
        {
            Console.WriteLine($"Pizza: {Size}, {Crust} crust");
            Console.WriteLine(
                Toppings.Count == 0
                    ? "Toppings: plain cheese"
                    : $"Toppings: {string.Join(", ", Toppings)}");
        }

        /// <summary>
        /// The builder owns temporary, changeable state. Only this nested class
        /// is allowed to call the Pizza constructor.
        /// </summary>
        public sealed class Builder
        {
            private string _size = "Medium";
            private string _crust = "Classic";
            private readonly List<string> _toppings = [];

            public Builder WithSize(string size)
            {
                _size = size;
                return this;
            }

            public Builder WithCrust(string crust)
            {
                _crust = crust;
                return this;
            }

            public Builder AddTopping(string topping)
            {
                if (!string.IsNullOrWhiteSpace(topping))
                {
                    _toppings.Add(topping);
                }

                return this;
            }

            /// <summary>
            /// Validates and freezes the collected values. Returning a new object
            /// keeps the product independent from later builder changes.
            /// </summary>
            public Pizza Build()
            {
                if (!AllowedSizes.Contains(_size, StringComparer.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        $"'{_size}' is not a supported size.");
                }

                if (!AllowedCrusts.Contains(_crust, StringComparer.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        $"'{_crust}' is not a supported crust.");
                }

                // ToArray creates a snapshot. If the builder later adds another
                // topping, the already-built pizza will not change.
                return new Pizza(_size, _crust, _toppings.ToArray());
            }
        }
    }
}
