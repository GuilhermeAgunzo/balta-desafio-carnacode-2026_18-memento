namespace Memento.Snapshots;

internal sealed record ImageMemento(
    byte[] Pixels,
    int Width,
    int Height,
    int Brightness,
    int Contrast,
    int Saturation,
    string FilterApplied,
    double Rotation,
    string Description
) : IMemento
{
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}
