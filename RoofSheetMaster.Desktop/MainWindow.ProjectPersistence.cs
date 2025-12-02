using System;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using RoofSheetMaster.Core;

namespace RoofSheetMaster.Desktop;

public partial class MainWindow
{
    private ProjectSettings CaptureProject()
    {
        return new ProjectSettings
        {
            UnitsIndex = UnitsComboBox.SelectedIndex,
            RoofTypeIndex = RoofTypeComboBox.SelectedIndex,

            RoofLength = double.TryParse(RoofLengthTextBox.Text, out var rl) ? rl : 0,
            RoofWidth = double.TryParse(RoofWidthTextBox.Text, out var rw) ? rw : 0,
            RoofAngle = double.TryParse(RoofAngleTextBox.Text, out var ra) ? ra : 0,

            SheetWidth = double.TryParse(SheetWidthTextBox.Text, out var sw) ? sw : 0,
            SheetOverlap = double.TryParse(SheetOverlapTextBox.Text, out var so) ? so : 0,
            RidgeGap = double.TryParse(RidgeGapTextBox.Text, out var rg) ? rg : 0,
            SheetLengthRounding = double.TryParse(SheetLengthRoundingTextBox.Text, out var sr) ? sr : 0,

            FaceOverridesEnabled = FaceOverridesCheckBox.IsChecked == true,

            HipFrontLeftLength = HipFrontLeftLengthTextBox.Text,
            HipFrontLeftWidth  = HipFrontLeftWidthTextBox.Text,

            HipFrontRightLength = HipFrontRightLengthTextBox.Text,
            HipFrontRightWidth  = HipFrontRightWidthTextBox.Text,

            HipBackLeftLength = HipBackLeftLengthTextBox.Text,
            HipBackLeftWidth  = HipBackLeftWidthTextBox.Text,

            HipBackRightLength = HipBackRightLengthTextBox.Text,
            HipBackRightWidth  = HipBackRightWidthTextBox.Text,

            ValleyUpperLength = ValleyUpperLengthTextBox.Text,
            ValleyUpperWidth  = ValleyUpperWidthTextBox.Text,

            ValleyLowerLength = ValleyLowerLengthTextBox.Text,
            ValleyLowerWidth  = ValleyLowerWidthTextBox.Text
        };
    }

    private void RestoreProject(ProjectSettings p)
    {
        UnitsComboBox.SelectedIndex = p.UnitsIndex;
        RoofTypeComboBox.SelectedIndex = p.RoofTypeIndex;

        RoofLengthTextBox.Text = p.RoofLength.ToString();
        RoofWidthTextBox.Text = p.RoofWidth.ToString();
        RoofAngleTextBox.Text = p.RoofAngle.ToString();

        SheetWidthTextBox.Text = p.SheetWidth.ToString();
        SheetOverlapTextBox.Text = p.SheetOverlap.ToString();
        RidgeGapTextBox.Text = p.RidgeGap.ToString();
        SheetLengthRoundingTextBox.Text = p.SheetLengthRounding.ToString();

        FaceOverridesCheckBox.IsChecked = p.FaceOverridesEnabled;
        FaceOverridesPanel.IsVisible = p.FaceOverridesEnabled;

        HipFrontLeftLengthTextBox.Text = p.HipFrontLeftLength;
        HipFrontLeftWidthTextBox.Text  = p.HipFrontLeftWidth;

        HipFrontRightLengthTextBox.Text = p.HipFrontRightLength;
        HipFrontRightWidthTextBox.Text  = p.HipFrontRightWidth;

        HipBackLeftLengthTextBox.Text = p.HipBackLeftLength;
        HipBackLeftWidthTextBox.Text  = p.HipBackLeftWidth;

        HipBackRightLengthTextBox.Text = p.HipBackRightLength;
        HipBackRightWidthTextBox.Text  = p.HipBackRightWidth;

        ValleyUpperLengthTextBox.Text = p.ValleyUpperLength;
        ValleyUpperWidthTextBox.Text  = p.ValleyUpperWidth;

        ValleyLowerLengthTextBox.Text = p.ValleyLowerLength;
        ValleyLowerWidthTextBox.Text  = p.ValleyLowerWidth;
    }

    private async void OnSaveProjectClick(object? sender, RoutedEventArgs e)
    {
        var options = new FilePickerSaveOptions
        {
            Title = "Save Project",
            SuggestedFileName = "Project.json",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("JSON files"){ Patterns = new[] { "*.json" } }
            }
        };

        var file = await StorageProvider.SaveFilePickerAsync(options);
        if (file == null)
            return;

        var settings = CaptureProject();
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });

        await using var stream = await file.OpenWriteAsync();
        using var writer = new System.IO.StreamWriter(stream);
        await writer.WriteAsync(json);

        ResultSummaryTextBlock.Text = $"Project saved to: {file.Path}";
    }

    private async void OnLoadProjectClick(object? sender, RoutedEventArgs e)
    {
        var options = new FilePickerOpenOptions
        {
            Title = "Load Project",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("JSON files"){ Patterns = new[] { "*.json" } }
            }
        };

        var files = await StorageProvider.OpenFilePickerAsync(options);
        if (files == null || files.Count == 0)
            return;

        var file = files[0];

        await using var stream = await file.OpenReadAsync();
        var settings = await JsonSerializer.DeserializeAsync<ProjectSettings>(stream);

        if (settings == null)
        {
            ResultSummaryTextBlock.Text = "Failed to load project.";
            return;
        }

        RestoreProject(settings);

        ResultSummaryTextBlock.Text = $"Project loaded from: {file.Path}";
    }
}
