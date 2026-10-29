using GoFPatternGuide.Core;

namespace GoFPatternGuide.Behavioral;

/// <summary>
/// ITERATOR
///
/// Intent: access the elements of a collection sequentially without exposing the
/// collection's internal representation.
///
/// The demo defines a small iterator for a playlist. The client walks the songs
/// through HasNext and Next instead of manipulating the playlist's List directly.
/// </summary>
public sealed class IteratorPattern : IPatternDemo
{
    public string Name => "Iterator";

    public string Category => PatternCategories.Behavioral;

    public string Summary =>
        "Traverses a collection step by step without exposing how the collection stores its items.";

    public string Explanation => """
        WHAT IT IS
        Iterator is a behavioral pattern that provides a way to access the
        elements of a collection one at a time without revealing the collection's
        internal structure.

        The collection creates an iterator object. The iterator remembers the
        current position and exposes operations such as HasNext and Next. Different
        collections can provide different iterators while client code follows the
        same traversal style.

        WHY IT EXISTS
        Collections come in many shapes: arrays, linked lists, trees, database
        cursors, and generated sequences. If every part of a program loops over the
        internal storage directly, the program becomes coupled to that storage.
        Changing the collection can break every loop.

        Iterator separates traversal from storage. The client asks, "Is there
        another item?" and "Give me the next one." It does not need to know whether
        the data lives in an array, a list, or a tree.

        THE PROBLEM IT SOLVES
        Suppose a music app exposes its songs through a public List. Client code
        writes for loops against that list. Later the app wants to load songs
        lazily from a database or traverse a playlist in a different order. Every
        client loop must change.

        With Iterator, Playlist can change how it stores or generates songs as long
        as it returns an object that follows the iterator contract.

        WHEN TO USE IT
        - A collection's internal structure should stay hidden.
        - Several traversal orders are needed over the same collection.
        - Client code should be able to treat different collections uniformly.
        - Traversal state should be separate from the collection itself.
        - You want lazy or on-demand generation of sequence items.

        ADVANTAGES
        - Hides internal collection details.
        - Supports multiple simultaneous traversals.
        - Allows different traversal algorithms over the same data.
        - Gives collections a common traversal interface.
        - Keeps loop code simple and readable.
        - Can produce items lazily instead of materializing everything at once.

        DISADVANTAGES
        - Adds interfaces and iterator classes for simple collections.
        - In C#, the built-in IEnumerable and IEnumerator already implement the
          pattern, so writing a custom iterator is often unnecessary.
        - Modifying a collection during traversal can invalidate the iterator.
        - Custom iterators can be harder to debug than a direct loop.
        - Over-engineering traversal for a tiny private array is usually wasteful.

        REAL-WORLD EXAMPLES
        - foreach loops over arrays, lists, dictionaries, and sets in C#.
        - Database cursors that fetch rows one at a time.
        - File-system walkers that yield files as they are discovered.
        - Playlists, photo albums, and paginated API results.
        - Tree traversals such as depth-first or breadth-first search.
        - LINQ queries that produce items lazily.

        HOW THE DEMO WORKS
        1. Song is a small record containing a title and artist.
        2. IIterator<Song> defines HasNext and Next.
        3. IAggregate<Song> defines GetIterator.
        4. Playlist implements IAggregate and creates a PlaylistIterator.
        5. PlaylistIterator keeps an index into the songs and returns one song at a
           time. The client never accesses the private List.
        6. A second iterator is created to show that each traversal has its own
           position and can run independently.

        THE MODERN C# VERSION
        C# already has a built-in iterator pattern:
        - IEnumerable<T> represents something that can be traversed;
        - IEnumerator<T> represents the traversal itself;
        - foreach calls GetEnumerator, MoveNext, and Current;
        - yield return makes it easy to write a lazy iterator.

        For production C#, prefer IEnumerable<T> and yield return unless you are
        teaching the pattern or need a custom traversal API. The demo implements
        the raw pattern so the moving parts are visible.

        A GOOD RULE OF THUMB
        Let the collection own its traversal details, and give callers a simple,
        consistent way to ask for the next item.
        """;

    public void Run()
    {
        var playlist = new Playlist();
        playlist.Add(new Song("A Design Pattern", "The Refactorers"));
        playlist.Add(new Song("Loose Coupling", "The Refactorers"));
        playlist.Add(new Song("Single Responsibility", "SOLID State"));

        Console.WriteLine("Walking the playlist with the custom iterator:\n");

        IPlaylistIterator iterator = playlist.GetIterator();
        var position = 1;

        while (iterator.HasNext())
        {
            Song song = iterator.Next();
            Console.WriteLine($"{position++}. {song.Title} - {song.Artist}");
        }

        Console.WriteLine("\nStarting a second iterator at the beginning...");

        // A new iterator has its own position. Two traversals can coexist without
        // interfering with each other.
        IPlaylistIterator second = playlist.GetIterator();
        Console.WriteLine($"First song of new traversal: {second.Next().Title}");

        Console.WriteLine("\nThe Playlist's internal List was never exposed.");
    }

    private sealed record Song(string Title, string Artist);

    // ITERATOR
    private interface IPlaylistIterator
    {
        bool HasNext();

        Song Next();
    }

    // AGGREGATE
    private interface IPlaylist
    {
        IPlaylistIterator GetIterator();
    }

    // CONCRETE AGGREGATE
    private sealed class Playlist : IPlaylist
    {
        private readonly List<Song> _songs = [];

        public void Add(Song song) => _songs.Add(song);

        public IPlaylistIterator GetIterator() => new PlaylistIterator(_songs);

        /// <summary>
        /// The iterator is a nested class so it can be private and focused on
        /// walking the playlist. It holds the traversal position.
        /// </summary>
        private sealed class PlaylistIterator : IPlaylistIterator
        {
            private readonly IReadOnlyList<Song> _songs;
            private int _index;

            public PlaylistIterator(IReadOnlyList<Song> songs)
            {
                _songs = songs;
            }

            public bool HasNext() => _index < _songs.Count;

            public Song Next()
            {
                if (!HasNext())
                {
                    throw new InvalidOperationException("The playlist has no more songs.");
                }

                return _songs[_index++];
            }
        }
    }
}
