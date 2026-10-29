using GoFPatternGuide.Core;

namespace GoFPatternGuide.Behavioral;

/// <summary>
/// OBSERVER
///
/// Intent: define a one-to-many dependency so that when one object changes
/// state, all of its subscribers are notified automatically.
///
/// The demo uses a weather station as the subject and two displays as observers.
/// The station does not know what the displays do with the reading.
/// </summary>
public sealed class ObserverPattern : IPatternDemo
{
    public string Name => "Observer";

    public string Category => PatternCategories.Behavioral;

    public string Summary =>
        "Notifies a list of subscribers automatically when an object's state changes.";

    public string Explanation => """
        WHAT IT IS
        Observer is a behavioral pattern that defines a subscription relationship
        between objects. One object, called the subject or publisher, maintains a
        list of observers. When the subject changes, it notifies every registered
        observer.

        The subject knows only the observer interface. It does not know whether an
        observer writes to a screen, sends an email, updates a database, or records
        a metric.

        WHY IT EXISTS
        Many parts of an application need to react when something changes: a user
        updates a profile, a sensor records a new reading, an order changes status,
        or a document is edited. If the subject calls each concrete listener
        directly, it becomes tightly coupled to every consumer.

        Adding a new reaction means editing the subject. Removing one means editing
        it again. The subject grows into a list of special cases.

        Observer reverses the dependency. Consumers subscribe to the subject. The
        subject publishes a change through one stable interface.

        THE PROBLEM IT SOLVES
        Imagine a weather station that directly calls PhoneDisplay and
        HeatIndexAlert. A new web dashboard requires changing the station. A test
        that wants to observe readings must deal with all the concrete displays.

        With Observer, the station keeps a list of IWeatherObserver objects.
        Subscribers can be added or removed at runtime. A test can subscribe a small
        recorder and inspect what it received.

        WHEN TO USE IT
        - One change should trigger reactions in an unknown number of objects.
        - Publishers and subscribers should be loosely coupled.
        - Subscribers need to join or leave at runtime.
        - You are implementing events, notifications, or a publish/subscribe system.
        - Multiple views must stay synchronized with one model.

        ADVANTAGES
        - Loose coupling between publisher and subscribers.
        - New observers can be added without changing the subject.
        - Subscribers can be attached and detached at runtime.
        - Supports broadcast communication naturally.
        - Makes it easy to test notifications with a fake observer.

        DISADVANTAGES
        - Notification order is often unspecified.
        - A slow or failing observer can delay or break the notification loop.
        - Memory leaks are possible if subscribers are never removed.
        - Cascading updates can make the control flow hard to follow.
        - Excessive events can create hidden dependencies and surprising behavior.
        - The subject may send more data than every observer needs.

        REAL-WORLD EXAMPLES
        - .NET events and event handlers.
        - UI data binding: a model change updates several views.
        - Newsletters and social-media followers.
        - Stock-price tickers and trading dashboards.
        - Sensor monitoring and alerting systems.
        - Message brokers and publish/subscribe systems.
        - Cache invalidation when a record changes.

        HOW THE DEMO WORKS
        1. WeatherReading is an immutable record containing the current values.
        2. IWeatherObserver is the observer interface with one Update method.
        3. WeatherStation is the subject. It keeps a list of observers and exposes
           Subscribe and Unsubscribe.
        4. PhoneDisplay and HeatIndexAlert are concrete observers with different
           reactions to the same reading.
        5. The demo subscribes both, publishes a comfortable reading, publishes a
           hot reading, removes the alert, and publishes again.

        PUSH VS PULL
        In a push model, the subject sends the changed data to observers, as this
        demo does. In a pull model, the subject sends only a notification and each
        observer asks the subject for the details it needs. Push is simpler; pull
        can reduce unnecessary data transfer for observers that need only a little.

        .NET EVENTS
        C# events are a language-level implementation of Observer. The event
        publisher exposes an event, and subscribers use += and -= to attach and
        detach handlers. For simple in-process notifications, events or
        IObservable<T> are often more idiomatic than writing the pattern by hand.

        A GOOD RULE OF THUMB
        Use Observer when a change should be announced to an open-ended audience,
        and always decide clearly who is responsible for unsubscribing.
        """;

    public void Run()
    {
        Console.WriteLine("Subscribing two displays to a weather station...\n");

        var station = new WeatherStation();
        var phone = new PhoneDisplay("Kitchen phone");
        var alert = new HeatIndexAlert(30.0);

        station.Subscribe(phone);
        station.Subscribe(alert);

        station.SetReading(new WeatherReading(24.0, 55));

        Console.WriteLine();
        station.SetReading(new WeatherReading(32.0, 60));

        Console.WriteLine("\nUnsubscribing the heat alert...");
        station.Unsubscribe(alert);

        Console.WriteLine();
        station.SetReading(new WeatherReading(34.0, 62));

        Console.WriteLine("\nThe station never referenced a concrete display type.");
    }

    // IMMUTABLE NOTIFICATION DATA
    private sealed record WeatherReading(double TemperatureC, double HumidityPercent);

    // OBSERVER
    private interface IWeatherObserver
    {
        void Update(WeatherReading reading);
    }

    // SUBJECT
    private sealed class WeatherStation
    {
        private readonly List<IWeatherObserver> _observers = [];

        public void Subscribe(IWeatherObserver observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }

        public void Unsubscribe(IWeatherObserver observer) =>
            _observers.Remove(observer);

        public void SetReading(WeatherReading reading)
        {
            Console.WriteLine(
                $"Station: {reading.TemperatureC:F1} C, " +
                $"{reading.HumidityPercent}% humidity.");

            // Iterate over a snapshot so an observer may unsubscribe while being
            // notified without invalidating the current loop.
            foreach (var observer in _observers.ToArray())
            {
                observer.Update(reading);
            }
        }
    }

    // CONCRETE OBSERVER 1
    private sealed class PhoneDisplay : IWeatherObserver
    {
        private readonly string _name;

        public PhoneDisplay(string name)
        {
            _name = name;
        }

        public void Update(WeatherReading reading)
        {
            Console.WriteLine(
                $"  [{_name}] {reading.TemperatureC:F1} C and " +
                $"{reading.HumidityPercent}% humidity.");
        }
    }

    // CONCRETE OBSERVER 2
    private sealed class HeatIndexAlert : IWeatherObserver
    {
        private readonly double _threshold;

        public HeatIndexAlert(double threshold)
        {
            _threshold = threshold;
        }

        public void Update(WeatherReading reading)
        {
            if (reading.TemperatureC >= _threshold)
            {
                Console.WriteLine(
                    $"  [Heat alert] It is {reading.TemperatureC:F1} C. Drink water!");
            }
        }
    }
}
