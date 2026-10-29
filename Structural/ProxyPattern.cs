using GoFPatternGuide.Core;

namespace GoFPatternGuide.Structural;

/// <summary>
/// PROXY
///
/// Intent: provide a substitute object that controls access to another object.
///
/// The demo uses a lazy-loading video proxy. The expensive real video is created
/// only on the first Play call, and later calls reuse it.
/// </summary>
public sealed class ProxyPattern : IPatternDemo
{
    public string Name => "Proxy";

    public string Category => PatternCategories.Structural;

    public string Summary =>
        "Stands in for another object and controls when or how it is accessed.";

    public string Explanation => """
        WHAT IT IS
        Proxy is a structural pattern that provides a stand-in object for another
        object. The proxy implements the same interface as the real object, so the
        client cannot tell the difference. It intercepts calls and decides whether,
        when, and how to forward them.

        Common proxy types include:
        - virtual proxy: delays creating an expensive object until it is needed;
        - protection proxy: checks permissions before forwarding a call;
        - remote proxy: represents an object in another process or on another machine;
        - caching proxy: stores results to avoid repeated work;
        - logging proxy: records access without changing the real object.

        WHY IT EXISTS
        Some objects are expensive to create, located far away, or require access
        checks. The client should not need to know those details. A proxy keeps the
        access policy in one place while preserving the original interface.

        This is useful when the real object must remain untouched or when the
        decision to access it belongs to infrastructure rather than business logic.

        THE PROBLEM IT SOLVES
        Imagine a video service that loads a large file as soon as a video object is
        constructed. A page may list hundreds of videos, but the user opens only
        one. Eagerly loading every video wastes bandwidth and memory.

        A virtual proxy can wait until Play is called. It can also cache the real
        object so repeated calls reuse it. The page still talks to IVideo and does
        not need a special loading path.

        WHEN TO USE IT
        - Object creation is expensive and should be delayed.
        - Access must be checked before calling the real object.
        - The real object is remote or slow.
        - Results should be cached.
        - You need to add logging, metrics, or auditing without editing the real
          object.
        - The client should remain unaware of these access concerns.

        ADVANTAGES
        - Adds control without changing the real object.
        - Supports lazy initialization and caching.
        - Can enforce security or usage policies.
        - Keeps cross-cutting access concerns separate from business logic.
        - Presents the same interface, so client code stays simple.

        DISADVANTAGES
        - Adds an extra object and an extra call on every operation.
        - Lazy loading can move latency to a surprising moment.
        - Cache invalidation and permission rules can become complex.
        - The client may not realize it is using a substitute.
        - It can hide failures or performance problems if logging is poor.
        - It is easy to confuse with Decorator because both wrap an interface.

        REAL-WORLD EXAMPLES
        - Lazy-loaded images or videos in a web application.
        - A remote service client that looks like a local object.
        - A permission-checking wrapper around a file or document service.
        - An API client that caches expensive responses.
        - An ORM proxy that loads related data on demand.
        - A logging or metrics wrapper around a repository.

        HOW THE DEMO WORKS
        1. IVideo is the common interface.
        2. RealVideo represents an expensive resource. Its constructor announces
           that it is loading.
        3. VideoProxy implements IVideo, holds the title, and creates RealVideo
           only on the first Play call.
        4. The proxy counts requests and reuses the real video on later calls.
        5. The demo creates two proxies. The second is never played, so its real
           video is never loaded.

        PROXY VS DECORATOR
        Both implement the same interface and wrap another object. A proxy controls
        access and often creates or locates the real object itself. A decorator is
        given an existing object and adds responsibilities. The distinction is
        intent, not just structure.

        PROXY VS ADAPTER
        Adapter changes the interface to make two incompatible APIs work together.
        Proxy keeps the same interface and controls access to it.

        A GOOD RULE OF THUMB
        If the question is "should this call reach the real object, and when?",
        think Proxy. If the question is "what extra behavior should happen around
        this call?", think Decorator.
        """;

    public void Run()
    {
        Console.WriteLine("Browsing a video library...\n");

        var firstVideo = new VideoProxy("Design Patterns in 12 Minutes");
        var secondVideo = new VideoProxy("Refactoring a Tiny App");

        // The client can use either proxy through the shared IVideo interface.
        IVideo player = firstVideo;

        Console.WriteLine($"Proxy created for '{firstVideo.Title}'.");
        Console.WriteLine($"Proxy created for '{secondVideo.Title}'.");
        Console.WriteLine("No heavy video has been loaded yet.\n");

        player.Play();
        Console.WriteLine();
        firstVideo.Play();

        Console.WriteLine("\nThe second video was never played, so it was never loaded.");
    }

    // SUBJECT
    private interface IVideo
    {
        void Play();
    }

    // REAL SUBJECT
    // Pretend this constructor is expensive: it might download or decode a file.
    private sealed class RealVideo : IVideo
    {
        private readonly string _title;

        public RealVideo(string title)
        {
            _title = title;
            Console.WriteLine($"  [RealVideo] Loading and buffering '{title}'...");
        }

        public void Play()
        {
            Console.WriteLine($"  [RealVideo] Playing '{_title}'.");
        }
    }

    // PROXY
    // It controls access to RealVideo and creates it only when needed.
    private sealed class VideoProxy : IVideo
    {
        private RealVideo? _realVideo;
        private int _playRequests;

        public VideoProxy(string title)
        {
            Title = title;
        }

        public string Title { get; }

        public void Play()
        {
            _playRequests++;
            Console.WriteLine(
                $"  [Proxy] Play request #{_playRequests} for '{Title}'.");

            // Lazy initialization: the expensive object is constructed on first
            // use, not when the proxy is created.
            _realVideo ??= new RealVideo(Title);
            _realVideo.Play();
        }
    }
}
