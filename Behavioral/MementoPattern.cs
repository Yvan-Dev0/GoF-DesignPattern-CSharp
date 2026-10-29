using GoFPatternGuide.Core;

namespace GoFPatternGuide.Behavioral;

/// <summary>
/// MEMENTO
///
/// Intent: capture an object's internal state so it can be restored later,
/// without exposing that state to the outside world.
///
/// The demo implements a tiny text editor with undo. The editor creates opaque
/// snapshots, and a separate history object stores them.
/// </summary>
public sealed class MementoPattern : IPatternDemo
{
    public string Name => "Memento";

    public string Category => PatternCategories.Behavioral;

    public string Summary =>
        "Saves and restores an object's state without exposing its implementation details.";

    public string Explanation => """
        WHAT IT IS
        Memento is a behavioral pattern that captures an object's internal state in
        a separate object so the state can be restored later. The saved object is
        called a memento.

        Three roles are involved:
        - the originator, whose state is saved and restored;
        - the memento, a snapshot of that state;
        - the caretaker, which stores mementos but does not inspect or modify them.

        WHY IT EXISTS
        Undo is a familiar feature, but implementing it safely is harder than it
        looks. If other code can read or modify the editor's internal state, the
        snapshot may be corrupted. If the editor exposes every field publicly just
        to support undo, the design becomes fragile.

        Memento lets the originator create a snapshot through its own methods. The
        caretaker holds the snapshot but does not need to understand its contents.

        THE PROBLEM IT SOLVES
        Imagine implementing undo by copying every editor field into a public
        object. Adding a field means updating the copy logic. External code might
        accidentally alter the snapshot. The editor's private implementation leaks
        into the rest of the program.

        With Memento, EditorSnapshot is created by TextEditor.Save and only
        TextEditor.Restore consumes it. The history object simply keeps a stack of
        snapshots.

        WHEN TO USE IT
        - You need undo, redo, or rollback.
        - An object's state must be saved without exposing its internals.
        - You need checkpoints before a risky operation.
        - A workflow must return to a previous valid state.
        - You want to restore an object without adding a public setter for every field.

        ADVANTAGES
        - Supports undo and rollback cleanly.
        - Protects the originator's encapsulation.
        - Keeps state-history management separate from business logic.
        - Makes snapshots explicit and easy to reason about.
        - Can support multiple saved checkpoints.

        DISADVANTAGES
        - Snapshots can consume a lot of memory if state is large or history is long.
        - Copying deep object graphs can be expensive.
        - The caretaker may need a policy to limit or discard old snapshots.
        - Restoring state can have side effects that are not captured in the memento.
        - Too many snapshots can make a program's behavior harder to debug.

        REAL-WORLD EXAMPLES
        - Undo/redo in text editors and drawing tools.
        - Save points in games.
        - Database transactions and rollback.
        - Form wizards that preserve a previous step.
        - Version history in document systems.
        - Configuration checkpoints before applying a risky change.

        HOW THE DEMO WORKS
        1. TextEditor is the originator. It owns the content and can create or
           restore a snapshot.
        2. EditorSnapshot is the memento. It is a small immutable record.
        3. History is the caretaker. It stores snapshots in a stack and never
           inspects their contents.
        4. The demo types some text, saves, types more, saves, types more, then
           undoes twice.

        SHALLOW VS DEEP SNAPSHOTS
        This editor stores a string, which is immutable, so a simple record is
        enough. If the editor held a mutable list of objects, the snapshot would
        need a deep copy; otherwise the saved state and the live state would share
        the same list.

        MEMENTO VS COMMAND UNDO
        Command can implement undo by storing the information needed to reverse an
        action. Memento stores a complete state snapshot. Command is often more
        memory-efficient for small reversible actions; Memento is simpler when the
        whole state is small or when reversal is difficult. The two patterns can be
        combined: a command can hold a memento to restore before the action.

        A GOOD RULE OF THUMB
        When you need a reliable "restore to exactly here" point, let the object
        create its own snapshot and let a caretaker store it.
        """;

    public void Run()
    {
        Console.WriteLine("Typing into an editor with undo history...\n");

        var editor = new TextEditor();
        var history = new History();

        editor.Type("Design patterns ");
        history.Save(editor);

        editor.Type("are reusable solutions ");
        history.Save(editor);

        editor.Type("to common software problems.");

        Console.WriteLine($"\nCurrent: \"{editor.Content}\"");
        Console.WriteLine($"History size: {history.Count}");

        Console.WriteLine("\nUndo once:");
        history.Undo(editor);
        Console.WriteLine($"Current: \"{editor.Content}\"");

        Console.WriteLine("\nUndo twice:");
        history.Undo(editor);
        Console.WriteLine($"Current: \"{editor.Content}\"");

        Console.WriteLine("\nUndo with no snapshots left:");
        history.Undo(editor);

        Console.WriteLine("\nThe history never read or changed the editor's internal state.");
    }

    // MEMENTO
    // Immutable snapshot. Only TextEditor creates it, and the caretaker only
    // stores and returns it.
    private sealed record EditorSnapshot(string Content);

    // ORIGINATOR
    private sealed class TextEditor
    {
        public string Content { get; private set; } = string.Empty;

        public void Type(string text)
        {
            Content += text;
            Console.WriteLine($"Typing: \"{text}\"");
        }

        public EditorSnapshot Save() => new(Content);

        public void Restore(EditorSnapshot snapshot)
        {
            Content = snapshot.Content;
        }
    }

    // CARETAKER
    // It manages the undo stack but does not inspect snapshot contents.
    private sealed class History
    {
        private readonly Stack<EditorSnapshot> _snapshots = new();

        public int Count => _snapshots.Count;

        public void Save(TextEditor editor)
        {
            _snapshots.Push(editor.Save());
            Console.WriteLine($"  Snapshot saved. History size: {_snapshots.Count}");
        }

        public void Undo(TextEditor editor)
        {
            if (_snapshots.Count == 0)
            {
                Console.WriteLine("  Nothing to undo.");
                return;
            }

            editor.Restore(_snapshots.Pop());
            Console.WriteLine($"  Restored snapshot. History size: {_snapshots.Count}");
        }
    }
}
