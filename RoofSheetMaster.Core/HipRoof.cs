namespace RoofSheetMaster.Core;

/// <summary>
/// Simple hip roof consisting of up to four faces.
/// For now this is just a container; geometry logic will be added later.
/// </summary>
public class HipRoof
{
    public RoofFace? FrontLeft { get; set; }
    public RoofFace? FrontRight { get; set; }
    public RoofFace? BackLeft { get; set; }
    public RoofFace? BackRight { get; set; }
}
