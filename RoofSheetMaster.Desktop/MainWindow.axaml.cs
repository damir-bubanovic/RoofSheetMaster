using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Media;
using RoofSheetMaster.Core;
using PanelModel = RoofSheetMaster.Core.Panel;

namespace RoofSheetMaster.Desktop;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
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

            var roofTypeIndex = RoofTypeComboBox.SelectedIndex;

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
                // Hip roof (4 faces, same dimensions for now)
                var hipRoof = new HipRoof
                {
                    FrontLeft = new RoofFace
                    {
                        Name = "FrontLeft",
                        RoofLength = roofLength,
                        RoofWidth = roofWidth,
                        RoofAngleDegrees = roofAngle,
                        SheetWidth = sheetWidth,
                        SheetOverlap = sheetOverlap,
                        RidgeGap = ridgeGap
                    },
                    FrontRight = new RoofFace
                    {
                        Name = "FrontRight",
                        RoofLength = roofLength,
                        RoofWidth = roofWidth,
                        RoofAngleDegrees = roofAngle,
                        SheetWidth = sheetWidth,
                        SheetOverlap = sheetOverlap,
                        RidgeGap = ridgeGap
                    },
                    BackLeft = new RoofFace
                    {
                        Name = "BackLeft",
                        RoofLength = roofLength,
                        RoofWidth = roofWidth,
                        RoofAngleDegrees = roofAngle,
                        SheetWidth = sheetWidth,
                        SheetOverlap = sheetOverlap,
                        RidgeGap = ridgeGap
                    },
                    BackRight = new RoofFace
                    {
                        Name = "BackRight",
                        RoofLength = roofLength,
                        RoofWidth = roofWidth,
                        RoofAngleDegrees = roofAngle,
                        SheetWidth = sheetWidth,
                        SheetOverlap = sheetOverlap,
                        RidgeGap = ridgeGap
                    }
                };

                materials = RoofCalculator.CalculateHipRoof(hipRoof);
                summarySuffix = "(hip, 4 faces same for now)";
            }
            else
            {
                // Valley roof (2 faces, same dimensions for now)
                var valleyRoof = new ValleyRoof
                {
                    UpperFace = new RoofFace
                    {
                        Name = "Upper",
                        RoofLength = roofLength,
                        RoofWidth = roofWidth,
                        RoofAngleDegrees = roofAngle,
                        SheetWidth = sheetWidth,
                        SheetOverlap = sheetOverlap,
                        RidgeGap = ridgeGap
                    },
                    LowerFace = new RoofFace
                    {
                        Name = "Lower",
                        RoofLength = roofLength,
                        RoofWidth = roofWidth,
                        RoofAngleDegrees = roofAngle,
                        SheetWidth = sheetWidth,
                        SheetOverlap = sheetOverlap,
                        RidgeGap = ridgeGap
                    }
                };

                materials = RoofCalculator.CalculateValleyRoof(valleyRoof);
                summarySuffix = "(valley, 2 faces same for now)";
            }

            ResultSummaryTextBlock.Text =
                $"Total sheets: {materials.TotalSheets} {summarySuffix}";
            PanelListBox.ItemsSource = materials.Panels;

            // Draw diagram from panels
            RenderDiagram(materials.Panels);
        }
        catch (Exception ex)
        {
            ResultSummaryTextBlock.Text = $"Error: {ex.Message}";
            PanelListBox.ItemsSource = Array.Empty<object>();
            DiagramCanvas.Children.Clear();
        }
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
