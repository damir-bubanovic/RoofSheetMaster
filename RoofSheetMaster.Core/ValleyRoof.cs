namespace RoofSheetMaster.Core;

/// <summary>
/// Simple valley roof consisting of two faces that meet in a valley.
/// For now this is just a container; valley-specific geometry will be added later.
/// </summary>
public class ValleyRoof
{
    public RoofFace? UpperFace { get; set; }
    public RoofFace? LowerFace { get; set; }
}
