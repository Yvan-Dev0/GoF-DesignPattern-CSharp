using GoFPatternGuide.Core;

namespace GoFPatternGuide.Structural;

/// <summary>
/// ADAPTER
///
/// Intent: let objects with incompatible interfaces work together by wrapping
/// one of them in a translator.
///
/// The demo has a laptop charger that expects a US outlet. A European wall
/// socket supplies power through a different method. The adapter implements the
/// interface the charger expects and delegates to the socket.
/// </summary>
public sealed class AdapterPattern : IPatternDemo
{
    public string Name => "Adapter";

    public string Category => PatternCategories.Structural;

    public string Summary =>
        "Connects incompatible interfaces by wrapping one object in a translator.";

    public string Explanation => """
        WHAT IT IS
        Adapter is a structural pattern that allows two objects with incompatible
        interfaces to collaborate. It wraps an existing object (the adaptee) in a
        new object (the adapter) that implements the interface the client expects.

        Think of a travel power adapter. The wall socket and the laptop charger
        both work correctly, but their physical connections do not match. The
        adapter translates one connection into the other without changing either
        device.

        WHY IT EXISTS
        We often need to use code we cannot or should not change: a third-party
        library, a legacy service, or a platform API. That code may have useful
        behavior but an interface that does not fit our application.

        Rewriting the library is risky. Changing every caller is noisy. Adapter
        places the translation in one small class and keeps the rest of the
        application dependent on a clean interface.

        THE PROBLEM IT SOLVES
        Suppose our application defines IUsOutlet with a method SupplyPower. A
        European socket offers Supply230Volts instead. The charger cannot call
        the socket directly because the method names and contracts differ.

        An adapter implements IUsOutlet. Its SupplyPower method calls the socket's
        Supply230Volts method and converts the result into the format the charger
        understands.

        WHEN TO USE IT
        - You need to use an existing class whose interface does not match.
        - You want to integrate a third-party or legacy component without spreading
          its types through your codebase.
        - You need to support several implementations behind one stable interface.
        - You want to isolate compatibility code in one place.
        - You are migrating gradually from an old API to a new one.

        ADVANTAGES
        - Reuses existing code without modifying it.
        - Keeps incompatible details at the boundary of the system.
        - Makes third-party dependencies easier to replace.
        - Allows one client interface to serve several underlying implementations.
        - Separates translation logic from business logic.

        DISADVANTAGES
        - Adds an extra layer and an extra object.
        - Too many adapters can make a codebase harder to navigate.
        - An adapter can hide poor performance or awkward behavior in the adaptee.
        - It translates interfaces, not deep semantic differences; if the underlying
          models disagree about meaning, the adapter may become complex.
        - A badly designed adapter can become a dumping ground for unrelated logic.

        REAL-WORLD EXAMPLES
        - Power adapters between plug standards.
        - A payment library adapter that maps Stripe, PayPal, or a bank API to one
          IPaymentGateway interface.
        - A legacy database wrapper that exposes modern repository methods.
        - A file-format reader that makes a third-party parser look like the app's
          own document reader.
        - .NET stream adapters such as StreamReader or StreamWriter, which adapt a
          byte stream into text reading and writing.

        HOW THE DEMO WORKS
        1. IUsOutlet is the target interface the laptop charger understands.
        2. EuropeanSocket is the adaptee. It supplies 230 volts through its own
           method.
        3. EuropeanToUsAdapter implements IUsOutlet and holds a EuropeanSocket.
        4. LaptopCharger depends only on IUsOutlet, so it never needs to know that
           a European socket is involved.

        OBJECT ADAPTER VS CLASS ADAPTER
        The demo is an object adapter: it contains a reference to the adaptee and
        delegates calls. This is the most common form in C#.

        A class adapter would inherit from both the target and the adaptee. C#
        does not support multiple class inheritance, so that form is rare. Object
        adaptation also works when the adaptee is sealed or comes from a library.

        ADAPTER VS FACADE
        Adapter changes an existing interface so it matches what a client expects.
        Facade creates a simpler interface over a group of subsystems. Adapter is
        about compatibility; Facade is about convenience.

        ADAPTER VS DECORATOR
        Adapter changes the interface. Decorator keeps the same interface and adds
        behavior around an object. Both wrap, but their purpose is different.

        A GOOD RULE OF THUMB
        Put an adapter at the boundary where two worlds meet, and keep the rest of
        your application speaking one consistent language.
        """;

    public void Run()
    {
        Console.WriteLine("A US laptop charger meets a European wall socket...\n");

        // The charger only understands IUsOutlet. It cannot call the European
        // socket's method directly.
        var charger = new LaptopCharger();
        var europeanSocket = new EuropeanSocket();

        // The adapter is the bridge between the two contracts.
        IUsOutlet adapter = new EuropeanToUsAdapter(europeanSocket);

        charger.Charge(adapter);

        Console.WriteLine("\nThe charger was not changed, and the socket was not changed.");
        Console.WriteLine("Only the small adapter knows how to translate between them.");
    }

    // TARGET
    // The interface the client already understands.
    private interface IUsOutlet
    {
        string SupplyPower();
    }

    // ADAPTEE
    // Existing code with a different interface. It might come from a third-party
    // library, so we pretend it cannot be edited.
    private sealed class EuropeanSocket
    {
        public string Supply230Volts() =>
            "230V at 50Hz from a European wall socket";
    }

    // ADAPTER
    // Implements the target and translates to the adaptee.
    private sealed class EuropeanToUsAdapter : IUsOutlet
    {
        private readonly EuropeanSocket _socket;

        public EuropeanToUsAdapter(EuropeanSocket socket)
        {
            _socket = socket;
        }

        public string SupplyPower()
        {
            // Translation happens here. In a real adapter this is where data
            // formats, units, exceptions, or method names would be mapped.
            var europeanPower = _socket.Supply230Volts();
            return $"Adapter converted: {europeanPower} -> compatible US charger input";
        }
    }

    // CLIENT
    // It depends on IUsOutlet and knows nothing about adapters or sockets.
    private sealed class LaptopCharger
    {
        public void Charge(IUsOutlet outlet)
        {
            Console.WriteLine($"Charger received: {outlet.SupplyPower()}");
            Console.WriteLine("Laptop is charging.");
        }
    }
}
