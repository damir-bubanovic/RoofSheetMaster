using System;
using System.Collections.Generic;

namespace RoofSheetMaster.Core;

public class ProjectSettings
{
    public int UnitsIndex { get; set; } = 1;
    public int RoofTypeIndex { get; set; } = 0;

    public double RoofLength { get; set; }
    public double RoofWidth { get; set; }
    public double RoofAngle { get; set; }

    public double SheetWidth { get; set; }
    public double SheetOverlap { get; set; }
    public double RidgeGap { get; set; }
    public double SheetLengthRounding { get; set; }

    public bool FaceOverridesEnabled { get; set; }

    public string? HipFrontLeftLength { get; set; }
    public string? HipFrontLeftWidth { get; set; }

    public string? HipFrontRightLength { get; set; }
    public string? HipFrontRightWidth { get; set; }

    public string? HipBackLeftLength { get; set; }
    public string? HipBackLeftWidth { get; set; }

    public string? HipBackRightLength { get; set; }
    public string? HipBackRightWidth { get; set; }

    public string? ValleyUpperLength { get; set; }
    public string? ValleyUpperWidth { get; set; }

    public string? ValleyLowerLength { get; set; }
    public string? ValleyLowerWidth { get; set; }
}
