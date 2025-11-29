namespace RoofSheetMaster.Core;

public class Panel
{
    public int Index { get; set; }
    public double EffectiveWidth { get; set; }
    public double SheetLength { get; set; }

    // Position of panel center along the eave (0 at one end), same units as RoofLength.
    public double EavePosition { get; set; }

    // For multi-face roofs (e.g. Face A / Face B on a gable)
    public string? Face { get; set; }
}
