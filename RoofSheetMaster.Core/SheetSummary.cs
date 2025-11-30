namespace RoofSheetMaster.Core;

public class SheetSummary
{
    /// <summary>
    /// The sheet length for this group of panels (in roof units, e.g. feet or meters).
    /// </summary>
    public double SheetLength { get; set; }

    /// <summary>
    /// Number of panels that share this sheet length.
    /// </summary>
    public int Count { get; set; }
}
