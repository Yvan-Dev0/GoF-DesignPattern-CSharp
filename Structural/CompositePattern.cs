using GoFPatternGuide.Core;

namespace GoFPatternGuide.Structural;

/// <summary>
/// COMPOSITE
///
/// Intent: treat individual objects and groups of objects uniformly through one
/// common interface.
///
/// The demo models files and folders. A file is a leaf, and a folder is a
/// composite that can contain files or other folders. Both can report a size and
/// display themselves.
/// </summary>
public sealed class CompositePattern : IPatternDemo
{
    public string Name => "Composite";

    public string Category => PatternCategories.Structural;

    public string Summary =>
        "Treats a single item and a tree of items through the same interface.";

    public string Explanation => """
        WHAT IT IS
        Composite is a structural pattern that arranges objects in a tree and lets
        clients treat individual objects and groups of objects in the same way.

        The tree has two kinds of participants:
        - a leaf, which has no children;
        - a composite, which contains child components.

        Both leaf and composite implement the same component interface. Client
        code can call an operation without checking whether it received a file or
        a folder.

        WHY IT EXISTS
        Many real structures are hierarchies: folders contain files and folders,
        menus contain items and submenus, an organization has teams and employees,
        and a drawing contains shapes and groups of shapes.

        Without Composite, code is full of checks like "if this is a folder, loop
        over its children; otherwise handle the file." Every new operation repeats
        that branching and every new node type requires changes.

        Composite moves the recursion into the tree objects. The client asks the
        root to do something, and the structure handles the details.

        THE PROBLEM IT SOLVES
        Suppose an application needs to calculate the total size of a project.
        Files have sizes. Folders do not have a direct size; their size is the sum
        of their contents. A naive implementation would recursively inspect the
        concrete types and duplicate that traversal for displaying, deleting, and
        searching.

        With Composite, Folder calculates its size by asking each child for its
        size. Since children also implement the component interface, the same call
        works at every level.

        WHEN TO USE IT
        - Your domain is naturally a tree.
        - Clients should not care whether they are handling one item or a group.
        - Operations such as display, size, search, or draw apply to both leaves
          and groups.
        - The hierarchy can grow to arbitrary depth.
        - You want to add new node types while keeping client code simple.

        ADVANTAGES
        - Makes tree structures easy to work with.
        - Removes type checks and special cases from client code.
        - Recursive operations are expressed naturally.
        - New leaf or composite types can fit the same interface.
        - The client can treat the whole tree and a single node uniformly.

        DISADVANTAGES
        - The common interface can become awkward if leaves and composites have
          very different capabilities.
        - It is tempting to put child-management methods on the shared interface,
          which forces leaves to implement meaningless methods.
        - Deep or cyclic structures need care to avoid stack or traversal problems.
        - A general component interface can make the design less explicit.
        - Small hierarchies may not justify the extra abstraction.

        REAL-WORLD EXAMPLES
        - Files and folders.
        - UI layouts, panels, and controls.
        - Menus, submenus, and menu items.
        - Organization charts.
        - Graphics groups containing shapes and other groups.
        - Document outlines with sections and subsections.

        HOW THE DEMO WORKS
        1. IFileSystemItem is the component. It promises Name, SizeInBytes, and
           Display.
        2. FileItem is a leaf. It returns its own size.
        3. Folder is a composite. It stores a list of children and adds their
           sizes recursively.
        4. The demo builds a project tree and asks the root for its size and
           display. The root does not know or care that some children are folders.

        SAFETY VS TRANSPARENCY
        One design choice is whether AddChild belongs on the component interface.
        Putting it there makes every node look the same, but leaves must implement
        AddChild by throwing an exception or doing nothing. Keeping AddChild only
        on Folder is safer but less uniform. This demo chooses the safer version:
        only folders know how to manage children.

        A GOOD RULE OF THUMB
        If client code keeps asking "is this a group or a single item?", Composite
        may let the whole tree answer the question for you.
        """;

    public void Run()
    {
        Console.WriteLine("Building a small project tree...\n");

        // Leaves
        var readme = new FileItem("README.md", 4_200);
        var program = new FileItem("Program.cs", 8_900);
        var catalog = new FileItem("PatternCatalog.cs", 6_100);
        var screenshot = new FileItem("menu.png", 245_000);

        // Composites can contain leaves and other composites.
        var sourceFolder = new Folder("Source");
        sourceFolder.Add(program);
        sourceFolder.Add(catalog);

        var docsFolder = new Folder("Docs");
        docsFolder.Add(readme);
        docsFolder.Add(screenshot);

        var projectFolder = new Folder("GoFPatternGuide");
        projectFolder.Add(sourceFolder);
        projectFolder.Add(docsFolder);

        projectFolder.Display();

        Console.WriteLine();
        Console.WriteLine($"Total project size: {projectFolder.SizeInBytes:N0} bytes");
        Console.WriteLine(
            "The same Display and SizeInBytes calls worked on individual files and folders.");
    }

    // COMPONENT
    private interface IFileSystemItem
    {
        string Name { get; }

        long SizeInBytes { get; }

        void Display(string indent = "");
    }

    // LEAF
    private sealed class FileItem : IFileSystemItem
    {
        public FileItem(string name, long sizeInBytes)
        {
            Name = name;
            SizeInBytes = sizeInBytes;
        }

        public string Name { get; }

        public long SizeInBytes { get; }

        public void Display(string indent = "")
        {
            Console.WriteLine($"{indent}- {Name} ({SizeInBytes:N0} bytes)");
        }
    }

    // COMPOSITE
    private sealed class Folder : IFileSystemItem
    {
        private readonly List<IFileSystemItem> _children = [];

        public Folder(string name)
        {
            Name = name;
        }

        public string Name { get; }

        // A folder's size is computed from its children. The recursion is hidden
        // behind the component interface.
        public long SizeInBytes => _children.Sum(child => child.SizeInBytes);

        public void Add(IFileSystemItem item)
        {
            ArgumentNullException.ThrowIfNull(item);
            _children.Add(item);
        }

        public void Display(string indent = "")
        {
            Console.WriteLine(
                $"{indent}+ {Name}/ ({SizeInBytes:N0} bytes total)");

            foreach (var child in _children)
            {
                // Each child prints itself. A file stops; a folder continues the
                // recursion with increased indentation.
                child.Display(indent + "  ");
            }
        }
    }
}
