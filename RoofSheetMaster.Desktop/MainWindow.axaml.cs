using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using RoofSheetMaster.Core;

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
            // Parse input values from text boxes
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

            var input = new RoofInput
            {
                RoofLength = roofLength,
                RoofWidth = roofWidth,
                RoofAngleDegrees = roofAngle,
                SheetWidth = sheetWidth,
                SheetOverlap = sheetOverlap,
                RidgeGap = ridgeGap
            };

            var materials = RoofCalculator.CalculateSimpleFace(input);

            ResultSummaryTextBlock.Text = $"Total sheets: {materials.TotalSheets}";
            PanelListBox.ItemsSource = materials.Panels;
        }
        catch (Exception ex)
        {
            ResultSummaryTextBlock.Text = $"Error: {ex.Message}";
            PanelListBox.ItemsSource = Array.Empty<object>();
        }
    }
}
