using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using RoofSheetMaster.Core;
using PanelModel = RoofSheetMaster.Core.Panel;

namespace RoofSheetMaster.Desktop;

public partial class MainWindow : Window
{
    private MaterialList? _lastMaterials;

    public MainWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Read an optional override value.
    /// If overrides are disabled, always returns defaultValue.
    /// If overrides are enabled:
    ///   - empty -> defaultValue
    ///   - invalid text -> throws with a clear error.
    /// </summary>
    private double GetOverrideOrDefault(
        TextBox textBox,
        double defaultValue,
        string fieldLabel,
        bool overridesEnabled)
    {
        if (!overridesEnabled)
            return defaultValue;

        var raw = textBox.Text?.Trim();

        if (string.IsNullOrEmpty(raw))
            return defaultValue;

        if (!double.TryParse(raw, out var value))
            throw new Exception($"Invalid {fieldLabel} override.");

        return value;
    }

    /// <summary>
    /// Round a value to the nearest multiple of increment.
    /// If increment <= 0, returns the original value.
    /// </summary>
    private static double RoundToIncrement(double value, double increment)
    {
        if (increment <= 0)
            return value;

        return Math.Round(value / increment) * increment;
    }

    /// <summary>
    /// Get human-readable units description from the Units combo.
    /// </summary>
    private string GetUnitsDescription()
    {
        // 0 = Metric (m), 1 = Imperial (ft)
        var idx = UnitsComboBox.SelectedIndex;
        return idx == 0 ? "metric (m)" : "imperial (ft)";
    }

    private void OnFaceOverridesToggle(object? sender, RoutedEventArgs e)
    {
        var enabled = FaceOverridesCheckBox.IsChecked == true;
        FaceOverridesPanel.IsVisible = enabled;
    }

    private void OnCalculateClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (!double.TryParse(RoofLengthTextBox.Text, out var roofLength))
                throw new Exception("Invalid roof length.");
            if (!double.TryParse(RoofWidthTextBox.Text, out var roofWidth))
                throw new Exception("Invalid roof width.");
            if (!double.TryParse(RoofAngleTextBox.Text, out var roofAngle))
                throw new Exception("Invalid roof angle.");
            if (!double.TryParse(SheetWidthTextBox.Text, out var sheetWidth))
                throw new Exception("Invalid sheet width.");
            if (!double.TryParse(SheetOverlapTextBox.Text, out var sheetOverlap))
                throw new Exception("Invalid sheet overlap.");
            if (!double.TryParse(RidgeGapTextBox.Text, out var ridgeGap))
                throw new Exception("Invalid ridge gap.");

            // Sheet length rounding
            double roundingIncrement = 0;
            var roundingRaw = SheetLengthRoundingTextBox.Text?.Trim();
            if (!string.IsNullOrEmpty(roundingRaw))
            {
                if (!double.TryParse(roundingRaw, out roundingIncrement))
                    throw new Exception("Invalid sheet length rounding value.");
            }
            if (roundingIncrement < 0)
                throw new Exception("Sheet length rounding must be zero or positive.");

            var roofTypeIndex = RoofTypeComboBox.SelectedIndex;
            var overridesEnabled = FaceOverridesCheckBox.IsChecked == true;
            var unitsDescription = GetUnitsDescription();

            MaterialList materials;
            string summarySuffix;

            if (roofTypeIndex == 0)
            {
                // Single face
                var input = new RoofInput
                {
                    RoofLength = roofLength,
                    RoofWidth = roofWidth,
                    RoofAngleDegrees = roofAngle,
                    SheetWidth = sheetWidth,
                    SheetOverlap = sheetOverlap,
                    RidgeGap = ridgeGap
                };

                materials = RoofCalculator.CalculateSimpleFace(input);
                summarySuffix = "(single face)";
            }
            else if (roofTypeIndex == 1)
            {
                // Gable roof (two faces, same dimensions)
                var input = new RoofInput
                {
                    RoofLength = roofLength,
                    RoofWidth = roofWidth,
                    RoofAngleDegrees = roofAngle,
                    SheetWidth = sheetWidth,
                    SheetOverlap = sheetOverlap,
                    RidgeGap = ridgeGap
                };

                materials = RoofCalculator.CalculateGableRoof(input);
                summarySuffix = "(gable, both faces)";
            }
            else if (roofTypeIndex == 2)
            {
                // Hip roof (4 faces, with optional per-face overrides)

                // FrontLeft overrides
                var flLength = GetOverrideOrDefault(
                    HipFrontLeftLengthTextBox, roofLength, "Hip FrontLeft length", overridesEnabled);
                var flWidth = GetOverrideOrDefault(
                    HipFrontLeftWidthTextBox, roofWidth, "Hip FrontLeft width", overridesEnabled);

                // FrontRight overrides
                var frLength = GetOverrideOrDefault(
                    HipFrontRightLengthTextBox, roofLength, "Hip FrontRight length", overridesEnabled);
                var frWidth = GetOverrideOrDefault(
                    HipFrontRightWidthTextBox, roofWidth, "Hip FrontRight width", overridesEnabled);

                // BackLeft overrides
                var blLength = GetOverrideOrDefault(
                    HipBackLeftLengthTextBox, roofLength, "Hip BackLeft length", overridesEnabled);
                var blWidth = GetOverrideOrDefault(
                    HipBackLeftWidthTextBox, roofWidth, "Hip BackLeft width", overridesEnabled);

                // BackRight overrides
                var brLength = GetOverrideOrDefault(
                    HipBackRightLengthTextBox, roofLength, "Hip BackRight length", overridesEnabled);
                var brWidth = GetOverrideOrDefault(
                    HipBackRightWidthTextBox, roofWidth, "Hip BackRight width", overridesEnabled);

                var hipRoof = new HipRoof
                {
                    FrontLeft = new RoofFace
                    {
                        Name = "FrontLeft",
                        RoofLength = flLength,
                        RoofWidth = flWidth,
                        RoofAngleDegrees = roofAngle,
                        SheetWidth = sheetWidth,
                        SheetOverlap = sheetOverlap,
                        RidgeGap = ridgeGap
                    },
                    FrontRight = new RoofFace
                    {
                        Name = "FrontRight",
                        RoofLength = frLength,
                        RoofWidth = frWidth,
                        RoofAngleDegrees = roofAngle,
                        SheetWidth = sheetWidth,
                        SheetOverlap = sheetOverlap,
                        RidgeGap = ridgeGap
                    },
                    BackLeft = new RoofFace
                    {
                        Name = "BackLeft",
                        RoofLength = blLength,
                        RoofWidth = blWidth,
                        RoofAngleDegrees = roofAngle,
                        SheetWidth = sheetWidth,
                        SheetOverlap = sheetOverlap,
                        RidgeGap = ridgeGap
                    },
                    BackRight = new RoofFace
                    {
                        Name = "BackRight",
                        RoofLength = brLength,
                        RoofWidth = brWidth,
                        RoofAngleDegrees = roofAngle,
                        SheetWidth = sheetWidth,
                        SheetOverlap = sheetOverlap,
                        RidgeGap = ridgeGap
                    }
                };

                materials = RoofCalculator.CalculateHipRoof(hipRoof);
                summarySuffix = overridesEnabled
                    ? "(hip, per-face dimensions)"
                    : "(hip, 4 faces same for now)";
            }
            else
            {
                // Valley roof (2 faces, with optional per-face overrides)

                // Upper face overrides
                var upperLength = GetOverrideOrDefault(
                    ValleyUpperLengthTextBox, roofLength, "Valley Upper length", overridesEnabled);
                var upperWidth = GetOverrideOrDefault(
                    ValleyUpperWidthTextBox, roofWidth, "Valley Upper width", overridesEnabled);

                // Lower face overrides
                var lowerLength = GetOverrideOrDefault(
                    ValleyLowerLengthTextBox, roofLength, "Valley Lower length", overridesEnabled);
                var lowerWidth = GetOverrideOrDefault(
                    ValleyLowerWidthTextBox, roofWidth, "Valley Lower width", overridesEnabled);

                var valleyRoof = new ValleyRoof
                {
                    UpperFace = new RoofFace
                    {
                        Name = "Upper",
                        RoofLength = upperLength,
                        RoofWidth = upperWidth,
                        RoofAngleDegrees = roofAngle,
                        SheetWidth = sheetWidth,
                        SheetOverlap = sheetOverlap,
                        RidgeGap = ridgeGap
                    },
                    LowerFace = new RoofFace
                    {
                        Name = "Lower",
                        RoofLength = lowerLength,
                        RoofWidth = lowerWidth,
                        RoofAngleDegrees = roofAngle,
                        SheetWidth = sheetWidth,
                        SheetOverlap = sheetOverlap,
                        RidgeGap = ridgeGap
                    }
                };

                materials = RoofCalculator.CalculateValleyRoof(valleyRoof);
                summarySuffix = overridesEnabled
                    ? "(valley, per-face dimensions)"
                    : "(valley, 2 faces same for now)";
            }

            // Apply sheet length rounding if requested.
            if (roundingIncrement > 0)
            {
                foreach (var panel in materials.Panels)
                {
                    panel.SheetLength = RoundToIncrement(panel.SheetLength, roundingIncrement);
                }
            }

            // remember last materials for export / summary
            _lastMaterials = materials;

            // bind UI lists
            PanelListBox.ItemsSource = materials.Panels;
            SheetSummaryListBox.ItemsSource = materials.SheetSummaries;

            ResultSummaryTextBlock.Text =
                $"Total sheets: {materials.TotalSheets} {summarySuffix} [{unitsDescription}]";

            // Draw diagram from panels
            RenderDiagram(materials.Panels);
        }
        catch (Exception ex)
        {
            _lastMaterials = null;
            ResultSummaryTextBlock.Text = $"Error: {ex.Message}";
            PanelListBox.ItemsSource = Array.Empty<object>();
            SheetSummaryListBox.ItemsSource = Array.Empty<object>();
            DiagramCanvas.Children.Clear();
        }
    }

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
                new FilePickerFileType("CSV files")
                {
                    Patterns = new[] { "*.csv" }
                }
            }
        };

        var file = await this.StorageProvider.SaveFilePickerAsync(options);
        if (file == null)
            return;

        await using (var stream = await file.OpenWriteAsync())
        using (var writer = new StreamWriter(stream))
        {
            writer.WriteLine("SheetLength,Count");
            foreach (var s in _lastMaterials.SheetSummaries)
            {
                writer.WriteLine($"{s.SheetLength:F3},{s.Count}");
            }
        }

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
                new FilePickerFileType("CSV files")
                {
                    Patterns = new[] { "*.csv" }
                }
            }
        };

        var file = await this.StorageProvider.SaveFilePickerAsync(options);
        if (file == null)
            return;

        await using (var stream = await file.OpenWriteAsync())
        using (var writer = new StreamWriter(stream))
        {
            writer.WriteLine("Face,Index,EavePosition,EffectiveWidth,SheetLength");
            foreach (var p in _lastMaterials.Panels)
            {
                var face = p.Face ?? string.Empty;
                writer.WriteLine(
                    $"{face},{p.Index},{p.EavePosition:F3},{p.EffectiveWidth:F3},{p.SheetLength:F3}");
            }
        }

        ResultSummaryTextBlock.Text = $"Exported panel list CSV to: {file.Path}";
    }

    private void RenderDiagram(IReadOnlyList<PanelModel> panels)
    {
        DiagramCanvas.Children.Clear();

        if (panels == null || panels.Count == 0)
            return;

        var canvasWidth = DiagramCanvas.Bounds.Width;
        var canvasHeight = DiagramCanvas.Bounds.Height;

        if (canvasWidth <= 0) canvasWidth = 600;
        if (canvasHeight <= 0) canvasHeight = 400;

        const double margin = 20;

        // Group panels by face name; single-face panels will be in group "Face"
        var groups = panels
            .GroupBy(p => string.IsNullOrWhiteSpace(p.Face) ? "Face" : p.Face!)
            .ToList();

        var groupInfos =
            new List<(string Face, List<PanelModel> Panels, double TotalWidth, double MaxLength)>();

        foreach (var g in groups)
        {
            var list = g.OrderBy(p => p.EavePosition).ToList();

            double totalWidth = 0;
            double maxLength = 0;

            foreach (var p in list)
            {
                totalWidth += p.EffectiveWidth;
                if (p.SheetLength > maxLength)
                    maxLength = p.SheetLength;
            }

            groupInfos.Add((g.Key, list, totalWidth, maxLength));
        }

        double maxGroupWidth = groupInfos.Max(x => x.TotalWidth);
        double maxGroupLength = groupInfos.Max(x => x.MaxLength);

        var availableWidth = canvasWidth - 2 * margin;
        var availableHeight = canvasHeight - 2 * margin;

        var rowCount = groupInfos.Count;
        var rowHeight = availableHeight / Math.Max(1, rowCount);
        if (rowHeight < 40) rowHeight = 40;

        var scaleX = maxGroupWidth > 0 ? availableWidth / maxGroupWidth : 1.0;
        var scaleY = maxGroupLength > 0 ? (rowHeight - 20) / maxGroupLength : 1.0;

        var rowIndex = 0;

        foreach (var (faceName, panelList, totalWidth, maxLength) in groupInfos)
        {
            var yBase = margin + rowIndex * rowHeight;
            var xCursor = margin;

            foreach (var p in panelList)
            {
                var rectWidth = p.EffectiveWidth * scaleX;
                var rectHeight = p.SheetLength * scaleY;

                var x = xCursor;
                var y = yBase + (rowHeight - rectHeight) / 2.0;

                var rect = new Rectangle
                {
                    Width = rectWidth,
                    Height = rectHeight,
                    Stroke = Brushes.DarkSlateGray,
                    StrokeThickness = 1,
                    Fill = Brushes.LightGray
                };

                Canvas.SetLeft(rect, x);
                Canvas.SetTop(rect, y);
                DiagramCanvas.Children.Add(rect);

                // Label: two lines -> face on first line, panel index on second line
                string labelText;
                if (string.IsNullOrWhiteSpace(p.Face))
                {
                    labelText = $"P{p.Index}";
                }
                else
                {
                    labelText = $"{p.Face}\nP{p.Index}";
                }

                var text = new TextBlock
                {
                    Text = labelText,
                    FontSize = 9
                };

                Canvas.SetLeft(text, x + 2);
                Canvas.SetTop(text, y + 2);
                DiagramCanvas.Children.Add(text);

                xCursor += rectWidth;
            }

            rowIndex++;
        }
    }
}
