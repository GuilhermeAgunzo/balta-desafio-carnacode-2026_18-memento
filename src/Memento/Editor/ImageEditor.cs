using Memento.Snapshots;

namespace Memento.Editor;

public class ImageEditor
{
    private byte[] _pixels;
    private int _width;
    private int _height;
    private int _brightness;
    private int _contrast;
    private int _saturation;
    private string _filterApplied;
    private double _rotation;

    public ImageEditor(int width, int height)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);

        _width = width;
        _height = height;
        _pixels = new byte[width * height * 3];
        _brightness = 0;
        _contrast = 0;
        _saturation = 0;
        _filterApplied = "None";
        _rotation = 0;

        Console.WriteLine($"[Editor] Imagem criada: {width}x{height}");
    }

    public void ApplyBrightness(int value)
    {
        _brightness += value;
        Console.WriteLine($"[Editor] Brilho ajustado para {_brightness}");
    }

    public void ApplyFilter(string filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        _filterApplied = filter;
        Console.WriteLine($"[Editor] Filtro aplicado: {filter}");
    }

    public void Rotate(double degrees)
    {
        _rotation += degrees;
        Console.WriteLine($"[Editor] Rotação: {_rotation}°");
    }

    public void Crop(int newWidth, int newHeight)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(newWidth);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(newHeight);

        _width = newWidth;
        _height = newHeight;
        Array.Resize(ref _pixels, newWidth * newHeight * 3);
        Console.WriteLine($"[Editor] Imagem cortada para {newWidth}x{newHeight}");
    }

    /// <summary>
    /// Creates a memento capturing the current state of the editor.
    /// </summary>
    public IMemento CreateMemento(string description)
    {
        return new ImageMemento(
            Pixels: (byte[])_pixels.Clone(),
            Width: _width,
            Height: _height,
            Brightness: _brightness,
            Contrast: _contrast,
            Saturation: _saturation,
            FilterApplied: _filterApplied,
            Rotation: _rotation,
            Description: description
        );
    }

    /// <summary>
    /// Restores the editor state from a memento.
    /// </summary>
    public void Restore(IMemento memento)
    {
        ArgumentNullException.ThrowIfNull(memento);

        if (memento is not ImageMemento snapshot)
            throw new ArgumentException("Invalid memento type.", nameof(memento));

        _pixels = (byte[])snapshot.Pixels.Clone();
        _width = snapshot.Width;
        _height = snapshot.Height;
        _brightness = snapshot.Brightness;
        _contrast = snapshot.Contrast;
        _saturation = snapshot.Saturation;
        _filterApplied = snapshot.FilterApplied;
        _rotation = snapshot.Rotation;

        Console.WriteLine($"[Editor] Estado restaurado: {snapshot.Description}");
    }

    public void DisplayInfo()
    {
        Console.WriteLine($"\n=== Estado Atual ===");
        Console.WriteLine($"Dimensões: {_width}x{_height}");
        Console.WriteLine($"Brilho: {_brightness}");
        Console.WriteLine($"Filtro: {_filterApplied}");
        Console.WriteLine($"Rotação: {_rotation}°\n");
    }
}
