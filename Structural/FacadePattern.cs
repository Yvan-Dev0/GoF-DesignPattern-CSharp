using GoFPatternGuide.Core;

namespace GoFPatternGuide.Structural;

/// <summary>
/// FACADE
///
/// Intent: provide one simple interface to a group of related subsystems.
///
/// The demo coordinates lights, a projector, a sound system, and a streaming
/// player behind one WatchMovie operation.
/// </summary>
public sealed class FacadePattern : IPatternDemo
{
    public string Name => "Facade";

    public string Category => PatternCategories.Structural;

    public string Summary =>
        "Gives client code one simple entry point for a complicated group of subsystems.";

    public string Explanation => """
        WHAT IT IS
        Facade is a structural pattern that provides a simplified, higher-level
        interface over a set of related classes. The facade knows the order in
        which the subsystems should be used and exposes operations that match the
        client's goals.

        A facade does not necessarily hide the subsystems completely. It is a
        convenience layer. Advanced clients may still use the underlying classes
        directly when they need more control.

        WHY IT EXISTS
        Complex features often require many low-level steps. Starting a movie
        night might mean dimming the lights, turning on the projector, selecting
        an input, powering up the sound system, choosing an audio mode, starting
        the player, and lowering the volume for the opening credits.

        If every caller repeats those steps, the application becomes fragile. A
        change in startup order must be fixed in many places, and a new developer
        must learn every subsystem before doing a simple task.

        A facade captures the common workflow in one place and gives it a name.

        THE PROBLEM IT SOLVES
        Suppose a smart-home app has separate controllers for lights, speakers,
        and a screen. Every button in the app repeats a long sequence of calls.
        If one call is forgotten, the system enters an inconsistent state. If a
        subsystem changes, every caller may need editing.

        HomeTheaterFacade offers WatchMovie and EndMovie. The client calls one
        method, and the facade coordinates the details in the correct order.

        WHEN TO USE IT
        - Client code must coordinate several related subsystems.
        - A common workflow has a fixed order or important setup steps.
        - You want to reduce coupling between application code and a complex library.
        - You need a simpler API for common tasks while keeping advanced access.
        - You are introducing a boundary around a legacy or third-party system.

        ADVANTAGES
        - Makes common tasks simple and expressive.
        - Reduces the number of dependencies client code must know about.
        - Centralizes workflow and ordering rules.
        - Makes the system easier for new developers to use.
        - Can provide a stable boundary around a changing subsystem.
        - Does not prevent advanced code from using lower-level classes.

        DISADVANTAGES
        - A facade can become a "god object" if it accumulates every possible
          operation.
        - It may hide useful capabilities or make advanced use less discoverable.
        - It adds another layer to debug and maintain.
        - If the facade is the only allowed path, it can become a bottleneck for
          changes.
        - It does not add new behavior; it mostly coordinates existing behavior.

        REAL-WORLD EXAMPLES
        - A home theater "movie night" button.
        - A computer's power button coordinating motherboard, storage, fans, and OS.
        - A payment facade that validates, charges, records, and sends a receipt.
        - A library API that hides several internal parsers and caches.
        - A service layer that coordinates repositories, email, and logging.
        - A one-click deployment command wrapping build, test, and publish steps.

        HOW THE DEMO WORKS
        1. RoomLights, Projector, SoundSystem, and StreamingPlayer are small
           subsystem classes with their own low-level operations.
        2. HomeTheaterFacade holds those subsystems and knows the correct sequence.
        3. WatchMovie prepares everything and starts playback.
        4. EndMovie reverses the setup so the room returns to a normal state.
        5. The client calls two clear operations instead of many small ones.

        FACADE VS ADAPTER
        Adapter makes an existing interface fit a different interface. Facade
        simplifies a group of interfaces into one convenient interface. Adapter is
        about compatibility; Facade is about usability.

        FACADE VS MEDIATOR
        Both can coordinate multiple objects. A facade is a one-way convenience
        layer used by clients. A mediator centralizes communication among
        colleagues that may talk back to it. A facade usually does not change how
        the subsystems communicate with each other.

        A GOOD RULE OF THUMB
        If a common task always requires the same six-step ritual, give the ritual
        a name and put it behind a facade.
        """;

    public void Run()
    {
        Console.WriteLine("Preparing the room for movie night...\n");

        // The client creates the subsystems once, then hands them to the facade.
        // In a larger app, a dependency injection container could do this wiring.
        var facade = new HomeTheaterFacade(
            new RoomLights(),
            new Projector(),
            new SoundSystem(),
            new StreamingPlayer());

        facade.WatchMovie("The Design Pattern Adventure");

        Console.WriteLine();
        Console.WriteLine("The movie has ended.");
        facade.EndMovie();

        Console.WriteLine("\nThe client used two meaningful operations instead of ten low-level calls.");
    }

    // FACADE
    private sealed class HomeTheaterFacade
    {
        private readonly RoomLights _lights;
        private readonly Projector _projector;
        private readonly SoundSystem _sound;
        private readonly StreamingPlayer _player;

        public HomeTheaterFacade(
            RoomLights lights,
            Projector projector,
            SoundSystem sound,
            StreamingPlayer player)
        {
            _lights = lights;
            _projector = projector;
            _sound = sound;
            _player = player;
        }

        /// <summary>
        /// One operation that expresses the user's goal. The ordering rules live
        /// here instead of being repeated in every UI event handler.
        /// </summary>
        public void WatchMovie(string title)
        {
            Console.WriteLine("Facade: starting the movie-night sequence.");
            _lights.Dim(15);
            _projector.TurnOn();
            _projector.SetInput("Streaming player");
            _projector.SetWideScreen();
            _sound.TurnOn();
            _sound.SetSurroundMode();
            _sound.SetVolume(18);
            _player.Start(title);
            Console.WriteLine("Facade: room is ready.");
        }

        public void EndMovie()
        {
            Console.WriteLine("Facade: shutting everything down.");
            _player.Stop();
            _sound.SetVolume(5);
            _sound.TurnOff();
            _projector.TurnOff();
            _lights.TurnOn();
            Console.WriteLine("Facade: room returned to normal.");
        }
    }

    // SUBSYSTEM 1
    private sealed class RoomLights
    {
        public void Dim(int percent) =>
            Console.WriteLine($"Lights: dimmed to {percent}%.");

        public void TurnOn() => Console.WriteLine("Lights: restored to full brightness.");
    }

    // SUBSYSTEM 2
    private sealed class Projector
    {
        public void TurnOn() => Console.WriteLine("Projector: warming up the lamp.");

        public void SetInput(string input) =>
            Console.WriteLine($"Projector: input set to {input}.");

        public void SetWideScreen() => Console.WriteLine("Projector: wide-screen mode enabled.");

        public void TurnOff() => Console.WriteLine("Projector: lamp cooling down.");
    }

    // SUBSYSTEM 3
    private sealed class SoundSystem
    {
        public void TurnOn() => Console.WriteLine("Sound: receiver powered on.");

        public void SetSurroundMode() => Console.WriteLine("Sound: surround mode selected.");

        public void SetVolume(int level) => Console.WriteLine($"Sound: volume set to {level}.");

        public void TurnOff() => Console.WriteLine("Sound: receiver powered off.");
    }

    // SUBSYSTEM 4
    private sealed class StreamingPlayer
    {
        public void Start(string title) => Console.WriteLine($"Player: now playing '{title}'.");

        public void Stop() => Console.WriteLine("Player: playback stopped.");
    }
}
