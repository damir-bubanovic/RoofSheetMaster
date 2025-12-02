using System;
using System.IO;
using System.Text;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using RoofSheetMaster.Core;

namespace RoofSheetMaster.Desktop;

public partial class MainWindow
{
    private async void OnExportCutListHtmlClick(object? sender, RoutedEventArgs e)
    {
        if (_lastMaterials == null || _lastMaterials.Panels.Count == 0)
        {
            ResultSummaryTextBlock.Text = "Nothing to export – calculate a roof first.";
            return;
        }

        var options = new FilePickerSaveOptions
        {
            Title = "Export cut list (HTML)",
            SuggestedFileName = "CutList.html",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("HTML files") { Patterns = new[] { "*.html", "*.htm" } }
            }
        };

        var file = await this.StorageProvider.SaveFilePickerAsync(options);
        if (file == null) return;

        var html = BuildCutListHtml(_lastMaterials, _lastFlashings);

        await using var stream = await file.OpenWriteAsync();
        using var writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        writer.Write(html);

        ResultSummaryTextBlock.Text = $"Exported cut list HTML to: {file.Path}";
    }

    private static string BuildCutListHtml(
        MaterialList materials,
        System.Collections.Generic.IReadOnlyList<FlashingSummary>? flashings)
    {
        var culture = CultureInfo.InvariantCulture;
        var sb = new StringBuilder();

        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html>");
        sb.AppendLine("<head>");
        sb.AppendLine("  <meta charset=\"utf-8\" />");
        sb.AppendLine("  <title>RoofSheetMaster – Cut List</title>");
        sb.AppendLine("  <style>");
        sb.AppendLine("    body { font-family: system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif; margin: 20px; }");
        sb.AppendLine("    h1 { margin-bottom: 0.2em; }");
        sb.AppendLine("    h2 { margin-top: 1.6em; margin-bottom: 0.4em; }");
        sb.AppendLine("    table { border-collapse: collapse; width: 100%; max-width: 900px; }");
        sb.AppendLine("    th, td { border: 1px solid #cccccc; padding: 4px 8px; font-size: 13px; }");
        sb.AppendLine("    th { background: #f0f0f0; text-align: left; }");
        sb.AppendLine("    tbody tr:nth-child(even) { background: #fafafa; }");
        sb.AppendLine("    .small { font-size: 11px; color: #666; }");
        sb.AppendLine("  </style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");

        sb.AppendLine("  <h1>RoofSheetMaster – Cut List</h1>");
        sb.AppendLine($"  <p class=\"small\">Generated at {DateTime.Now}</p>");
        sb.AppendLine($"  <p>Total sheets: <strong>{materials.Panels.Count}</strong></p>");

        // Sheet summary (grouped lengths)
        sb.AppendLine("  <h2>Sheet length summary</h2>");
        sb.AppendLine("  <table>");
        sb.AppendLine("    <thead><tr><th>Sheet length</th><th>Count</th></tr></thead>");
        sb.AppendLine("    <tbody>");

        foreach (var s in materials.SheetSummaries)
        {
            var len = s.SheetLength.ToString("F3", culture);
            sb.AppendLine($"      <tr><td>{len}</td><td>{s.Count}</td></tr>");
        }

        sb.AppendLine("    </tbody>");
        sb.AppendLine("  </table>");

        // Full panel list
        sb.AppendLine("  <h2>Panel list</h2>");
        sb.AppendLine("  <table>");
        sb.AppendLine("    <thead>");
        sb.AppendLine("      <tr>");
        sb.AppendLine("        <th>#</th>");
        sb.AppendLine("        <th>Face</th>");
        sb.AppendLine("        <th>Eave position</th>");
        sb.AppendLine("        <th>Effective width</th>");
        sb.AppendLine("        <th>Sheet length</th>");
        sb.AppendLine("      </tr>");
        sb.AppendLine("    </thead>");
        sb.AppendLine("    <tbody>");

        foreach (var p in materials.Panels)
        {
            var face = string.IsNullOrWhiteSpace(p.Face)
                ? "&nbsp;"
                : System.Net.WebUtility.HtmlEncode(p.Face);

            var eavePos = p.EavePosition.ToString("F3", culture);
            var effWidth = p.EffectiveWidth.ToString("F3", culture);
            var sheetLen = p.SheetLength.ToString("F3", culture);

            sb.AppendLine("      <tr>");
            sb.AppendLine($"        <td>{p.Index}</td>");
            sb.AppendLine($"        <td>{face}</td>");
            sb.AppendLine($"        <td>{eavePos}</td>");
            sb.AppendLine($"        <td>{effWidth}</td>");
            sb.AppendLine($"        <td>{sheetLen}</td>");
            sb.AppendLine("      </tr>");
        }

        sb.AppendLine("    </tbody>");
        sb.AppendLine("  </table>");

        // Flashings (if present)
        if (flashings != null && flashings.Count > 0)
        {
            sb.AppendLine("  <h2>Flashings / accessories</h2>");
            sb.AppendLine("  <table>");
            sb.AppendLine("    <thead>");
            sb.AppendLine("      <tr>");
            sb.AppendLine("        <th>Name</th>");
            sb.AppendLine("        <th>Total length</th>");
            sb.AppendLine("        <th>Count</th>");
            sb.AppendLine("        <th>Notes</th>");
            sb.AppendLine("      </tr>");
            sb.AppendLine("    </thead>");
            sb.AppendLine("    <tbody>");

            foreach (var f in flashings)
            {
                AppendFlashingRow(sb, f, culture);
            }

            sb.AppendLine("    </tbody>");
            sb.AppendLine("  </table>");
        }

        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }

    private static void AppendFlashingRow(
        StringBuilder sb,
        FlashingSummary f,
        CultureInfo culture)
    {
        var lengthText = f.TotalLength <= 0
            ? "&nbsp;"
            : f.TotalLength.ToString("F3", culture);

        var countText = f.Count > 0
            ? f.Count.ToString(culture)
            : "&nbsp;";

        var safeName = System.Net.WebUtility.HtmlEncode(f.Name ?? string.Empty);
        var notesText = string.IsNullOrWhiteSpace(f.Notes)
            ? "&nbsp;"
            : System.Net.WebUtility.HtmlEncode(f.Notes);

        sb.AppendLine("      <tr>");
        sb.AppendLine($"        <td>{safeName}</td>");
        sb.AppendLine($"        <td>{lengthText}</td>");
        sb.AppendLine($"        <td>{countText}</td>");
        sb.AppendLine($"        <td>{notesText}</td>");
        sb.AppendLine("      </tr>");
    }
}
