using System;

namespace RoofSheetMaster.Core;

public static class RoofCalculator
{
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
                SheetLength = sheetLength
            });
        }

        return result;
    }
}
