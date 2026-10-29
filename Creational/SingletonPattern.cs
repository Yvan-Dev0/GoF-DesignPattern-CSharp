using GoFPatternGuide.Core;

namespace GoFPatternGuide.Creational;

/// <summary>
/// SINGLETON
///
/// Intent: guarantee that a class has only one instance and provide a controlled
/// access point to it.
///
/// This demo uses Lazy&lt;T&gt; for thread-safe, on-demand initialization. It also
/// prints the limitations of global state so learners do not treat Singleton as
/// a default solution for sharing data.
/// </summary>
public sealed class SingletonPattern : IPatternDemo
{
    public string Name => "Singleton";

    public string Category => PatternCategories.Creational;

    public string Summary =>
        "Guarantees one shared instance of a class and provides controlled access to it.";

    public string Explanation => """
        WHAT IT IS
        Singleton is a creational pattern that ensures a class has exactly one
        instance in an application and gives code a well-known way to reach it.

        The class usually has:
        - a private constructor, so other code cannot create more instances;
        - a private static field that holds the one instance;
        - a public static property or method that returns it.

        WHY IT EXISTS
        Some resources are naturally shared. Application-wide settings, a
        lightweight in-process logger, or a hardware device controller may need a
        single coordinating object. Creating several copies could cause
        inconsistent state or conflicting access.

        Singleton centralizes that shared instance and controls when it is created.

        THE PROBLEM IT SOLVES
        Without a guard, a program can accidentally create multiple configuration
        objects. One part changes the theme to dark, while another part still reads
        the default theme from a different object. Bugs then depend on which copy
        was updated.

        A Singleton makes the shared state explicit and prevents accidental
        duplicates.

        WHEN TO USE IT
        - Exactly one instance is genuinely required by the domain or a device.
        - The instance must be reachable from many places without passing it through
          every method.
        - Lazy initialization is useful because the object is expensive.
        - You understand the testing and coupling costs and still need the pattern.

        ADVANTAGES
        - Guarantees a single instance.
        - Controls construction and initialization.
        - Can delay expensive creation until first use.
        - Provides a simple access point for a truly global resource.
        - Can be made thread-safe with Lazy or a static constructor.

        DISADVANTAGES
        - Introduces global mutable state, which makes behavior harder to reason
          about and tests harder to isolate.
        - Hides dependencies: a method can use the Singleton without declaring it.
        - Can make parallel tests interfere because they share the same instance.
        - Violates the single-responsibility principle if the class both does work
          and manages its own lifecycle.
        - Subclassing and flexible configuration become awkward.
        - It is often overused when dependency injection would be clearer.

        REAL-WORLD EXAMPLES
        - A process-wide configuration registry.
        - A single hardware controller for a printer or sensor.
        - A shared cache coordinator in a desktop application.
        - A legacy logging service that predates dependency injection.
        - A game's single input manager or audio manager.

        IMPORTANT MODERN PERSPECTIVE
        In modern .NET applications, dependency injection often gives you the same
        "one shared object" behavior by registering a service with a singleton
        lifetime. The object is still one instance, but it does not need a static
        Instance property. Consumers declare their dependency, which is easier to
        test and replace.

        Use the Singleton pattern when the class itself must enforce uniqueness.
        If the application container can enforce it, prefer constructor injection.

        HOW THE DEMO WORKS
        1. AppSettings has a private constructor.
        2. Lazy<AppSettings> creates the instance only when Instance is first read.
        3. LazyThreadSafetyMode.ExecutionAndPublication makes initialization safe
           when several threads ask at the same time.
        4. The demo gets the instance twice, changes a value through the first
           reference, and shows that both references see the same state.

        THREAD SAFETY
        A naive lazy Singleton can create two instances if two threads enter the
        creation check simultaneously. Lazy<T> handles the locking for us. A static
        readonly field initialized inline is another simple thread-safe option.

        A GOOD RULE OF THUMB
        Use Singleton only when uniqueness is a real requirement. If you merely
        want convenient access, dependency injection is usually the healthier
        choice.
        """;

    public void Run()
    {
        Console.WriteLine("Requesting the application settings twice...\n");

        AppSettings first = AppSettings.Instance;
        AppSettings second = AppSettings.Instance;

        Console.WriteLine($"Same object? {ReferenceEquals(first, second)}");
        Console.WriteLine($"Application: {first.ApplicationName}");
        Console.WriteLine($"Theme before change: {second.Theme}");

        // Both variables point to the same object, so this change is visible
        // through either reference. That is the defining behavior of Singleton.
        first.Theme = "Dark";

        Console.WriteLine($"Theme after change through first reference: {second.Theme}");
        Console.WriteLine($"Settings instance count: {AppSettings.InstanceCount}");

        Console.WriteLine("\nRemember: one global object is convenient, but it is still global state.");
    }

    /// <summary>
    /// A small application settings object used to demonstrate Singleton.
    /// In a larger .NET app, this could instead be registered as a singleton
    /// service in the dependency injection container.
    /// </summary>
    private sealed class AppSettings
    {
        // Lazy<T> creates the instance on first access and publishes it safely
        // across threads.
        private static readonly Lazy<AppSettings> LazyInstance =
            new(
                () => new AppSettings(),
                LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Private constructor: code outside this class cannot use new.
        /// </summary>
        private AppSettings()
        {
            InstanceCount++;
        }

        public static AppSettings Instance => LazyInstance.Value;

        // This is only for the lesson output. In production code, counting
        // instances would usually be unnecessary and could itself become state.
        public static int InstanceCount { get; private set; }

        public string ApplicationName { get; } = "Focus Habits";

        public string Theme { get; set; } = "Light";
    }
}
