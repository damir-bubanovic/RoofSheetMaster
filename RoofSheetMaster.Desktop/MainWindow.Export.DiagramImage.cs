using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;

namespace RoofSheetMaster.Desktop;

public partial class MainWindow
{
    private async void OnExportLayoutPngClick(object? sender, RoutedEventArgs e)
    {
        if (DiagramCanvas == null)
        {
            ResultSummaryTextBlock.Text = "Nothing to export – diagram not available.";
            return;
        }

        var bounds = DiagramCanvas.Bounds;
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            ResultSummaryTextBlock.Text = "Nothing to export – calculate a roof first.";
            return;
        }

        var options = new FilePickerSaveOptions
        {
            Title = "Export layout image (PNG)",
            SuggestedFileName = "RoofLayout.png",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("PNG image") { Patterns = new[] { "*.png" } }
            }
        };

        var file = await this.StorageProvider.SaveFilePickerAsync(options);
        if (file == null)
        {
            ResultSummaryTextBlock.Text = "Export cancelled.";
            return;
        }

        // Create a bitmap matching the current canvas size (at 96 DPI)
        var pixelSize = new PixelSize(
            (int)Math.Ceiling(bounds.Width),
            (int)Math.Ceiling(bounds.Height));

        if (pixelSize.Width <= 0 || pixelSize.Height <= 0)
        {
            ResultSummaryTextBlock.Text = "Nothing to export – diagram size is zero.";
            return;
        }

        var dpi = new Vector(96, 96);
        var bitmap = new RenderTargetBitmap(pixelSize, dpi);

        // Ensure layout is up to date
        DiagramCanvas.Measure(bounds.Size);
        DiagramCanvas.Arrange(new Rect(bounds.Size));

        // Render the current diagram (including current zoom transform)
        bitmap.Render(DiagramCanvas);

        await using (var stream = await file.OpenWriteAsync())
        {
            // RenderTargetBitmap.Save(stream) writes PNG
            bitmap.Save(stream);
        }

        ResultSummaryTextBlock.Text = $"Exported layout PNG to: {file.Path}";
    }
}
