using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using RoofSheetMaster.Core;
using RoofSheetMaster.Desktop.Views;

namespace RoofSheetMaster.Desktop;

public partial class MainWindow
{
    // Shared calculation/export state
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
}
