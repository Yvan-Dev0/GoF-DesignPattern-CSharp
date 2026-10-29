using GoFPatternGuide.Core;

namespace GoFPatternGuide.Exercises.Core;

/// <summary>
/// A short recognition quiz: read a design situation and pick the pattern that
/// fits. Some questions are traps where the simplest answer is no pattern at
/// all, because recognizing when NOT to use a pattern is part of the skill.
/// </summary>
internal static class PatternQuiz
{
    private const string NoPattern = "No pattern: keep the simple code";

    private sealed record Question(string Situation, string[] Options, int Answer, string Explanation);

    private static readonly Question[] Questions =
    [
        new(
            "Customers can pay invoices by card, bank transfer, or wallet. Each method " +
            "has its own fee rule, and finance adds a new method every few months. " +
            "The checkout code should not change when that happens.",
            ["Strategy", "Singleton", "Composite", "Memento"],
            0,
            "The fee rule is the thing that varies. Each rule becomes its own " +
            "strategy behind one interface, and checkout only asks for the fee."),
        new(
            "A dashboard, an email digest, and an audit log must all react whenever a " +
            "ticket changes status. More listeners will be added later, and the ticket " +
            "class should not know about any of them.",
            ["Facade", "Observer", "Builder", "Proxy"],
            1,
            "One subject, many independent listeners that come and go: that is " +
            "Observer. The ticket raises an event and does not care who listens."),
        new(
            "Your app must send SMS through a vendor SDK whose method is " +
            "SendTextMessage(string msisdn, string body, int flags). The rest of the " +
            "app already uses an ISmsSender interface with Send(phone, text).",
            ["Bridge", "Decorator", "Adapter", "Mediator"],
            2,
            "Two interfaces that do the same job but do not match. A small adapter " +
            "implements ISmsSender and translates the call to the vendor SDK."),
        new(
            "A settings page reads two values from appsettings.json in one place. " +
            "Nothing else in the app reads configuration, and nothing is expected to.",
            ["Singleton", "Abstract Factory", "Facade", NoPattern],
            3,
            "Trap. One reader in one place needs no pattern. Adding a Singleton or " +
            "factory here would only add classes. Reach for a pattern when the " +
            "problem actually repeats or is about to."),
        new(
            "A sales order can be Draft, Submitted, Approved, Shipped, or Cancelled. " +
            "Every method (Edit, Approve, Ship, Cancel) starts with a long if/else on " +
            "the current status, and the rules keep changing.",
            ["State", "Strategy", "Command", "Iterator"],
            0,
            "The object's behavior depends on its current status, and the status " +
            "changes over time. State moves each status's rules into its own class. " +
            "(Strategy looks similar, but the client chooses a strategy; a state " +
            "chooses the next state itself.)"),
        new(
            "Users want Undo and Redo for actions in a text editor, and the same " +
            "actions must be queueable for a macro recorder.",
            ["Memento", "Command", "Observer", "Visitor"],
            1,
            "Turning each action into an object gives you a queue, a history, and " +
            "an Undo method in one place. Memento could store snapshots, but " +
            "Command is the one that also supports queues and macros."),
        new(
            "Building an HTTP request needs a URL and optionally headers, a timeout, a " +
            "retry policy, a body, and authentication. The constructor already has nine " +
            "parameters, most of them null in practice.",
            ["Prototype", "Factory Method", "Builder", "Adapter"],
            2,
            "Many optional parts assembled step by step is Builder. It replaces a " +
            "telescoping constructor with readable, named steps."),
        new(
            "Approving an expense goes to the team lead, then the manager, then the " +
            "director, depending on the amount. Some departments skip a level, and the " +
            "order may change.",
            ["Chain of Responsibility", "Mediator", "Template Method", "Composite"],
            0,
            "Each approver either handles the request or passes it on. Chain of " +
            "Responsibility lets you reorder or skip levels by relinking the chain."),
        new(
            "Calling an image service is slow. You want to load images only when they " +
            "are first displayed and cache them afterwards, without changing the code " +
            "that displays them.",
            ["Decorator", "Flyweight", "Proxy", "Facade"],
            2,
            "A stand-in with the same interface that controls access to the real " +
            "object (lazy loading and caching) is a Proxy. A Decorator adds new " +
            "behavior; a Proxy controls when and whether the real object is used."),
        new(
            "A finance app shows the totals of folders, sub-folders, and individual " +
            "expense lines. A folder's total is the sum of everything inside it, at " +
            "any depth.",
            ["Iterator", "Composite", "Visitor", "Bridge"],
            1,
            "A tree where single items and groups are treated the same way is " +
            "Composite. Total() on a folder simply sums Total() on its children."),
        new(
            "Two report types (Sales and Inventory) both load data, filter it, format " +
            "it, and save it. Only the loading and formatting steps differ; the order " +
            "of the steps must never change.",
            ["Strategy", "Template Method", "Builder", "State"],
            1,
            "A fixed sequence of steps with some steps filled in by subclasses is " +
            "Template Method. The base class owns the order."),
        new(
            "A method converts a temperature from Celsius to Fahrenheit. A teammate " +
            "suggests an ITemperatureConverterStrategy in case more conversions are " +
            "needed one day.",
            ["Strategy", "Adapter", "Factory Method", NoPattern],
            3,
            "Trap. 'In case one day' is not a recurring problem. Keep the one method; " +
            "introduce Strategy when a second conversion actually appears and the " +
            "callers need to choose between them.")
    ];

    public static void Run()
    {
        ConsoleUi.Clear();
        ConsoleUi.Header("Which Pattern? A Recognition Quiz");
        ConsoleUi.Info($"{Questions.Length} short situations. Pick the design that fits best.");
        ConsoleUi.Muted("Some questions are traps: sometimes the best answer is no pattern at all.");

        var score = 0;
        var answered = 0;

        for (var index = 0; index < Questions.Length; index++)
        {
            var question = Questions[index];
            ConsoleUi.SubHeader($"Question {index + 1} of {Questions.Length}");
            Console.WriteLine(question.Situation);
            Console.WriteLine();

            for (var option = 0; option < question.Options.Length; option++)
            {
                ConsoleUi.WriteLine($"  {option + 1}. {question.Options[option]}", ConsoleColor.Cyan);
            }

            var choice = ConsoleUi.ReadInt("Your answer (0 to stop): ", 0, question.Options.Length);
            if (choice <= 0)
            {
                break;
            }

            answered++;
            if (choice - 1 == question.Answer)
            {
                score++;
                ConsoleUi.Success("Correct.");
            }
            else
            {
                ConsoleUi.Error($"Not quite. The best answer is: {question.Options[question.Answer]}.");
            }

            ConsoleUi.Muted(question.Explanation);
        }

        Console.WriteLine();
        ConsoleUi.Rule();
        ConsoleUi.Info($"Score: {score} of {answered} answered.");
        ConsoleUi.PressEnter("Press Enter to return to the main menu...");
    }
}
