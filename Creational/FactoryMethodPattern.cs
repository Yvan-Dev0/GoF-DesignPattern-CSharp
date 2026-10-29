using GoFPatternGuide.Core;

namespace GoFPatternGuide.Creational;

/// <summary>
/// FACTORY METHOD
///
/// Intent: define a method for creating an object, but let subclasses decide
/// which concrete class to instantiate.
///
/// NotificationService owns the reusable notification workflow. Each subclass
/// supplies the kind of INotification it wants to create.
/// </summary>
public sealed class FactoryMethodPattern : IPatternDemo
{
    public string Name => "Factory Method";

    public string Category => PatternCategories.Creational;

    public string Summary =>
        "Lets subclasses choose which concrete object a reusable workflow creates.";

    public string Explanation => """
        WHAT IT IS
        Factory Method is a creational pattern that defines an operation for
        creating an object, then lets subclasses decide which concrete class to
        instantiate. The creator class contains useful work that depends on the
        created object, but it depends only on a shared interface.

        In the classic form, a base creator declares an abstract factory method.
        Concrete creators override that method and return their own product. The
        base class calls the factory method from its normal business logic.

        WHY IT EXISTS
        A common mistake is to mix a useful workflow with hard-coded object
        creation. A notification service might send messages today by email, but
        the sending method might contain "new EmailNotification()". Adding SMS
        means editing that method. Soon the method is a pile of if statements and
        the workflow is different every time.

        Factory Method separates two questions:
        - What steps does every notification workflow share?
        - Which notification channel is used for this service?

        The workflow stays in one place, and creation becomes a replaceable
        extension point.

        THE PROBLEM IT SOLVES
        Suppose a report generator must export PDF and CSV reports. If the base
        class directly creates a PdfExporter, CSV support requires changing the
        base class. The base class must know every future format. This violates
        the open/closed principle: code should be open for extension but closed
        for modification.

        With Factory Method, a ReportGenerator declares CreateExporter. PdfReport
        and CsvReport override it. The shared generation steps remain untouched.

        WHEN TO USE IT
        - A class cannot know in advance which concrete object it should create.
        - A reusable workflow needs a product supplied by subclasses.
        - You want to keep creation logic out of business logic.
        - A library wants to provide extension points for consumers.
        - You need to centralize creation while allowing multiple variants.

        ADVANTAGES
        - Removes direct dependencies on concrete product classes.
        - Keeps one reusable workflow instead of duplicated variants.
        - New products can often be added without changing existing client code.
        - Makes the creation decision explicit and testable.
        - Supports substitution with test doubles or alternative implementations.

        DISADVANTAGES
        - Requires a class hierarchy, even for a small variation.
        - Adds indirection: a method call must be followed to see the product.
        - Can lead to many small subclasses for simple differences.
        - If products need to be created dynamically from configuration, a
          registry or dependency injection container may fit better.
        - It is often confused with Abstract Factory, which creates families.

        REAL-WORLD EXAMPLES
        - A document editor whose application class creates the correct document
          type for each platform.
        - Notification frameworks with email, SMS, push, and in-app channels.
        - Logistics systems where a base planner creates the right transport.
        - UI frameworks where each platform supplies native controls.
        - Test frameworks that let a subclass provide a fixture or test data object.

        HOW THE DEMO WORKS
        1. INotification is the product interface. It promises Send.
        2. EmailNotification and SmsNotification are concrete products.
        3. NotificationService is the creator. Its Notify method contains the
           shared workflow: validate, create, and send.
        4. The abstract CreateNotification method is the factory method.
        5. Two concrete services choose different products without changing the
           workflow in the base class.

        FACTORY METHOD VS A SIMPLE FACTORY
        A "simple factory" is often just a class with a switch that returns an
        object. That is a useful technique, but it is not the GoF Factory Method
        pattern. Factory Method uses inheritance or implementation of a creation
        operation to let a subclass decide the product while the base class owns
        the workflow.

        FACTORY METHOD VS ABSTRACT FACTORY
        Factory Method usually creates one product. Abstract Factory creates a
        family of related products through several creation methods.

        A GOOD RULE OF THUMB
        If you can describe a stable workflow with one replaceable creation step,
        Factory Method is a natural fit.
        """;

    public void Run()
    {
        Console.WriteLine("The same notification workflow, with two product choices:\n");

        // The caller sees only NotificationService. It does not create an email
        // or SMS object itself, so the workflow cannot be bypassed accidentally.
        NotificationService emailService = new EmailNotificationService();
        NotificationService smsService = new SmsNotificationService();

        emailService.Notify("ana@example.com", "Your order has shipped.");
        Console.WriteLine();
        smsService.Notify("+1-555-0100", "Your order has shipped.");

        Console.WriteLine("\nBoth services share Notify, but each supplies its own product.");
    }

    // PRODUCT INTERFACE
    // The creator's reusable code knows only these members.
    private interface INotification
    {
        string Channel { get; }

        void Send(string recipient, string message);
    }

    // CONCRETE PRODUCT 1
    private sealed class EmailNotification : INotification
    {
        public string Channel => "Email";

        public void Send(string recipient, string message)
        {
            Console.WriteLine($"  [{Channel}] To: {recipient}");
            Console.WriteLine($"  Subject: Order update");
            Console.WriteLine($"  Body: {message}");
        }
    }

    // CONCRETE PRODUCT 2
    private sealed class SmsNotification : INotification
    {
        public string Channel => "SMS";

        public void Send(string recipient, string message)
        {
            Console.WriteLine($"  [{Channel}] To: {recipient}");
            Console.WriteLine($"  Text: {message}");
        }
    }

    // CREATOR
    // This class owns the shared algorithm. It does not know which concrete
    // notification it will receive.
    private abstract class NotificationService
    {
        /// <summary>
        /// The factory method. Subclasses decide which product is created.
        /// </summary>
        protected abstract INotification CreateNotification();

        /// <summary>
        /// Shared workflow. It calls the factory method instead of using new
        /// directly on a concrete product.
        /// </summary>
        public void Notify(string recipient, string message)
        {
            if (string.IsNullOrWhiteSpace(recipient))
            {
                Console.WriteLine("  Notification skipped: no recipient.");
                return;
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                Console.WriteLine("  Notification skipped: empty message.");
                return;
            }

            INotification notification = CreateNotification();
            Console.WriteLine($"Preparing notification via {notification.Channel}...");
            notification.Send(recipient, message);
        }
    }

    // CONCRETE CREATOR 1
    private sealed class EmailNotificationService : NotificationService
    {
        protected override INotification CreateNotification() => new EmailNotification();
    }

    // CONCRETE CREATOR 2
    private sealed class SmsNotificationService : NotificationService
    {
        protected override INotification CreateNotification() => new SmsNotification();
    }
}
