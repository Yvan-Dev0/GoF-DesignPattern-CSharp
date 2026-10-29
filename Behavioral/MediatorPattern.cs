using GoFPatternGuide.Core;

namespace GoFPatternGuide.Behavioral;

/// <summary>
/// MEDIATOR
///
/// Intent: reduce direct communication between objects by routing their messages
/// through one central mediator.
///
/// The demo uses a chat room. Users send messages to the room, and the room
/// delivers each message to the other participants. Users do not know about one
/// another.
/// </summary>
public sealed class MediatorPattern : IPatternDemo
{
    public string Name => "Mediator";

    public string Category => PatternCategories.Behavioral;

    public string Summary =>
        "Centralizes communication so connected objects do not need to know each other.";

    public string Explanation => """
        WHAT IT IS
        Mediator is a behavioral pattern that reduces direct dependencies between
        objects by having them communicate through a central object called the
        mediator.

        The participating objects are often called colleagues. A colleague sends
        a message to the mediator instead of calling other colleagues directly.
        The mediator decides who should receive the message and how the interaction
        should proceed.

        WHY IT EXISTS
        When several objects communicate directly, the number of connections can
        grow rapidly. With five objects, there can be up to ten two-way
        relationships. With twenty objects, the web becomes nearly impossible to
        understand.

        Each object may also need to know details about the others: names, methods,
        availability, and ordering rules. Changing one participant can require
        changes across the system.

        A mediator turns a many-to-many web into a hub. Colleagues depend on the
        mediator, and the mediator coordinates the interactions.

        THE PROBLEM IT SOLVES
        Imagine a chat application where every User holds references to every other
        User and calls them directly. Adding a new user means updating everyone's
        contact list. Removing a user leaves dangling references. Broadcasting a
        message requires custom logic for each recipient.

        With a ChatRoom mediator, each user registers once. Sending a message means
        calling the room. The room owns the broadcast rule and the participant
        list.

        WHEN TO USE IT
        - Many objects communicate in complex, overlapping ways.
        - You want to reduce direct coupling between participants.
        - Communication rules should be centralized and easy to change.
        - Participants should be reusable without knowing the whole group.
        - You are modeling a hub-and-spoke interaction such as a chat room,
          control tower, or event broker.

        ADVANTAGES
        - Reduces many-to-many dependencies to a central hub.
        - Makes communication rules easier to find and change.
        - Keeps participants focused on their own behavior.
        - Makes it easier to add or remove participants.
        - Can simplify testing by replacing the mediator with a test double.

        DISADVANTAGES
        - The mediator can become a large, complex "god object."
        - It can become a single point of failure or a performance bottleneck.
        - Participants still depend on the mediator interface.
        - Too much logic can migrate into the mediator, making it hard to maintain.
        - It may be unnecessary when only two objects communicate.

        REAL-WORLD EXAMPLES
        - Chat rooms where users send through the room.
        - Air traffic control coordinating aircraft.
        - A UI dialog coordinating its form fields and buttons.
        - An event bus or message broker.
        - A game lobby coordinating players.
        - A workflow coordinator that decides which service acts next.

        HOW THE DEMO WORKS
        1. IChatMediator defines Register and Send.
        2. User is the colleague base class. It holds a mediator reference and
           exposes Send and Receive.
        3. ChatUser is a concrete colleague. It adds no special rules; the base
           class is enough for this demo.
        4. ChatRoom is the concrete mediator. It keeps the participant list and
           broadcasts messages to everyone except the sender.
        5. The demo registers three users. Alice sends a message, then Bob does.
           The users never hold references to each other.

        MEDIATOR VS FACADE
        A facade simplifies a group of subsystems for client code, usually in one
        direction. A mediator coordinates communication among colleagues and often
        supports two-way interaction. The mediator is a participant in the
        conversation; a facade is an entry point.

        MEDIATOR VS OBSERVER
        Observer is a one-to-many notification mechanism: a subject notifies its
        subscribers. Mediator is a broader coordination pattern. An event broker
        can use Observer internally to deliver messages, but Mediator focuses on
        centralizing interaction rules.

        A GOOD RULE OF THUMB
        When everyone needs to talk to everyone, give them one place to talk instead.
        """;

    public void Run()
    {
        Console.WriteLine("Opening a chat room with three participants...\n");

        var room = new ChatRoom();
        var alice = new ChatUser("Alice", room);
        var bob = new ChatUser("Bob", room);
        var carol = new ChatUser("Carol", room);

        room.Register(alice);
        room.Register(bob);
        room.Register(carol);

        alice.Send("Has anyone tried the Strategy pattern?");
        Console.WriteLine();
        bob.Send("Yes, it made our checkout code much easier to extend.");

        Console.WriteLine("\nAlice, Bob, and Carol never referenced one another directly.");
    }

    // MEDIATOR INTERFACE
    private interface IChatMediator
    {
        void Register(User user);

        void Send(string message, User sender);
    }

    // COLLEAGUE BASE CLASS
    private abstract class User
    {
        protected User(string name, IChatMediator mediator)
        {
            Name = name;
            Mediator = mediator;
        }

        protected IChatMediator Mediator { get; }

        public string Name { get; }

        public void Send(string message)
        {
            Console.WriteLine($"{Name} sends: {message}");
            Mediator.Send(message, this);
        }

        public virtual void Receive(string senderName, string message)
        {
            Console.WriteLine($"  {Name} receives from {senderName}: {message}");
        }
    }

    // CONCRETE COLLEAGUE
    private sealed class ChatUser : User
    {
        public ChatUser(string name, IChatMediator mediator)
            : base(name, mediator)
        {
        }
    }

    // CONCRETE MEDIATOR
    private sealed class ChatRoom : IChatMediator
    {
        private readonly List<User> _users = [];

        public void Register(User user)
        {
            _users.Add(user);
            Console.WriteLine($"Room: {user.Name} joined.");
        }

        public void Send(string message, User sender)
        {
            // The room owns the broadcast rule. A real chat room might also store
            // history, filter banned users, or send to a group only.
            foreach (var user in _users)
            {
                if (!ReferenceEquals(user, sender))
                {
                    user.Receive(sender.Name, message);
                }
            }
        }
    }
}
