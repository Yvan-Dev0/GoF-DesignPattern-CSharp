using GoFPatternGuide.Core;

namespace GoFPatternGuide.Behavioral;

/// <summary>
/// TEMPLATE METHOD
///
/// Intent: define the skeleton of an algorithm in a base class and let subclasses
/// override specific steps without changing the algorithm's structure.
///
/// The demo prepares coffee and green tea. Both follow the same sequence, but
/// each supplies its own brewing and condiment steps.
/// </summary>
public sealed class TemplateMethodPattern : IPatternDemo
{
    public string Name => "Template Method";

    public string Category => PatternCategories.Behavioral;

    public string Summary =>
        "Defines a fixed algorithm skeleton while letting subclasses customize individual steps.";

    public string Explanation => """
        WHAT IT IS
        Template Method is a behavioral pattern that defines the outline of an
        algorithm in a base class. The base class calls a sequence of steps, some
        of which are fixed and some of which are supplied by subclasses.

        The overall sequence does not change. Only the holes in the template are
        filled in by derived classes.

        WHY IT EXISTS
        Many processes share the same structure but differ in a few details.
        Preparing a hot drink always involves boiling water, brewing, pouring, and
        optionally adding condiments. A report export always gathers data,
        formats it, writes it, and closes the output. An import always opens,
        validates, transforms, and saves.

        Copying the whole sequence into every variant duplicates the order and
        makes it easy for one version to drift. Template Method keeps the shared
        skeleton in one place.

        THE PROBLEM IT SOLVES
        Suppose CoffeeMaker and TeaMaker each implement their own Prepare method.
        They both boil water and pour into a cup, but the duplication is invisible
        until someone changes the safety rule for boiling water. Now both classes
        must be updated, and a third drink may copy the old behavior.

        With Template Method, the base Beverage class owns Prepare. It calls
        abstract or virtual steps that subclasses implement. A change to the shared
        sequence automatically applies to every drink.

        WHEN TO USE IT
        - Several algorithms share the same sequence of steps.
        - The order of steps is important and should not be duplicated.
        - Only a few steps vary between implementations.
        - You want to provide optional hooks for subclasses.
        - A framework needs to control a process while allowing customization.

        ADVANTAGES
        - Removes duplicated algorithm structure.
        - Ensures a consistent order of operations.
        - Lets subclasses customize only the relevant parts.
        - Makes the shared process easy to find and change.
        - Hooks provide optional extension points without forcing every subclass to
          implement them.
        - Supports the Hollywood principle: the base class calls the subclass, not
          the other way around.

        DISADVANTAGES
        - Requires inheritance, which is a stronger coupling than composition.
        - Subclasses can become fragile when the base class changes.
        - The flow can be harder to follow because control jumps between classes.
        - Too many hooks can make the template difficult to understand.
        - It is less flexible at runtime than Strategy.
        - Deep inheritance hierarchies can become rigid.

        REAL-WORLD EXAMPLES
        - Preparing coffee or tea.
        - Data import and export pipelines.
        - Test frameworks with setup, execute, and teardown steps.
        - Web request pipelines with fixed stages and customizable hooks.
        - Game turn sequences: start turn, act, resolve, end turn.
        - Document generation with a fixed layout process.

        HOW THE DEMO WORKS
        1. Beverage is the abstract base class. Prepare is the template method and
           is not virtual, so subclasses cannot accidentally change the sequence.
        2. BoilWater and PourInCup are fixed private steps.
        3. Brew and AddCondiments are abstract steps that every drink must provide.
        4. WantsCondiments is a virtual hook. Coffee accepts the default; GreenTea
           overrides it to skip condiments.
        5. The demo prepares both drinks through the same Prepare method.

        HOLLYWOOD PRINCIPLE
        "Don't call us, we'll call you." The base class controls the algorithm and
        calls the subclass's steps at the right time. The subclass does not call
        the overall sequence itself. This inversion of control is the key idea.

        TEMPLATE METHOD VS STRATEGY
        Template Method varies steps through inheritance and fixes the algorithm's
        structure. Strategy replaces the entire algorithm through composition and
        can switch at runtime. Use Template Method when the sequence is truly
        shared; use Strategy when the whole approach may change.

        A GOOD RULE OF THUMB
        If two processes have the same recipe but a few different ingredients,
        put the recipe in a base class and let subclasses supply the ingredients.
        """;

    public void Run()
    {
        Console.WriteLine("Preparing two hot drinks with the same recipe...\n");

        // The caller sees only Beverage. The template method controls the process.
        Beverage coffee = new Coffee();
        Beverage tea = new GreenTea();

        coffee.Prepare();
        tea.Prepare();

        Console.WriteLine("\nBoth drinks used Prepare; neither subclass changed the step order.");
    }

    // ABSTRACT CLASS / TEMPLATE
    private abstract class Beverage
    {
        /// <summary>
        /// The template method. It is intentionally not virtual: the sequence is
        /// the stable part of the design and should not be replaced by subclasses.
        /// </summary>
        public void Prepare()
        {
            Console.WriteLine($"--- Preparing {DrinkName} ---");
            BoilWater();
            Brew();
            PourIntoCup();

            // A hook: subclasses may opt out of this optional step.
            if (WantsCondiments())
            {
                AddCondiments();
            }

            Console.WriteLine($"{DrinkName} is ready.");
        }

        protected abstract string DrinkName { get; }

        // Fixed step shared by every beverage.
        private void BoilWater() => Console.WriteLine("1. Boil water.");

        // Custom step that every beverage must provide.
        protected abstract void Brew();

        // Fixed step shared by every beverage.
        private void PourIntoCup() => Console.WriteLine("3. Pour into a cup.");

        // Hook with a sensible default. Subclasses may override it.
        protected virtual bool WantsCondiments() => true;

        // Custom step that every beverage must provide.
        protected abstract void AddCondiments();
    }

    // CONCRETE IMPLEMENTATION 1
    private sealed class Coffee : Beverage
    {
        protected override string DrinkName => "Coffee";

        protected override void Brew() =>
            Console.WriteLine("2. Brew coffee grounds.");

        protected override void AddCondiments() =>
            Console.WriteLine("4. Add milk and sugar.");
    }

    // CONCRETE IMPLEMENTATION 2
    private sealed class GreenTea : Beverage
    {
        protected override string DrinkName => "Green tea";

        protected override void Brew() =>
            Console.WriteLine("2. Steep green tea leaves.");

        protected override void AddCondiments() =>
            Console.WriteLine("4. Add lemon.");

        // This drink skips the optional condiment step, demonstrating the hook.
        protected override bool WantsCondiments() => false;
    }
}
