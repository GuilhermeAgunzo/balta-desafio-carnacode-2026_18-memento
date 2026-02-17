using Memento.Editor;
using Memento.Snapshots;

namespace Memento.History;

public class EditorHistory
{
    private readonly ImageEditor _editor;
    private readonly LinkedList<IMemento> _undoStack = new();
    private readonly Stack<IMemento> _redoStack = new();
    private readonly int _maxSnapshots;

    public EditorHistory(ImageEditor editor, int maxSnapshots = 50)
    {
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxSnapshots);

        _editor = editor;
        _maxSnapshots = maxSnapshots;
    }

    public int UndoCount => Math.Max(0, _undoStack.Count - 1);
    public int RedoCount => _redoStack.Count;

    /// <summary>
    /// Saves the current editor state as a snapshot. Clears redo history.
    /// </summary>
    public void Backup(string description)
    {
        _undoStack.AddLast(_editor.CreateMemento(description));
        _redoStack.Clear();

        while (_undoStack.Count > _maxSnapshots)
            _undoStack.RemoveFirst();

        Console.WriteLine($"[Histórico] Backup: {description} (Undo: {UndoCount}, Redo: {RedoCount})");
    }

    /// <summary>
    /// Restores the editor to the previous snapshot.
    /// </summary>
    public void Undo()
    {
        if (_undoStack.Count <= 1)
        {
            Console.WriteLine("[Histórico] Nada para desfazer");
            return;
        }

        var current = _undoStack.Last!.Value;
        _undoStack.RemoveLast();
        _redoStack.Push(current);

        var previous = _undoStack.Last!.Value;
        _editor.Restore(previous);

        Console.WriteLine($"[Histórico] Undo (Undo: {UndoCount}, Redo: {RedoCount})");
    }

    /// <summary>
    /// Re-applies the last undone snapshot.
    /// </summary>
    public void Redo()
    {
        if (_redoStack.Count == 0)
        {
            Console.WriteLine("[Histórico] Nada para refazer");
            return;
        }

        var memento = _redoStack.Pop();
        _undoStack.AddLast(memento);
        _editor.Restore(memento);

        Console.WriteLine($"[Histórico] Redo (Undo: {UndoCount}, Redo: {RedoCount})");
    }
}
