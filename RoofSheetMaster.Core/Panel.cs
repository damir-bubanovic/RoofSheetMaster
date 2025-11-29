namespace RoofSheetMaster.Core;

public class Panel
{
    public int Index { get; set; }
    public double EffectiveWidth { get; set; }
    public double SheetLength { get; set; }

    // For multi-face roofs (e.g. Face A / Face B on a gable)
    public string? Face { get; set; }
}
