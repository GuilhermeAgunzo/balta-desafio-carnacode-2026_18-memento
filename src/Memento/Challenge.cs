// DESAFIO: Editor de Imagens com Histórico de Edições
// PROBLEMA: Um editor de imagens precisa permitir desfazer/refazer operações (crop, filtros,
// rotação). O código atual salva o estado completo da imagem após cada edição, consumindo
// muita memória, ou não implementa undo de forma adequada violando encapsulamento

using System;
using System.Collections.Generic;

namespace DesignPatternChallenge
{
    // Contexto: Editor de imagens com operações que modificam estado complexo
    // Precisa permitir undo/redo sem expor detalhes internos
    
    public class ImageEditor
    {
        // Estado interno complexo
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
            _width = width;
            _height = height;
            _pixels = new byte[width * height * 3]; // RGB
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
            _width = newWidth;
            _height = newHeight;
            Array.Resize(ref _pixels, newWidth * newHeight * 3);
            Console.WriteLine($"[Editor] Imagem cortada para {newWidth}x{newHeight}");
        }

        // Problema: Métodos públicos que expõem estado interno para salvar
        public int GetWidth() => _width;
        public int GetHeight() => _height;
        public int GetBrightness() => _brightness;
        public int GetContrast() => _contrast;
        public string GetFilter() => _filterApplied;
        public double GetRotation() => _rotation;
        public byte[] GetPixels() => _pixels; // Expõe array interno!

        // Problema: Métodos públicos para restaurar estado
        public void SetWidth(int width) => _width = width;
        public void SetHeight(int height) => _height = height;
        public void SetBrightness(int brightness) => _brightness = brightness;
        public void SetContrast(int contrast) => _contrast = contrast;
        public void SetFilter(string filter) => _filterApplied = filter;
        public void SetRotation(double rotation) => _rotation = rotation;
        public void SetPixels(byte[] pixels) => _pixels = pixels;

        public void DisplayInfo()
        {
            Console.WriteLine($"\n=== Estado Atual ===");
            Console.WriteLine($"Dimensões: {_width}x{_height}");
            Console.WriteLine($"Brilho: {_brightness}");
            Console.WriteLine($"Filtro: {_filterApplied}");
            Console.WriteLine($"Rotação: {_rotation}°\n");
        }
    }

    // Tentativa 1: Salvar estado completo (ineficiente)
    public class ImageSnapshot
    {
        // Problema: Duplica todo o estado, mesmo campos não modificados
        public byte[] Pixels { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Brightness { get; set; }
        public int Contrast { get; set; }
        public int Saturation { get; set; }
        public string FilterApplied { get; set; }
        public double Rotation { get; set; }

        // Problema: Cálculo de memória
        // Imagem 1920x1080 = ~6MB de pixels
        // 10 snapshots = ~60MB só para histórico!
        public long GetMemoryUsage()
        {
            return Pixels.Length + 
                   sizeof(int) * 4 + 
                   sizeof(double) + 
                   (FilterApplied?.Length * 2 ?? 0);
        }
    }

    public class EditorWithFullSnapshots
    {
        private ImageEditor _editor;
        private Stack<ImageSnapshot> _history;

        public EditorWithFullSnapshots(ImageEditor editor)
        {
            _editor = editor;
            _history = new Stack<ImageSnapshot>();
        }

        public void SaveState()
        {
            // Problema: Salva TUDO, mesmo o que não mudou
            var snapshot = new ImageSnapshot
            {
                Pixels = _editor.GetPixels(),  // Copia array inteiro!
                Width = _editor.GetWidth(),
                Height = _editor.GetHeight(),
                Brightness = _editor.GetBrightness(),
                Contrast = _editor.GetContrast(),
                FilterApplied = _editor.GetFilter(),
                Rotation = _editor.GetRotation()
            };

            _history.Push(snapshot);
            Console.WriteLine($"[Histórico] Estado salvo (Snapshots: {_history.Count})");
            
            // Simulando uso de memória
            long totalMemory = 0;
            foreach (var snap in _history)
            {
                totalMemory += snap.GetMemoryUsage();
            }
            Console.WriteLine($"[Histórico] Memória usada: ~{totalMemory / (1024 * 1024)}MB");
        }

        public void Undo()
        {
            if (_history.Count > 0)
            {
                var snapshot = _history.Pop();
                
                // Problema: Restaura tudo, violando encapsulamento
                _editor.SetPixels(snapshot.Pixels);
                _editor.SetWidth(snapshot.Width);
                _editor.SetHeight(snapshot.Height);
                _editor.SetBrightness(snapshot.Brightness);
                _editor.SetContrast(snapshot.Contrast);
                _editor.SetFilter(snapshot.FilterApplied);
                _editor.SetRotation(snapshot.Rotation);

                Console.WriteLine($"[Histórico] Estado restaurado");
            }
        }
    }

    // Tentativa 2: Salvar apenas diferenças (complexo e frágil)
    public class ImageDelta
    {
        public string Operation { get; set; }
        public Dictionary<string, object> Changes { get; set; }

        // Problema: Código frágil para aplicar/reverter deltas
        // Problema: Difícil garantir integridade
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Editor de Imagens - Problema de Undo ===\n");

            var editor = new ImageEditor(1920, 1080);
            var history = new EditorWithFullSnapshots(editor);

            Console.WriteLine("=== Realizando Edições ===");
            
            history.SaveState(); // Estado inicial
            editor.DisplayInfo();

            editor.ApplyBrightness(20);
            history.SaveState();

            editor.ApplyFilter("Sepia");
            history.SaveState();

            editor.Rotate(90);
            history.SaveState();

            editor.Crop(1280, 720);
            history.SaveState();

            editor.DisplayInfo();

            Console.WriteLine("=== Desfazendo ===");
            history.Undo();
            editor.DisplayInfo();

            Console.WriteLine("\n=== PROBLEMAS ===");
            Console.WriteLine("Abordagem 1 (Snapshot completo):");
            Console.WriteLine("✗ Uso excessivo de memória (duplica estado completo)");
            Console.WriteLine("✗ Performance ruim (copia grandes arrays)");
            Console.WriteLine("✗ Viola encapsulamento (getters/setters para tudo)");
            Console.WriteLine("✗ Estado interno exposto publicamente");
            Console.WriteLine();
            Console.WriteLine("Abordagem 2 (Getters/Setters públicos):");
            Console.WriteLine("✗ Quebra encapsulamento do editor");
            Console.WriteLine("✗ Qualquer código pode modificar estado interno");
            Console.WriteLine("✗ Difícil garantir invariantes");
            Console.WriteLine();
            Console.WriteLine("Abordagem 3 (Delta/Diff):");
            Console.WriteLine("✗ Complexo de implementar corretamente");
            Console.WriteLine("✗ Frágil (fácil introduzir bugs)");
            Console.WriteLine("✗ Difícil reverter operações complexas");

            Console.WriteLine("\n=== Requisitos Não Atendidos ===");
            Console.WriteLine("• Salvar estado sem expor internals");
            Console.WriteLine("• Uso eficiente de memória");
            Console.WriteLine("• Manter encapsulamento forte");
            Console.WriteLine("• Permitir múltiplos pontos de restauração");
            Console.WriteLine("• Serialização de estado (salvar em arquivo)");

            // Perguntas para reflexão:
            // - Como capturar estado sem violar encapsulamento?
            // - Como armazenar snapshots de forma eficiente?
            // - Como permitir que objeto se restaure sem expor internals?
            // - Como externalizar estado mantendo integridade?
        }
    }
}
