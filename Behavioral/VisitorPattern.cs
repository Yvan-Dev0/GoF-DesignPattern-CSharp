using GoFPatternGuide.Core;

namespace GoFPatternGuide.Behavioral;

/// <summary>
/// VISITOR
///
/// Intent: add operations to an object structure without modifying the classes
/// of the objects being visited.
///
/// The demo has a shopping cart containing books and electronics. Two visitors
/// calculate sales tax and a discounted final price. The product classes do not
/// contain tax or discount formulas.
/// </summary>
public sealed class VisitorPattern : IPatternDemo
{
    public string Name => "Visitor";

    public string Category => PatternCategories.Behavioral;

    public string Summary =>
        "Adds new operations to a stable object structure without editing its classes.";

    public string Explanation => """
        WHAT IT IS
        Visitor is a behavioral pattern that lets you define new operations on a
        group of related objects without changing those objects' classes.

        The pattern has two main parts:
        - elements, which accept a visitor;
        - visitors, which contain an operation for each element type.

        Each element's Accept method calls the correct visitor overload. This is
        called double dispatch: the operation depends on both the element type and
        the visitor type.

        WHY IT EXISTS
        Suppose a shopping cart contains books, electronics, and clothing. The
        product classes hold their data and basic behavior. Now the business needs
        tax calculation, discount calculation, shipping estimates, and an export
        format.

        Adding each operation as a method on every product means editing every
        class whenever a new operation appears. Related products may live in
        different assemblies, and some classes may be sealed or owned by another
        team.

        Visitor moves the operations into separate visitor classes. The product
        classes only need a small Accept method.

        THE PROBLEM IT SOLVES
        Without Visitor, a tax rule might be implemented as a switch over product
        types in a service. Adding a new operation duplicates the type switch.
        Adding a product type means updating every switch and potentially every
        operation.

        With Visitor, each operation is a class with one overload per element type.
        The element's Accept method chooses the overload, so the service does not
        contain a type switch.

        WHEN TO USE IT
        - An object structure is stable, but many new operations are needed.
        - Operations are unrelated to the core responsibility of the elements.
        - You want to keep tax, export, reporting, or analysis code out of domain
          classes.
        - Double dispatch is required to choose behavior by both element and
          operation type.
        - The element hierarchy does not change often.

        ADVANTAGES
        - Adds operations without modifying element classes.
        - Groups related behavior in one visitor class.
        - Makes operations easy to add and understand.
        - Avoids long type-switch statements in services.
        - Can accumulate state while visiting a structure.
        - Keeps domain objects focused on their own data and behavior.

        DISADVANTAGES
        - Adding a new element type requires updating every visitor interface and
          implementation.
        - The double-dispatch flow is harder for beginners to follow.
        - Visitors may need access to element internals, which can weaken
          encapsulation unless carefully designed.
        - It is a poor fit when the element hierarchy changes frequently.
        - It can be overkill when only one or two simple operations exist.

        REAL-WORLD EXAMPLES
        - Tax and discount calculations over a shopping cart.
        - Exporting a document tree to HTML, Markdown, or PDF.
        - Static analysis over an abstract syntax tree.
        - Rendering shapes in different formats.
        - Generating reports from a stable domain model.
        - Applying validation rules to a set of form fields.

        HOW THE DEMO WORKS
        1. IShopItem is the element interface. It exposes Name, Price, and Accept.
        2. Book and Electronics are concrete elements. Each Accept calls the matching
           Visit overload on the visitor.
        3. IShopVisitor declares Visit(Book) and Visit(Electronics).
        4. TaxVisitor calculates sales tax with a different rate for each type.
        5. DiscountVisitor calculates a discounted final price with a different rate
           for each type.
        6. The demo sends the same cart through both visitors.

        WHY ACCEPT AND VISIT ARE BOTH NEEDED
        If the cart simply called visitor.Visit(item), the compiler would choose the
        Visit overload based on the variable's static type, which might be
        IShopItem. Visitor would not know whether the item was a book or an
        electronic device.

        Calling item.Accept(visitor) starts from the concrete item. Inside Book.Accept,
        this is known to be a Book, so visitor.Visit(this) selects Visit(Book).
        That second dispatch is the key to the pattern.

        VISITOR VS POLYMORPHISM
        Ordinary polymorphism is better when the set of operations is stable and
        new element types are common. Visitor is better when the element types are
        stable and new operations are common. Choose based on which dimension
        changes more often.

        A GOOD RULE OF THUMB
        If you keep adding operations but rarely add new item types, Visitor may be
        a good fit. If you keep adding item types, ordinary virtual methods are
        usually simpler.
        """;

    public void Run()
    {
        Console.WriteLine("Sending one shopping cart through two different operations...\n");

        // The cart is a stable structure of element types. Each visitor adds a new
        // operation without changing Book or Electronics.
        IReadOnlyList<IShopItem> cart =
        [
            new Book("Clean Code", 32m),
            new Book("The Pragmatic Programmer", 40m),
            new Electronics("Mechanical keyboard", 85m),
            new Electronics("Monitor", 220m)
        ];

        Console.WriteLine("Cart:");
        foreach (var item in cart)
        {
            Console.WriteLine($"  - {item.Name}: ${item.Price:F2}");
        }

        var visitors = new IShopVisitor[]
        {
            new SalesTaxVisitor(),
            new DiscountVisitor()
        };

        foreach (var visitor in visitors)
        {
            // Sum calls Accept on each element. The element chooses the correct
            // Visit overload based on its concrete type.
            decimal result = cart.Sum(item => item.Accept(visitor));
            Console.WriteLine($"\n{visitor.Name}: ${result:F2}");
        }

        Console.WriteLine("\nNeither product class contains tax or discount logic.");
    }

    // ELEMENT INTERFACE
    private interface IShopItem
    {
        string Name { get; }

        decimal Price { get; }

        decimal Accept(IShopVisitor visitor);
    }

    // CONCRETE ELEMENT 1
    private sealed class Book : IShopItem
    {
        public Book(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        public string Name { get; }

        public decimal Price { get; }

        // The concrete type chooses the Visit(Book) overload.
        public decimal Accept(IShopVisitor visitor) => visitor.Visit(this);
    }

    // CONCRETE ELEMENT 2
    private sealed class Electronics : IShopItem
    {
        public Electronics(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        public string Name { get; }

        public decimal Price { get; }

        public decimal Accept(IShopVisitor visitor) => visitor.Visit(this);
    }

    // VISITOR INTERFACE
    // Every new element type requires a new overload here and in each visitor.
    private interface IShopVisitor
    {
        string Name { get; }

        decimal Visit(Book book);

        decimal Visit(Electronics electronics);
    }

    // CONCRETE VISITOR 1
    private sealed class SalesTaxVisitor : IShopVisitor
    {
        public string Name => "Sales tax";

        // Books often receive a reduced tax rate; electronics use the standard one.
        public decimal Visit(Book book) => book.Price * 0.05m;

        public decimal Visit(Electronics electronics) => electronics.Price * 0.15m;
    }

    // CONCRETE VISITOR 2
    private sealed class DiscountVisitor : IShopVisitor
    {
        public string Name => "Discounted total";

        // These methods return the final price after the visitor's own rule.
        public decimal Visit(Book book) => book.Price * 0.90m;

        public decimal Visit(Electronics electronics) => electronics.Price * 0.95m;
    }
}
