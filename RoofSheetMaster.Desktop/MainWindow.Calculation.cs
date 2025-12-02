using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using RoofSheetMaster.Core;
using RoofSheetMaster.Desktop.Views;


namespace RoofSheetMaster.Desktop;

public partial class MainWindow
{
    private MaterialList? _lastMaterials;
    private IReadOnlyList<FlashingSummary>? _lastFlashings;

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

    private static double RoundToIncrement(double value, double increment)
    {
        if (increment <= 0)
            return value;

        return Math.Round(value / increment) * increment;
    }

    private string GetUnitsDescription()
    {
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
                var flLength = GetOverrideOrDefault(
                    HipFrontLeftLengthTextBox, roofLength, "Hip FrontLeft length", overridesEnabled);
                var flWidth = GetOverrideOrDefault(
                    HipFrontLeftWidthTextBox, roofWidth, "Hip FrontLeft width", overridesEnabled);

                var frLength = GetOverrideOrDefault(
                    HipFrontRightLengthTextBox, roofLength, "Hip FrontRight length", overridesEnabled);
                var frWidth = GetOverrideOrDefault(
                    HipFrontRightWidthTextBox, roofWidth, "Hip FrontRight width", overridesEnabled);

                var blLength = GetOverrideOrDefault(
                    HipBackLeftLengthTextBox, roofLength, "Hip BackLeft length", overridesEnabled);
                var blWidth = GetOverrideOrDefault(
                    HipBackLeftWidthTextBox, roofWidth, "Hip BackLeft width", overridesEnabled);

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
                var upperLength = GetOverrideOrDefault(
                    ValleyUpperLengthTextBox, roofLength, "Valley Upper length", overridesEnabled);
                var upperWidth = GetOverrideOrDefault(
                    ValleyUpperWidthTextBox, roofWidth, "Valley Upper width", overridesEnabled);

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

            // Rounding first, so flashings based on final lengths.
            if (roundingIncrement > 0)
            {
                foreach (var panel in materials.Panels)
                    panel.SheetLength = RoundToIncrement(panel.SheetLength, roundingIncrement);
            }

            // Calculate flashings / accessories from panels.
            var flashings = FlashingCalculator.CalculateFlashings(materials);

            // store for exports
            _lastMaterials = materials;
            _lastFlashings = flashings;

            // bind UI lists
            MaterialsViewControl.PanelsListBoxControl.ItemsSource = materials.Panels;
            MaterialsViewControl.SheetSummaryListBoxControl.ItemsSource = materials.SheetSummaries;
            MaterialsViewControl.FlashingListBoxControl.ItemsSource = flashings;


            ResultSummaryTextBlock.Text =
                $"Total sheets: {materials.TotalSheets} {summarySuffix} [{unitsDescription}]";

            RenderDiagram(materials.Panels);
        }
        catch (Exception ex)
        {
            _lastMaterials = null;
            _lastFlashings = null;

            ResultSummaryTextBlock.Text = $"Error: {ex.Message}";
            MaterialsViewControl.PanelsListBoxControl.ItemsSource = Array.Empty<object>();
            MaterialsViewControl.SheetSummaryListBoxControl.ItemsSource = Array.Empty<object>();
            MaterialsViewControl.FlashingListBoxControl.ItemsSource = Array.Empty<object>();
            DiagramCanvas.Children.Clear();
        }
    }
}
