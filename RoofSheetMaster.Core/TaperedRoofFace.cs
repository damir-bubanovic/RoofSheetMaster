namespace RoofSheetMaster.Core;

/// <summary>
/// A roof face where the horizontal run (eave-to-ridge distance)
/// changes from one end of the eave to the other (tapered).
/// This approximates what happens near hips/valleys: sheets get shorter.
/// </summary>
public class TaperedRoofFace
{
    public string Name { get; set; } = "Tapered";

    // Length along the eave (plan view)
    public double RoofLength { get; set; }

    // Horizontal run at the "start" end of the eave
    public double StartRun { get; set; }

    // Horizontal run at the "end" end of the eave
    public double EndRun { get; set; }

    public double RoofAngleDegrees { get; set; }

    public double SheetWidth { get; set; }
    public double SheetOverlap { get; set; }

    // Optional gap near ridge; for now applied equally
    public double RidgeGap { get; set; }
}
