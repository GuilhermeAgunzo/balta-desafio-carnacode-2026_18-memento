namespace Memento.Snapshots;

public interface IMemento
{
    DateTime CreatedAt { get; }
    string Description { get; }
}
