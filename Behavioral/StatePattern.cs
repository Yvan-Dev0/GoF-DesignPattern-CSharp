using GoFPatternGuide.Core;

namespace GoFPatternGuide.Behavioral;

/// <summary>
/// STATE
///
/// Intent: let an object change its behavior when its internal state changes,
/// as if it changed class.
///
/// The demo models an order that moves from awaiting payment to paid to shipped.
/// Each state object decides which transitions are valid.
/// </summary>
public sealed class StatePattern : IPatternDemo
{
    public string Name => "State";

    public string Category => PatternCategories.Behavioral;

    public string Summary =>
        "Moves state-specific behavior into state objects so an object can change how it acts.";

    public string Explanation => """
        WHAT IT IS
        State is a behavioral pattern that allows an object to alter its behavior
        when its internal state changes. The object appears to change its class
        because different state objects handle its operations differently.

        The context object holds a reference to a current state. Public operations
        delegate to that state. States can also decide when to move the context to
        another state.

        WHY IT EXISTS
        Objects with lifecycles often behave differently at each stage. An order
        cannot ship before it is paid. A vending machine cannot dispense before
        money is inserted. A document cannot be edited after it is published.

        Without State, the object's methods fill with repeated conditionals that
        check a status field. Every new status adds branches to many methods, and
        invalid transitions are easy to miss.

        State gives each stage its own class with clear rules.

        THE PROBLEM IT SOLVES
        A status-field implementation might look like this:

            if (status == "AwaitingPayment") ...
            else if (status == "Paid") ...
            else if (status == "Shipped") ...

        The same chain appears in Pay, Ship, Cancel, and every future operation.
        The object's behavior is spread across conditionals instead of organized by
        state.

        With State, Pay and Ship delegate to the current state object. The
        AwaitingPayment state knows that Pay is allowed, while the Shipped state
        knows that Pay is not.

        WHEN TO USE IT
        - An object has a lifecycle with distinct modes.
        - Behavior varies significantly by state.
        - Transition rules are important and should be explicit.
        - Many methods contain repeated status conditionals.
        - You want to add a state without editing every operation.

        ADVANTAGES
        - Keeps state-specific behavior together in one class.
        - Removes large conditional blocks from the context.
        - Makes valid transitions explicit.
        - Makes adding a new state easier.
        - Improves readability for complex lifecycles.
        - Supports state-specific data and rules.

        DISADVANTAGES
        - Adds a class per state.
        - Many small states can feel like over-engineering.
        - State transitions can be spread across classes, so the overall lifecycle
          may be harder to see at a glance.
        - The context and states are coupled: states often need to change the
          context.
        - Thread safety and persistence of state need separate design.
        - It can be confused with Strategy when the structure looks similar.

        REAL-WORLD EXAMPLES
        - Order, payment, and shipping lifecycles.
        - Vending machines.
        - Traffic lights.
        - Media players: stopped, playing, paused.
        - Document workflows: draft, review, published, archived.
        - Network connections: disconnected, connecting, connected.
        - Game characters: idle, running, jumping, stunned.

        HOW THE DEMO WORKS
        1. IOrderState defines the operations that every state handles: Pay and
           Ship, plus a readable Name.
        2. Order is the context. It starts in AwaitingPaymentState and delegates
           both operations to its current state.
        3. AwaitingPaymentState accepts payment and moves to PaidState.
        4. PaidState accepts shipping and moves to ShippedState.
        5. ShippedState rejects both further payment and further shipping.
        6. The demo tries an invalid shipment, pays, ships, and then tries an
           invalid payment.

        STATE VS STRATEGY
        The structures are similar: a context holds an interface and delegates.
        Strategy is usually chosen from outside to select an algorithm. State is
        usually changed by the state objects themselves as a lifecycle evolves.
        Strategy answers "which approach should I use?" State answers "what phase
        am I in, and what is allowed now?"

        STATE VS STATUS ENUM
        A simple status enum is fine when almost no behavior changes. State becomes
        valuable when each status has substantial rules and operations. Do not
        create a class hierarchy just to store three strings.

        A GOOD RULE OF THUMB
        When behavior depends heavily on a lifecycle phase, give each phase an
        object and let it enforce its own rules.
        """;

    public void Run()
    {
        Console.WriteLine("Walking through an order lifecycle...\n");

        var order = new Order();
        Console.WriteLine($"Current status: {order.Status}");

        // Invalid transition: payment must happen first.
        order.Ship();

        // Valid transition: awaiting payment -> paid.
        order.Pay();

        // Invalid transition: an order cannot be paid twice.
        order.Pay();

        // Valid transition: paid -> shipped.
        order.Ship();

        // Invalid transition: shipped is the end of this simple lifecycle.
        order.Ship();

        Console.WriteLine($"\nFinal status: {order.Status}");
    }

    // STATE INTERFACE
    private interface IOrderState
    {
        string Name { get; }

        void Pay(Order order);

        void Ship(Order order);
    }

    // CONTEXT
    private sealed class Order
    {
        private IOrderState _state = new AwaitingPaymentState();

        public string Status => _state.Name;

        public void Pay() => _state.Pay(this);

        public void Ship() => _state.Ship(this);

        /// <summary>
        /// Used by state objects to move the order to its next phase. It is
        /// internal to the pattern's classes and not part of the public API.
        /// </summary>
        internal void MoveTo(IOrderState state)
        {
            _state = state;
            Console.WriteLine($"  State changed to: {_state.Name}");
        }
    }

    // CONCRETE STATE 1
    private sealed class AwaitingPaymentState : IOrderState
    {
        public string Name => "Awaiting payment";

        public void Pay(Order order)
        {
            Console.WriteLine("  Payment received.");
            order.MoveTo(new PaidState());
        }

        public void Ship(Order order)
        {
            Console.WriteLine("  Cannot ship: the order has not been paid.");
        }
    }

    // CONCRETE STATE 2
    private sealed class PaidState : IOrderState
    {
        public string Name => "Paid";

        public void Pay(Order order)
        {
            Console.WriteLine("  Cannot pay: the order is already paid.");
        }

        public void Ship(Order order)
        {
            Console.WriteLine("  Shipping label created.");
            order.MoveTo(new ShippedState());
        }
    }

    // CONCRETE STATE 3
    private sealed class ShippedState : IOrderState
    {
        public string Name => "Shipped";

        public void Pay(Order order)
        {
            Console.WriteLine("  Cannot pay: the order has already shipped.");
        }

        public void Ship(Order order)
        {
            Console.WriteLine("  Cannot ship again: the order is already on its way.");
        }
    }
}
