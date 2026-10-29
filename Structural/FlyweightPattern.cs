using GoFPatternGuide.Core;

namespace GoFPatternGuide.Structural;

/// <summary>
/// FLYWEIGHT
///
/// Intent: share the common, immutable part of many objects to reduce memory
/// use, while keeping the unique part outside the shared object.
///
/// The demo draws many trees. Every oak shares one TreeType object containing
/// its name, color, and texture; only the position stays unique per tree.
/// </summary>
public sealed class FlyweightPattern : IPatternDemo
{
    public string Name => "Flyweight";

    public string Category => PatternCategories.Structural;

    public string Summary =>
        "Shares immutable data among many objects and keeps only the unique state per instance.";

    public string Explanation => """
        WHAT IT IS
        Flyweight is a structural pattern that reduces memory usage by sharing
        common data among many similar objects. The shared data is called the
        intrinsic state. Data that differs for each object is called the extrinsic
        state and is passed in from outside or stored by the client.

        A flyweight factory makes sure each shared combination is created only
        once and then returns the existing object.

        WHY IT EXISTS
        Some programs create enormous numbers of objects. A forest might contain
        millions of trees. A text editor might render millions of characters. A
        game might draw thousands of bullets, particles, or map tiles.

        If every object stores its full appearance and configuration, memory use
        becomes enormous. Much of that data is identical: all oak trees have the
        same texture, all bullet types share the same sprite, and most characters
        share the same font.

        Flyweight separates what is common from what is unique, so the common part
        can be shared.

        THE PROBLEM IT SOLVES
        Suppose a forest has 100,000 oak trees. Each Tree object stores a name, a
        color, a large texture, and an x/y position. The first three values are
        identical for every oak. Storing 100,000 copies of the same texture wastes
        memory.

        With Flyweight, one TreeType holds the name, color, and texture. Each Tree
        holds only position plus a reference to the shared TreeType. The unique
        position remains separate, and the shared data exists once.

        WHEN TO USE IT
        - The program creates a very large number of similar objects.
        - Most object state can be made immutable and shared.
        - The remaining state can be supplied externally or stored by the client.
        - Memory is a real bottleneck, not just a theoretical concern.
        - The cost of a lookup or factory is smaller than the memory saved.

        ADVANTAGES
        - Can dramatically reduce memory consumption.
        - Reduces the number of large objects the garbage collector must manage.
        - Centralizes shared data, making updates to shared appearance consistent.
        - Works well with rendering, simulation, and text-layout systems.
        - Makes the distinction between shared and unique state explicit.

        DISADVANTAGES
        - Adds complexity: clients must manage extrinsic state and the factory.
        - Shared objects must be immutable, or one change will affect everyone.
        - The factory and lookups introduce CPU and code overhead.
        - Not useful when objects have little shared data or are few in number.
        - Can make object identity and lifetime harder to understand.
        - Premature use can make code slower if memory is not actually a problem.

        REAL-WORLD EXAMPLES
        - Trees in a forest where species appearance is shared.
        - Characters in a text editor where glyphs and font data are shared.
        - Map tiles that reuse one image per tile type.
        - Bullet or particle types in a game.
        - Icons in a file explorer where all PDFs share one icon object.
        - Cached formatting or style objects in a large document.

        HOW THE DEMO WORKS
        1. TreeType is the flyweight. It stores name, color, and texture, and is
           immutable.
        2. TreeTypeFactory stores one TreeType per unique key. Repeated requests
           return the same object.
        3. Tree stores only x, y, and a reference to a TreeType. Its position is
           extrinsic state because it differs per tree.
        4. The demo plants several trees from three species and prints how many
           shared TreeType objects were created.

        INTRINSIC VS EXTRINSIC STATE
        Intrinsic state belongs to the shared flyweight and must not depend on the
        individual instance. Extrinsic state is supplied when the flyweight is
        used. A tree's species is intrinsic; its position is extrinsic. A
        character's glyph shape is intrinsic; its screen position and selection
        state are extrinsic.

        A GOOD RULE OF THUMB
        Use Flyweight only after measuring a memory problem and finding a large
        amount of truly shared, immutable data. Sharing is powerful, but it is not
        free complexity.
        """;

    public void Run()
    {
        Console.WriteLine("Planting a small forest with shared tree types...\n");

        var factory = new TreeTypeFactory();
        var forest = new List<Tree>
        {
            new(factory.Get("Oak", "Green", "Rough bark"), 10, 20),
            new(factory.Get("Oak", "Green", "Rough bark"), 35, 18),
            new(factory.Get("Oak", "Green", "Rough bark"), 70, 24),
            new(factory.Get("Pine", "Dark green", "Needle texture"), 15, 60),
            new(factory.Get("Pine", "Dark green", "Needle texture"), 50, 55),
            new(factory.Get("Palm", "Bright green", "Long fronds"), 80, 72)
        };

        foreach (var tree in forest)
        {
            tree.Draw();
        }

        Console.WriteLine();
        Console.WriteLine($"Trees drawn: {forest.Count}");
        Console.WriteLine($"Shared TreeType objects: {factory.SharedTypeCount}");
        Console.WriteLine("Each tree stores its position separately, but all oaks share one type.");
    }

    /// <summary>
    /// Flyweight: immutable data shared by many tree instances.
    /// </summary>
    private sealed class TreeType
    {
        public TreeType(string name, string color, string texture)
        {
            Name = name;
            Color = color;
            Texture = texture;
        }

        public string Name { get; }

        public string Color { get; }

        public string Texture { get; }

        public void Draw(int x, int y)
        {
            Console.WriteLine(
                $"  [{x,3},{y,3}] {Name} ({Color}, {Texture})");
        }
    }

    /// <summary>
    /// Flyweight factory: creates each shared type once and reuses it.
    /// </summary>
    private sealed class TreeTypeFactory
    {
        private readonly Dictionary<string, TreeType> _types = [];

        public int SharedTypeCount => _types.Count;

        public TreeType Get(string name, string color, string texture)
        {
            // The key represents all intrinsic state. If any part differs, it is
            // a different flyweight.
            var key = $"{name}|{color}|{texture}";

            if (!_types.TryGetValue(key, out var type))
            {
                type = new TreeType(name, color, texture);
                _types[key] = type;
                Console.WriteLine($"Created shared type: {name}");
            }

            return type;
        }
    }

    /// <summary>
    /// Context object: stores the unique position and points to the shared type.
    /// </summary>
    private sealed class Tree
    {
        private readonly int _x;
        private readonly int _y;
        private readonly TreeType _type;

        public Tree(TreeType type, int x, int y)
        {
            _type = type;
            _x = x;
            _y = y;
        }

        public void Draw()
        {
            // Position is extrinsic state: it is supplied per tree rather than
            // stored inside the shared TreeType.
            _type.Draw(_x, _y);
        }
    }
}
