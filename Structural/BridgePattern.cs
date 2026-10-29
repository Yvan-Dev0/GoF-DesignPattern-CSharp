using GoFPatternGuide.Core;

namespace GoFPatternGuide.Structural;

/// <summary>
/// BRIDGE
///
/// Intent: separate an abstraction from its implementation so both can vary
/// independently.
///
/// The demo separates a remote control (abstraction) from devices such as a TV
/// and a radio (implementation). A remote can work with any device, and a new
/// device can work with any remote.
/// </summary>
public sealed class BridgePattern : IPatternDemo
{
    public string Name => "Bridge";

    public string Category => PatternCategories.Structural;

    public string Summary =>
        "Separates an abstraction from its implementation so each can grow independently.";

    public string Explanation => """
        WHAT IT IS
        Bridge is a structural pattern that splits a large class hierarchy into two
        dimensions:
        - an abstraction, which represents a high-level concept;
        - an implementation, which supplies the low-level details.

        The abstraction holds a reference to an implementation and delegates work
        to it. The two sides communicate through an interface, so each can change
        without forcing a change in the other.

        WHY IT EXISTS
        Some concepts vary in more than one direction. A remote control can be
        basic or advanced. It can also control a TV or a radio. If we use
        inheritance for every combination, the hierarchy grows quickly:

            BasicTvRemote
            AdvancedTvRemote
            BasicRadioRemote
            AdvancedRadioRemote

        Add a streaming box and a game console, and the number of classes explodes.
        Bridge replaces that explosion with two small hierarchies: remotes and
        devices.

        THE PROBLEM IT SOLVES
        Inheritance is a strong relationship. It works best when the child truly is
        a specialized version of the parent. A remote is not a kind of TV, and a TV
        is not a kind of remote. Combining them through inheritance creates rigid,
        duplicated classes.

        Bridge uses composition instead: a remote has a device. A BasicRemote can
        control any IDevice, and a new SmartProjector can be controlled by any
        remote without creating a new remote subclass.

        WHEN TO USE IT
        - A class has two or more independent dimensions of variation.
        - You want to avoid a combinatorial explosion of subclasses.
        - Both the abstraction and implementation should be extensible.
        - Implementation details should not be exposed to client code.
        - You want to swap implementations at runtime.

        ADVANTAGES
        - Keeps abstraction and implementation independent.
        - Reduces the number of subclasses.
        - Allows implementations to change at runtime.
        - Hides low-level details behind a stable interface.
        - Makes both sides easier to test separately.

        DISADVANTAGES
        - Adds indirection and more types for a simple situation.
        - The abstraction must be designed around the capabilities the
          implementation interface can provide.
        - It can be confused with Strategy, because both use composition and an
          interface.
        - If there is only one dimension of variation, inheritance or a simple
          interface may be enough.

        REAL-WORLD EXAMPLES
        - Remote controls and devices.
        - Shapes and rendering APIs (vector, raster, SVG).
        - Cross-platform windows and platform-specific window implementations.
        - Payment checkout flows and payment providers.
        - Database repositories and database drivers.
        - Notification types and delivery channels.

        HOW THE DEMO WORKS
        1. IDevice is the implementation interface. It defines power and volume
           operations that every device supports.
        2. Tv and Radio are concrete implementations.
        3. RemoteControl is the abstraction. It holds an IDevice and implements
           common remote behavior.
        4. AdvancedRemote extends the abstraction with Mute.
        5. The demo pairs a basic remote with a TV and an advanced remote with a
           radio, showing that either side can be combined freely.

        BRIDGE VS ADAPTER
        Adapter is usually introduced after the fact to make two existing
        interfaces compatible. Bridge is designed up front to let two dimensions
        vary independently. Adapter changes an interface; Bridge separates an
        abstraction from its implementation.

        BRIDGE VS STRATEGY
        Both delegate to an interface. Strategy is a behavioral pattern focused on
        interchangeable algorithms. Bridge is a structural pattern focused on
        separating a hierarchy from its implementation hierarchy.

        A GOOD RULE OF THUMB
        If you find yourself writing class names that combine two independent
        dimensions, consider a bridge instead of another inheritance branch.
        """;

    public void Run()
    {
        Console.WriteLine("Pairing remotes with devices in different combinations...\n");

        // Dimension 1: remote type. Dimension 2: device type. They are combined
        // through composition rather than a class for every pair.
        var tvRemote = new RemoteControl(new Tv());
        tvRemote.TogglePower();
        tvRemote.VolumeUp();
        tvRemote.VolumeUp();
        tvRemote.ShowStatus();

        Console.WriteLine();

        var radioRemote = new AdvancedRemote(new Radio());
        radioRemote.TogglePower();
        radioRemote.VolumeUp();
        radioRemote.Mute();
        radioRemote.ShowStatus();

        Console.WriteLine("\nNo BasicTvRemote or AdvancedRadioRemote subclasses were needed.");
    }

    // IMPLEMENTATION INTERFACE
    // The low-level operations that every device must provide.
    private interface IDevice
    {
        string Name { get; }

        bool IsOn { get; }

        int Volume { get; }

        void TurnOn();

        void TurnOff();

        void SetVolume(int level);
    }

    // CONCRETE IMPLEMENTATION 1
    private sealed class Tv : IDevice
    {
        public string Name => "Living room TV";

        public bool IsOn { get; private set; }

        public int Volume { get; private set; } = 10;

        public void TurnOn()
        {
            IsOn = true;
            Console.WriteLine($"{Name}: screen on.");
        }

        public void TurnOff()
        {
            IsOn = false;
            Console.WriteLine($"{Name}: screen off.");
        }

        public void SetVolume(int level)
        {
            Volume = Math.Clamp(level, 0, 30);
            Console.WriteLine($"{Name}: volume set to {Volume}.");
        }
    }

    // CONCRETE IMPLEMENTATION 2
    private sealed class Radio : IDevice
    {
        public string Name => "Kitchen radio";

        public bool IsOn { get; private set; }

        public int Volume { get; private set; } = 5;

        public void TurnOn()
        {
            IsOn = true;
            Console.WriteLine($"{Name}: tuning in.");
        }

        public void TurnOff()
        {
            IsOn = false;
            Console.WriteLine($"{Name}: radio off.");
        }

        public void SetVolume(int level)
        {
            Volume = Math.Clamp(level, 0, 20);
            Console.WriteLine($"{Name}: volume set to {Volume}.");
        }
    }

    // ABSTRACTION
    // High-level remote behavior. It delegates device-specific work to IDevice.
    private class RemoteControl
    {
        protected readonly IDevice Device;

        public RemoteControl(IDevice device)
        {
            Device = device;
        }

        public void TogglePower()
        {
            if (Device.IsOn)
            {
                Device.TurnOff();
            }
            else
            {
                Device.TurnOn();
            }
        }

        public void VolumeUp() => Device.SetVolume(Device.Volume + 1);

        public void VolumeDown() => Device.SetVolume(Device.Volume - 1);

        public virtual void ShowStatus()
        {
            Console.WriteLine(
                $"Status: {Device.Name} is {(Device.IsOn ? "on" : "off")} " +
                $"at volume {Device.Volume}.");
        }
    }

    // REFINED ABSTRACTION
    // Adds a remote-only capability without touching any device class.
    private sealed class AdvancedRemote : RemoteControl
    {
        public AdvancedRemote(IDevice device)
            : base(device)
        {
        }

        public void Mute()
        {
            Device.SetVolume(0);
            Console.WriteLine("Advanced remote: mute pressed.");
        }
    }
}
