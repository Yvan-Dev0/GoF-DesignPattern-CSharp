using GoFPatternGuide.Core;

namespace GoFPatternGuide.Behavioral;

/// <summary>
/// STRATEGY
///
/// Intent: define a family of interchangeable algorithms, encapsulate each one,
/// and let the client select one at runtime.
///
/// The demo applies different discount strategies to the same shopping cart
/// total. Checkout never needs to know how a discount is calculated.
/// </summary>
public sealed class StrategyPattern : IPatternDemo
{
    public string Name => "Strategy";

    public string Category => PatternCategories.Behavioral;

    public string Summary =>
        "Makes a family of algorithms interchangeable so clients can choose one at runtime.";

    public string Explanation => """
        WHAT IT IS
        Strategy is a behavioral pattern that defines a family of algorithms,
        puts each one behind a common interface, and makes them interchangeable.
        The client chooses which strategy to use, and the context delegates the
        work to it.

        The strategy contains the changing part of an algorithm. The context owns
        the stable workflow and does not need to know the strategy's details.

        WHY IT EXISTS
        Many features have several valid approaches: calculating a discount,
        choosing a shipping method, sorting a list, compressing a file, or
        selecting a payment provider. If all approaches are packed into one method
        with conditionals, the method becomes difficult to read and every new
        approach requires editing it.

        Strategy gives each approach its own class and a shared contract. The
        client can switch approaches without changing the surrounding workflow.

        THE PROBLEM IT SOLVES
        A checkout method might contain:

            if (customerType == "Regular") ...
            else if (customerType == "Member") ...
            else if (coupon != null) ...

        Each new promotion adds another branch. Some branches need different data,
        and testing one rule means navigating all the others.

        With Strategy, Checkout receives an IDiscountStrategy. The caller supplies
        the applicable rule. Checkout simply applies it and calculates the total.

        WHEN TO USE IT
        - Several algorithms solve the same problem in different ways.
        - The algorithm should be selected at runtime.
        - You want to remove large conditionals from business logic.
        - Different clients need different behavior from the same context.
        - Algorithms should be independently testable.
        - You want to add a new algorithm without editing existing ones.

        ADVANTAGES
        - Eliminates long conditional chains for algorithm selection.
        - Makes each algorithm small and independently testable.
        - Supports runtime switching.
        - Keeps the context focused on its main responsibility.
        - New strategies can be added without changing the context.
        - Encourages composition over inheritance.

        DISADVANTAGES
        - Adds an interface and several classes.
        - The client must know which strategy to choose.
        - For a tiny, stable algorithm, a simple method or delegate is often enough.
        - Too many strategies can make a codebase feel fragmented.
        - The context may need to expose data that strategies require.
        - Strategy cannot easily change the context's own state; that is closer to
          the State pattern.

        REAL-WORLD EXAMPLES
        - Discount, tax, and pricing rules.
        - Payment providers such as credit card, PayPal, or bank transfer.
        - Shipping-cost calculators.
        - Sorting and compression algorithms.
        - Route planners: fastest, shortest, or scenic.
        - Authentication methods.
        - Image-export formats such as PNG, JPEG, or WebP.

        HOW THE DEMO WORKS
        1. IDiscountStrategy is the strategy interface with a Name and Apply method.
        2. NoDiscountStrategy, PercentageDiscountStrategy, and FixedAmountDiscountStrategy
           are concrete algorithms.
        3. Checkout is the context. It holds a strategy and calculates a total by
           delegating the discount calculation.
        4. The same $120 cart is processed with three strategies.
        5. The demo switches strategies at runtime using SetDiscountStrategy.

        STRATEGY VS STATE
        Both delegate to an interface. Strategy is selected from the outside and
        usually represents an algorithm choice. State is selected by the object's
        lifecycle and represents a mode that changes behavior. A state often
        transitions to another state; a strategy usually does not transition itself.

        STRATEGY VS TEMPLATE METHOD
        Template Method uses inheritance to vary steps inside a fixed algorithm.
        Strategy uses composition to replace the whole algorithm. Strategy is more
        flexible at runtime; Template Method is simpler when the skeleton is truly
        shared and inheritance is a natural fit.

        MODERN C# ALTERNATIVE
        For simple cases, a delegate can act as a lightweight strategy:

            Func<decimal, decimal> discount = total => total * 0.90m;

        Use a full Strategy interface when the algorithm needs a name, multiple
        operations, dependencies, or a meaningful domain role.

        A GOOD RULE OF THUMB
        When you find yourself choosing between several algorithms with a switch or
        if/else, give each algorithm a name and make the choice a strategy.
        """;

    public void Run()
    {
        Console.WriteLine("Applying three discount strategies to the same cart...\n");

        const decimal cartTotal = 120m;
        var checkout = new Checkout(new NoDiscountStrategy());

        Console.WriteLine($"Cart total before discount: ${cartTotal:F2}");
        Console.WriteLine($"Strategy: {checkout.DiscountName}");
        Console.WriteLine($"Final total: ${checkout.CalculateTotal(cartTotal):F2}\n");

        // The context is unchanged; only the algorithm object is different.
        checkout.SetDiscountStrategy(new PercentageDiscountStrategy(20m));
        Console.WriteLine($"Strategy: {checkout.DiscountName}");
        Console.WriteLine($"Final total: ${checkout.CalculateTotal(cartTotal):F2}\n");

        checkout.SetDiscountStrategy(new FixedAmountDiscountStrategy(15m));
        Console.WriteLine($"Strategy: {checkout.DiscountName}");
        Console.WriteLine($"Final total: ${checkout.CalculateTotal(cartTotal):F2}");

        Console.WriteLine("\nCheckout never contained an if/else for discount rules.");
    }

    // STRATEGY
    private interface IDiscountStrategy
    {
        string Name { get; }

        decimal Apply(decimal subtotal);
    }

    // CONCRETE STRATEGY 1
    private sealed class NoDiscountStrategy : IDiscountStrategy
    {
        public string Name => "No discount";

        public decimal Apply(decimal subtotal) => subtotal;
    }

    // CONCRETE STRATEGY 2
    private sealed class PercentageDiscountStrategy : IDiscountStrategy
    {
        private readonly decimal _percent;

        public PercentageDiscountStrategy(decimal percent)
        {
            _percent = Math.Clamp(percent, 0m, 100m);
        }

        public string Name => $"{_percent:0.#}% off";

        public decimal Apply(decimal subtotal) => subtotal * (1m - (_percent / 100m));
    }

    // CONCRETE STRATEGY 3
    private sealed class FixedAmountDiscountStrategy : IDiscountStrategy
    {
        private readonly decimal _amount;

        public FixedAmountDiscountStrategy(decimal amount)
        {
            _amount = Math.Max(0m, amount);
        }

        public string Name => $"${_amount:F2} off";

        // A discount must never produce a negative total.
        public decimal Apply(decimal subtotal) => Math.Max(0m, subtotal - _amount);
    }

    // CONTEXT
    private sealed class Checkout
    {
        private IDiscountStrategy _discountStrategy;

        public Checkout(IDiscountStrategy discountStrategy)
        {
            _discountStrategy = discountStrategy;
        }

        public string DiscountName => _discountStrategy.Name;

        public void SetDiscountStrategy(IDiscountStrategy discountStrategy)
        {
            _discountStrategy = discountStrategy;
        }

        public decimal CalculateTotal(decimal subtotal) =>
            _discountStrategy.Apply(subtotal);
    }
}
