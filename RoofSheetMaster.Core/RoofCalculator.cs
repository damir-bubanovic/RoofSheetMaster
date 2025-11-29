using System;

namespace RoofSheetMaster.Core;

public static class RoofCalculator
{
    // Simple one-slope, rectangular roof face
    public static MaterialList CalculateSimpleFace(RoofInput input)
    {
        if (input.SheetWidth <= 0)
            throw new ArgumentException("SheetWidth must be > 0");
        if (input.RoofLength <= 0)
            throw new ArgumentException("RoofLength must be > 0");

        var coverage = input.SheetWidth - input.SheetOverlap;
        if (coverage <= 0)
            throw new ArgumentException("Sheet overlap cannot be >= sheet width");

        var sheetCount = (int)Math.Ceiling(input.RoofLength / coverage);

        var horizRun = Math.Max(0, input.RoofWidth - input.RidgeGap);

        var angleRad = input.RoofAngleDegrees * Math.PI / 180.0;

        var sheetLength = horizRun / Math.Cos(angleRad);

        var result = new MaterialList();

        for (int i = 0; i < sheetCount; i++)
        {
            result.Panels.Add(new Panel
            {
                Index = i + 1,
                EffectiveWidth = coverage,
                SheetLength = sheetLength,
                Face = null // single face, not labeled yet
            });
        }

        return result;
    }

    // Gable roof: two faces. If faceB is null, we assume it is the same as faceA.
    public static MaterialList CalculateGableRoof(RoofInput faceA, RoofInput? faceB = null)
    {
        var inputB = faceB ?? new RoofInput
        {
            RoofLength = faceA.RoofLength,
            RoofWidth = faceA.RoofWidth,
            RoofAngleDegrees = faceA.RoofAngleDegrees,
            SheetWidth = faceA.SheetWidth,
            SheetOverlap = faceA.SheetOverlap,
            RidgeGap = faceA.RidgeGap
        };

        var materialsA = CalculateSimpleFace(faceA);
        var materialsB = CalculateSimpleFace(inputB);

        foreach (var p in materialsA.Panels)
            p.Face = "Face A";

        foreach (var p in materialsB.Panels)
            p.Face = "Face B";

        var combined = new MaterialList();
        combined.Panels.AddRange(materialsA.Panels);
        combined.Panels.AddRange(materialsB.Panels);

        return combined;
    }
}
