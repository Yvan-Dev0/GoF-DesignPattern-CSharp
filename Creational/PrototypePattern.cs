using GoFPatternGuide.Core;

namespace GoFPatternGuide.Creational;

/// <summary>
/// PROTOTYPE
///
/// Intent: create new objects by copying an existing object instead of building
/// every detail from scratch.
///
/// The demo copies a game character and proves that the copy has its own
/// inventory list. This is a deep copy, not just a second reference.
/// </summary>
public sealed class PrototypePattern : IPatternDemo
{
    public string Name => "Prototype";

    public string Category => PatternCategories.Creational;

    public string Summary =>
        "Creates new objects by cloning a prepared example, including a safe copy of its data.";

    public string Explanation => """
        WHAT IT IS
        Prototype is a creational pattern that creates a new object by copying an
        existing object, called the prototype. The original object acts as a
        template. The client asks for a clone and may adjust a few details.

        The important question in this pattern is not "how do I call a
        constructor?" but "how do I duplicate an object that is already in a
        useful state?"

        WHY IT EXISTS
        Some objects are expensive or awkward to create from scratch. Their setup
        may require reading configuration, running several calculations, or
        combining many optional parts. If the program already has a correctly
        configured example, copying it can be simpler than rebuilding it.

        Prototype also lets creation happen without the client knowing the exact
        concrete type. If a variable holds an IShape, calling Clone can return the
        correct circle or square without a type switch.

        THE PROBLEM IT SOLVES
        Imagine a game that loads a detailed enemy template from a configuration
        file. Creating each new enemy repeats file parsing and setup. Or imagine
        an editor that needs to duplicate a complex shape with dozens of styling
        properties. Calling a giant constructor again is noisy and easy to get
        wrong.

        A clone captures the current state and gives the program a fresh object.
        The client changes only what needs to differ.

        WHEN TO USE IT
        - Creating an object from scratch is expensive or complicated.
        - Many objects share a similar starting configuration.
        - You want to copy an object without depending on its concrete class.
        - You need to preserve a snapshot of an object's state.
        - A system loads templates or presets that users can customize.

        ADVANTAGES
        - Avoids repeating expensive initialization.
        - Can produce objects without knowing their concrete classes.
        - Makes presets and templates easy to reuse.
        - Keeps object creation close to the object's own copying rules.
        - Supports deep or shallow copying as appropriate for the domain.

        DISADVANTAGES
        - Deep copying is difficult when an object graph contains cycles or shared
          resources such as files, sockets, or database connections.
        - A shallow copy can surprise callers by sharing mutable lists or objects.
        - Copy constructors and clone methods must be maintained as fields change.
        - The standard ICloneable interface does not specify whether it returns a
          shallow or deep copy, so many teams avoid it in favor of named methods.
        - Cloning a mutable object can create subtle bugs if the copy is not
          independent where it needs to be.

        REAL-WORLD EXAMPLES
        - A document editor duplicating a styled template.
        - A game spawning enemies from an archetype.
        - A graphics program copying a shape with all of its formatting.
        - Configuration presets in an application.
        - Test data builders that start from a known good object and vary a field.
        - Object caching systems that hand out copies to prevent shared mutation.

        HOW THE DEMO WORKS
        1. GameCharacter is the prototype. It has a name and a mutable inventory.
        2. DeepClone creates a new character and copies the inventory into a new
           list. The two characters no longer share that list.
        3. The demo changes the clone's name and inventory.
        4. The original remains unchanged, which proves the copy is independent.

        SHALLOW VS DEEP COPY
        A shallow copy copies value fields and object references. If two objects
        point to the same List, changing one list changes both. A deep copy also
        creates new copies of nested objects. Deep copying is not always correct:
        sometimes sharing is intentional, such as a shared read-only cache. Decide
        based on the domain rather than copying everything blindly.

        MODERN C# NOTE
        Records provide value-based equality and a with expression for convenient
        copies, but a with expression still performs a shallow copy. If a record
        contains a List, both copies normally refer to the same list. For safe
        independent state, use immutable collections or write an explicit clone
        method as this demo does.

        A GOOD RULE OF THUMB
        Use Prototype when "start from this exact setup and change a little" is
        clearer, cheaper, or safer than "call a constructor and rebuild everything."
        """;

    public void Run()
    {
        Console.WriteLine("Creating a character template and cloning it...\n");

        // The original is fully configured before any copies are made.
        var original = new GameCharacter(
            "Forest Ranger",
            ["Longbow", "Rope", "Healing herb"]);

        GameCharacter clone = original.DeepClone();
        clone.Name = "Forest Ranger (clone)";
        clone.Inventory.Add("Torch");

        Console.WriteLine("ORIGINAL");
        original.Describe();

        Console.WriteLine("\nCLONE");
        clone.Describe();

        Console.WriteLine("\nThe clone added a torch. The original inventory did not change.");
        Console.WriteLine($"ReferenceEquals(original, clone) = {ReferenceEquals(original, clone)}");
        Console.WriteLine(
            $"Same inventory object? {ReferenceEquals(original.Inventory, clone.Inventory)}");
    }

    /// <summary>
    /// The prototype. Its clone method defines exactly what "copy" means for
    /// this domain.
    /// </summary>
    private sealed class GameCharacter
    {
        public GameCharacter(string name, IEnumerable<string> inventory)
        {
            Name = name;

            // Copy the incoming sequence so the new character does not share a
            // list owned by the caller.
            Inventory = new List<string>(inventory);
        }

        public string Name { get; set; }

        public List<string> Inventory { get; }

        /// <summary>
        /// Creates an independent copy. We use an explicit name instead of
        /// ICloneable because callers can see that this is a deep copy.
        /// </summary>
        public GameCharacter DeepClone() =>
            new(Name, Inventory); // Constructor creates a fresh List<string>.

        public void Describe()
        {
            Console.WriteLine($"Name: {Name}");
            Console.WriteLine($"Inventory: {string.Join(", ", Inventory)}");
        }
    }
}
