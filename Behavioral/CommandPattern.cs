using GoFPatternGuide.Core;

namespace GoFPatternGuide.Behavioral;

/// <summary>
/// COMMAND
///
/// Intent: encapsulate a request as an object so it can be stored, queued,
/// logged, or undone.
///
/// The demo wraps a light's on/off operations in command objects and lets a
/// remote control execute and undo the last command.
/// </summary>
public sealed class CommandPattern : IPatternDemo
{
    public string Name => "Command";

    public string Category => PatternCategories.Behavioral;

    public string Summary =>
        "Turns a request into an object that can be executed, stored, queued, or undone.";

    public string Explanation => """
        WHAT IT IS
        Command is a behavioral pattern that packages a request as a standalone
        object. The command object contains enough information to perform the
        action, including a reference to the receiver that will do the work.

        The object that sends the request (the invoker) does not need to know the
        details of the receiver. It only calls Execute on a command. Commands can
        also expose Undo, which makes reversible operations possible.

        WHY IT EXISTS
        Direct method calls are fine when the sender and receiver are tightly
        connected. But sometimes a request must be delayed, queued, logged,
        retried, grouped, or reversed. A raw method call disappears after it runs;
        it cannot be stored in a list or placed on a queue.

        Turning the request into an object gives it a lifetime. The program can
        keep it, inspect it, send it to another component, or undo it.

        THE PROBLEM IT SOLVES
        Imagine a smart-home remote with buttons. If each button directly calls
        light.TurnOn(), the remote is permanently coupled to that exact light and
        method. Adding undo means every button needs custom bookkeeping. Adding
        macros means the remote must know how to combine device calls.

        With Command, each button holds an ICommand. A TurnOnLightCommand knows
        which light to turn on and how to reverse it. The remote simply executes
        commands and keeps the last one for undo.

        WHEN TO USE IT
        - Requests should be parameterized or stored for later.
        - You need undo/redo.
        - Operations should be queued, scheduled, or sent across a boundary.
        - You want to log or audit actions.
        - A UI button should be decoupled from the business operation it triggers.
        - You need to group commands into macros or transactions.

        ADVANTAGES
        - Decouples the invoker from the receiver.
        - Makes requests first-class objects that can be stored and passed around.
        - Enables undo, redo, queues, scheduling, and logging.
        - Supports composite commands and macros.
        - Makes operations easy to test in isolation.

        DISADVANTAGES
        - Adds a class for each operation, which can feel heavy for small apps.
        - Undo is difficult when an operation is not naturally reversible.
        - Commands can accumulate in memory if history is unbounded.
        - Too many tiny command classes can obscure a simple workflow.
        - Correct undo may require storing previous state and handling failures.

        REAL-WORLD EXAMPLES
        - Menu items, toolbar buttons, and keyboard shortcuts in an editor.
        - Undo/redo stacks in text editors and drawing programs.
        - Job queues and background task processing.
        - Database transactions represented as operations.
        - Macro recording in a game or productivity tool.
        - Remote controls and smart-home automation.

        HOW THE DEMO WORKS
        1. Light is the receiver. It knows how to turn itself on and off.
        2. ICommand defines Execute and Undo.
        3. TurnOnLightCommand and TurnOffLightCommand hold a Light and implement the
           two operations.
        4. RemoteControl is the invoker. It executes whichever command it receives
           and remembers the last command for undo.
        5. The demo turns the light on, undoes it, turns it off, and undoes again.

        COMMAND VS STRATEGY
        Strategy selects one algorithm for a task. Command packages a request,
        including its receiver and the ability to reverse or store it. A command
        often says "do this specific thing"; a strategy says "use this approach."

        COMMAND VS OBSERVER
        Observer broadcasts a notification to many listeners. Command represents a
        specific action to be performed, usually by one receiver.

        A GOOD RULE OF THUMB
        If you need to treat an action like data, Command is the pattern to reach
        for.
        """;

    public void Run()
    {
        Console.WriteLine("Using a remote control with undo support...\n");

        var light = new Light();
        var remote = new RemoteControl();

        // The remote has no reference to Light. It only sees ICommand objects.
        ICommand turnOn = new TurnOnLightCommand(light);
        ICommand turnOff = new TurnOffLightCommand(light);

        remote.Press(turnOn);
        remote.Press(turnOff);
        remote.UndoLast();
        remote.UndoLast();

        Console.WriteLine($"\nFinal light state: {(light.IsOn ? "ON" : "OFF")}");
    }

    // RECEIVER
    private sealed class Light
    {
        public bool IsOn { get; private set; }

        public void TurnOn()
        {
            IsOn = true;
            Console.WriteLine("Light: ON");
        }

        public void TurnOff()
        {
            IsOn = false;
            Console.WriteLine("Light: OFF");
        }
    }

    // COMMAND INTERFACE
    private interface ICommand
    {
        string Name { get; }

        void Execute();

        void Undo();
    }

    // CONCRETE COMMAND 1
    private sealed class TurnOnLightCommand : ICommand
    {
        private readonly Light _light;

        public TurnOnLightCommand(Light light)
        {
            _light = light;
        }

        public string Name => "Turn light on";

        public void Execute() => _light.TurnOn();

        // Undo is the opposite operation in this simple case.
        public void Undo() => _light.TurnOff();
    }

    // CONCRETE COMMAND 2
    private sealed class TurnOffLightCommand : ICommand
    {
        private readonly Light _light;

        public TurnOffLightCommand(Light light)
        {
            _light = light;
        }

        public string Name => "Turn light off";

        public void Execute() => _light.TurnOff();

        public void Undo() => _light.TurnOn();
    }

    // INVOKER
    // It knows when to run a command and keeps a tiny undo history.
    private sealed class RemoteControl
    {
        private ICommand? _lastCommand;

        public void Press(ICommand command)
        {
            Console.WriteLine($"Remote: executing '{command.Name}'.");
            command.Execute();
            _lastCommand = command;
        }

        public void UndoLast()
        {
            if (_lastCommand is null)
            {
                Console.WriteLine("Remote: nothing to undo.");
                return;
            }

            Console.WriteLine($"Remote: undoing '{_lastCommand.Name}'.");
            _lastCommand.Undo();
            _lastCommand = null; // This demo supports one-step undo only.
        }
    }
}
