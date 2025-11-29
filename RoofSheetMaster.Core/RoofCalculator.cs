using System;

namespace RoofSheetMaster.Core;

public static class RoofCalculator
{
    // Core calculation used for any single rectangular planar face
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
            // Center of this panel along the eave
            var eavePos = coverage * (i + 0.5);

            result.Panels.Add(new Panel
            {
                Index = i + 1,
                EffectiveWidth = coverage,
                SheetLength = sheetLength,
                EavePosition = eavePos,
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

    // New: tapered face – sheet length changes from StartRun to EndRun along the eave
    public static MaterialList CalculateTaperedFace(TaperedRoofFace face)
    {
        if (face.SheetWidth <= 0)
            throw new ArgumentException("SheetWidth must be > 0");
        if (face.RoofLength <= 0)
            throw new ArgumentException("RoofLength must be > 0");

        var coverage = face.SheetWidth - face.SheetOverlap;
        if (coverage <= 0)
            throw new ArgumentException("Sheet overlap cannot be >= sheet width");

        var sheetCount = (int)Math.Ceiling(face.RoofLength / coverage);
        var angleRad = face.RoofAngleDegrees * Math.PI / 180.0;

        var result = new MaterialList();

        for (int i = 0; i < sheetCount; i++)
        {
            // Center of this panel along the eave (0 at start)
            var eavePos = coverage * (i + 0.5);

            // Normalized position along the eave from 0 to 1
            var t = face.RoofLength > 0 ? eavePos / face.RoofLength : 0.0;
            if (t < 0) t = 0;
            if (t > 1) t = 1;

            // Horizontal run interpolates between StartRun and EndRun
            var horizRun = face.StartRun + t * (face.EndRun - face.StartRun);

            // Apply ridge gap (same idea as rectangular face)
            horizRun = Math.Max(0, horizRun - face.RidgeGap);

            var sheetLength = horizRun / Math.Cos(angleRad);

            result.Panels.Add(new Panel
            {
                Index = i + 1,
                EffectiveWidth = coverage,
                SheetLength = sheetLength,
                EavePosition = eavePos,
                Face = face.Name
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
            inputB.RoofAngleDegrees,   // fixed line
            inputB.SheetWidth,
            inputB.SheetOverlap,
            inputB.RidgeGap,
            "Face B");

        var combined = new MaterialList();
        combined.Panels.AddRange(materialsA.Panels);
        combined.Panels.AddRange(materialsB.Panels);

        return combined;
    }




    // Gable roof overload using RoofFace could be added later if needed.

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
