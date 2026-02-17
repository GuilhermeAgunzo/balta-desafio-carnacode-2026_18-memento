using Memento.Editor;
using Memento.History;

Console.WriteLine("=== Editor de Imagens - Memento Pattern ===\n");

var editor = new ImageEditor(1920, 1080);
var history = new EditorHistory(editor, maxSnapshots: 20);

history.Backup("Estado inicial");
editor.DisplayInfo();

editor.ApplyBrightness(20);
history.Backup("Brilho +20");

editor.ApplyFilter("Sepia");
history.Backup("Filtro Sepia");

editor.Rotate(90);
history.Backup("Rotação 90°");

editor.Crop(1280, 720);
history.Backup("Crop 1280x720");

editor.DisplayInfo();

Console.WriteLine("=== Desfazendo (Undo) ===");
history.Undo();
editor.DisplayInfo();

history.Undo();
editor.DisplayInfo();

Console.WriteLine("=== Refazendo (Redo) ===");
history.Redo();
editor.DisplayInfo();

Console.WriteLine("=== Mais um Undo ===");
history.Undo();
history.Undo();
editor.DisplayInfo();

Console.WriteLine("=== Undo até o limite ===");
history.Undo();
editor.DisplayInfo();

history.Undo();