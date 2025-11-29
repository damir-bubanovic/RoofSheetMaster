using System;

namespace RoofSheetMaster.Core;

public static class RoofCalculator
{
    // Core calculation used for any single planar face
    private static MaterialList CalculateFaceInternal(
        double roofLength,
        double roofWidth,
        double roofAngleDegrees,
        double sheetWidth,
        double sheetOverlap,
        double ridgeGap,
        string? faceName)
    {
        if (sheetWidth <= 0)
            throw new ArgumentException("SheetWidth must be > 0");
        if (roofLength <= 0)
            throw new ArgumentException("RoofLength must be > 0");

        var coverage = sheetWidth - sheetOverlap;
        if (coverage <= 0)
            throw new ArgumentException("Sheet overlap cannot be >= sheet width");

        var sheetCount = (int)Math.Ceiling(roofLength / coverage);

        var horizRun = Math.Max(0, roofWidth - ridgeGap);

        var angleRad = roofAngleDegrees * Math.PI / 180.0;

        var sheetLength = horizRun / Math.Cos(angleRad);

        var result = new MaterialList();

        for (int i = 0; i < sheetCount; i++)
        {
            result.Panels.Add(new Panel
            {
                Index = i + 1,
                EffectiveWidth = coverage,
                SheetLength = sheetLength,
                Face = faceName
            });
        }

        return result;
    }

    // Existing API: simple face using RoofInput
    public static MaterialList CalculateSimpleFace(RoofInput input)
    {
        return CalculateFaceInternal(
            input.RoofLength,
            input.RoofWidth,
            input.RoofAngleDegrees,
            input.SheetWidth,
            input.SheetOverlap,
            input.RidgeGap,
            null);
    }

    // New: simple face using RoofFace
    public static MaterialList CalculateFace(RoofFace face)
    {
        return CalculateFaceInternal(
            face.RoofLength,
            face.RoofWidth,
            face.RoofAngleDegrees,
            face.SheetWidth,
            face.SheetOverlap,
            face.RidgeGap,
            face.Name);
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

        var materialsA = CalculateFaceInternal(
            faceA.RoofLength,
            faceA.RoofWidth,
            faceA.RoofAngleDegrees,
            faceA.SheetWidth,
            faceA.SheetOverlap,
            faceA.RidgeGap,
            "Face A");

        var materialsB = CalculateFaceInternal(
            inputB.RoofLength,
            inputB.RoofWidth,
            inputB.RoofAngleDegrees,
            inputB.SheetWidth,
            inputB.SheetOverlap,
            inputB.RidgeGap,
            "Face B");

        var combined = new MaterialList();
        combined.Panels.AddRange(materialsA.Panels);
        combined.Panels.AddRange(materialsB.Panels);

        return combined;
    }

    // Hip roof: sum panels from up to four faces
    public static MaterialList CalculateHipRoof(HipRoof roof)
    {
        var result = new MaterialList();

        void AddFace(RoofFace? face)
        {
            if (face == null)
                return;

            var faceMaterials = CalculateFace(face);
            result.Panels.AddRange(faceMaterials.Panels);
        }

        AddFace(roof.FrontLeft);
        AddFace(roof.FrontRight);
        AddFace(roof.BackLeft);
        AddFace(roof.BackRight);

        return result;
    }



    // Valley roof: sum panels from two faces
    public static MaterialList CalculateValleyRoof(ValleyRoof roof)
    {
        var result = new MaterialList();

        void AddFace(RoofFace? face)
        {
            if (face == null)
                return;

            var faceMaterials = CalculateFace(face);
            result.Panels.AddRange(faceMaterials.Panels);
        }

        AddFace(roof.UpperFace);
        AddFace(roof.LowerFace);

        return result;
    }



}
