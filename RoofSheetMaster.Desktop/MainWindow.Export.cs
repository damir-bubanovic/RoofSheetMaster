using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace RoofSheetMaster.Desktop;

public partial class MainWindow
{
    private async void OnExportSheetSummaryCsvClick(object? sender, RoutedEventArgs e)
    {
        if (_lastMaterials == null || _lastMaterials.SheetSummaries.Count == 0)
        {
            ResultSummaryTextBlock.Text = "Nothing to export – calculate a roof first.";
            return;
        }

        var options = new FilePickerSaveOptions
        {
            Title = "Export sheet summary CSV",
            SuggestedFileName = "SheetSummary.csv",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("CSV files") { Patterns = new[] { "*.csv" } }
            }
        };

        var file = await this.StorageProvider.SaveFilePickerAsync(options);
        if (file == null) return;

        await using var stream = await file.OpenWriteAsync();
        using var writer = new StreamWriter(stream);

        writer.WriteLine("SheetLength,Count");
        foreach (var s in _lastMaterials.SheetSummaries)
            writer.WriteLine($"{s.SheetLength:F3},{s.Count}");

        ResultSummaryTextBlock.Text = $"Exported sheet summary CSV to: {file.Path}";
    }


    private async void OnExportPanelsCsvClick(object? sender, RoutedEventArgs e)
    {
        if (_lastMaterials == null || _lastMaterials.Panels.Count == 0)
        {
            ResultSummaryTextBlock.Text = "Nothing to export – calculate a roof first.";
            return;
        }

        var options = new FilePickerSaveOptions
        {
            Title = "Export full panel list CSV",
            SuggestedFileName = "Panels.csv",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("CSV files") { Patterns = new[] { "*.csv" } }
            }
        };

        var file = await this.StorageProvider.SaveFilePickerAsync(options);
        if (file == null) return;

        await using var stream = await file.OpenWriteAsync();
        using var writer = new StreamWriter(stream);

        writer.WriteLine("Face,Index,EavePosition,EffectiveWidth,SheetLength");
        foreach (var p in _lastMaterials.Panels)
        {
            var face = p.Face ?? string.Empty;
            writer.WriteLine(
                $"{face},{p.Index},{p.EavePosition:F3},{p.EffectiveWidth:F3},{p.SheetLength:F3}");
        }

        ResultSummaryTextBlock.Text = $"Exported panel list CSV to: {file.Path}";
    }


    private async void OnExportFlashingsCsvClick(object? sender, RoutedEventArgs e)
    {
        if (_lastFlashings == null || _lastFlashings.Count == 0)
        {
            ResultSummaryTextBlock.Text = "Nothing to export – calculate a roof first.";
            return;
        }

        var options = new FilePickerSaveOptions
        {
            Title = "Export flashings CSV",
            SuggestedFileName = "Flashings.csv",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("CSV files") { Patterns = new[] { "*.csv" } }
            }
        };

        var file = await this.StorageProvider.SaveFilePickerAsync(options);
        if (file == null) return;

        await using var stream = await file.OpenWriteAsync();
        using var writer = new StreamWriter(stream);

        writer.WriteLine("Name,TotalLength,Count,Notes");

        foreach (var f in _lastFlashings)
        {
            var safeName = f.Name?.Replace(",", " ") ?? "";
            var safeNotes = f.Notes?.Replace(",", " ") ?? "";

            writer.WriteLine($"{safeName},{f.TotalLength:F3},{f.Count},{safeNotes}");
        }

        ResultSummaryTextBlock.Text = $"Exported flashings CSV to: {file.Path}";
    }
}
