using GoFPatternGuide.Core;

namespace GoFPatternGuide.Behavioral;

/// <summary>
/// CHAIN OF RESPONSIBILITY
///
/// Intent: pass a request along a chain of handlers until one of them handles it.
///
/// The demo routes expense requests through a team lead, a manager, and a
/// director. Each approver has a spending limit and can pass the request onward.
/// </summary>
public sealed class ChainOfResponsibilityPattern : IPatternDemo
{
    public string Name => "Chain of Responsibility";

    public string Category => PatternCategories.Behavioral;

    public string Summary =>
        "Passes a request along a chain of handlers until one handler can deal with it.";

    public string Explanation => """
        WHAT IT IS
        Chain of Responsibility is a behavioral pattern that lets a request travel
        through a sequence of handlers. Each handler decides whether it can process
        the request. If it cannot, it forwards the request to the next handler.

        The sender does not need to know which handler will eventually succeed. It
        only needs to send the request into the chain.

        WHY IT EXISTS
        Many workflows have escalation rules. A small expense can be approved by a
        team lead. A medium expense needs a manager. A large expense needs a
        director. If the sender contains a long if/else ladder for every possible
        approver, adding a new approval level means editing the sender.

        Chain of Responsibility moves the decision into small handler objects. The
        chain can be assembled once and reused, and each handler has one clear rule.

        THE PROBLEM IT SOLVES
        A simple implementation might look like this:

            if (amount <= 100) teamLead.Approve();
            else if (amount <= 1000) manager.Approve();
            else if (amount <= 10000) director.Approve();

        This works until the rules change. Perhaps the manager is on vacation, or a
        finance department must be inserted between manager and director. The
        sender must know every level and every threshold.

        With a chain, the sender just calls Handle. Each approver either approves
        or passes the request along. The chain structure is separate from the
        request-sending code.

        WHEN TO USE IT
        - Several objects may handle a request, but the correct one is not known
          in advance.
        - Handlers can be organized in a meaningful order.
        - You want to add or reorder handlers without changing the sender.
        - A request should stop at the first handler that can process it.
        - You want to avoid deeply nested conditional logic.

        ADVANTAGES
        - Decouples the sender from the receivers.
        - Makes each handler small and focused.
        - Lets you change the chain at runtime.
        - Supports adding new handlers without editing client code.
        - Avoids long if/else ladders.
        - Can model escalation and approval workflows clearly.

        DISADVANTAGES
        - There is no guarantee that any handler will process the request.
        - A request may pass through many handlers, making behavior harder to trace.
        - Debugging requires following the chain.
        - Poorly designed chains can become cyclic or extremely long.
        - It may be less direct than a simple switch for a tiny, stable set of rules.

        REAL-WORLD EXAMPLES
        - Expense or purchase approval limits.
        - Customer-support tickets escalating through tiers.
        - HTTP middleware pipelines that pass a request through logging,
          authentication, compression, and routing.
        - Event bubbling in a user interface.
        - Exception handling by successive catch blocks.
        - Fraud checks applied in a sequence until one blocks a transaction.

        HOW THE DEMO WORKS
        1. Expense is the request, carrying a purpose and an amount.
        2. Approver is the handler base class. It stores the next handler and
           implements the forwarding logic once.
        3. TeamLead, Manager, and Director each implement CanApprove and a role name.
        4. The demo builds the chain team lead -> manager -> director.
        5. Each expense enters at the front. The first approver with enough authority
           handles it; otherwise the request moves to the next link.
        6. A request larger than every limit falls off the end and receives a clear
           "no approver" response.

        IMPORTANT DESIGN CHOICE
        A handler can either stop after handling or continue passing the request.
        This demo stops at the first approver. Middleware pipelines often continue
        and let every handler act. Both are valid variations of the same idea.

        A GOOD RULE OF THUMB
        When a request should try several possible handlers in order, build a chain
        rather than teaching the sender every rule.
        """;

    public void Run()
    {
        Console.WriteLine("Building an approval chain: Team Lead -> Manager -> Director\n");

        // Build the chain once. SetNext returns the next approver so the calls can
        // be chained, but each object remains a normal handler.
        var teamLead = new TeamLead();
        teamLead
            .SetNext(new Manager())
            .SetNext(new Director());

        var requests = new[]
        {
            new Expense("Team lunch", 85m),
            new Expense("Conference ticket", 750m),
            new Expense("New laptop", 4_500m),
            new Expense("Department retreat", 18_000m)
        };

        foreach (var request in requests)
        {
            Console.WriteLine($"Request: {request.Purpose} (${request.Amount:N0})");
            Console.WriteLine($"  {teamLead.Handle(request)}");
            Console.WriteLine();
        }

        Console.WriteLine("The sender never checked the amount against each role's limit.");
    }

    // REQUEST
    private sealed record Expense(string Purpose, decimal Amount);

    // HANDLER
    // The base class owns the forwarding behavior so every concrete handler does
    // not have to duplicate it.
    private abstract class Approver
    {
        private Approver? _next;

        /// <summary>
        /// Connects the next handler and returns it, which makes chain building
        /// read naturally.
        /// </summary>
        public Approver SetNext(Approver next)
        {
            _next = next;
            return next;
        }

        public string Handle(Expense expense)
        {
            if (CanApprove(expense))
            {
                return $"{Role} approved it.";
            }

            // If this handler cannot approve, pass the request to the next link.
            return _next is not null
                ? _next.Handle(expense)
                : "No approver in the chain has enough authority.";
        }

        protected abstract string Role { get; }

        protected abstract bool CanApprove(Expense expense);
    }

    // CONCRETE HANDLER 1
    private sealed class TeamLead : Approver
    {
        protected override string Role => "Team Lead";

        protected override bool CanApprove(Expense expense) => expense.Amount <= 100m;
    }

    // CONCRETE HANDLER 2
    private sealed class Manager : Approver
    {
        protected override string Role => "Manager";

        protected override bool CanApprove(Expense expense) => expense.Amount <= 1_000m;
    }

    // CONCRETE HANDLER 3
    private sealed class Director : Approver
    {
        protected override string Role => "Director";

        protected override bool CanApprove(Expense expense) => expense.Amount <= 10_000m;
    }
}
