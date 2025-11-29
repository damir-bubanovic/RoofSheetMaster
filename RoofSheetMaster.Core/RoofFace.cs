namespace RoofSheetMaster.Core;

/// <summary>
/// Represents a single planar roof face (e.g. one side of a gable or hip roof).
/// For now it reuses the same properties as RoofInput; later we can extend it
/// with extra geometric info (orientation, offsets from hips/valleys, etc.).
/// </summary>
public class RoofFace
{
    public string Name { get; set; } = "Face";

    public double RoofLength { get; set; }       // along eave
    public double RoofWidth { get; set; }        // eave to ridge (horizontal)
    public double RoofAngleDegrees { get; set; } // slope angle
    public double SheetWidth { get; set; }
    public double SheetOverlap { get; set; }
    public double RidgeGap { get; set; }
}
