﻿using System;
using RoofSheetMaster.Core;

class Program
{
    static void Main()
    {
        var input = new RoofInput
        {
            RoofLength = 40.0,
            RoofWidth = 15.0,
            RoofAngleDegrees = 26.565,
            SheetWidth = 3.0,
            SheetOverlap = 0.125,
            RidgeGap = 0.0
        };

        Console.WriteLine("=== Single Face ===");
        var singleFace = RoofCalculator.CalculateSimpleFace(input);
        Console.WriteLine($"Total sheets (single face): {singleFace.TotalSheets}");
        foreach (var p in singleFace.Panels)
        {
            Console.WriteLine($"Panel {p.Index}: cover = {p.EffectiveWidth:F3}, length = {p.SheetLength:F3}");
        }

        Console.WriteLine();
        Console.WriteLine("=== Gable Roof (two faces, same dimensions) ===");
        var gable = RoofCalculator.CalculateGableRoof(input);
        Console.WriteLine($"Total sheets (gable): {gable.TotalSheets}");
        foreach (var p in gable.Panels)
        {
            Console.WriteLine($"{p.Face} - Panel {p.Index}: cover = {p.EffectiveWidth:F3}, length = {p.SheetLength:F3}");
        }

        Console.WriteLine();
        Console.WriteLine("=== Single Face via RoofFace ===");

        var face = new RoofFace
        {
            Name = "Front",
            RoofLength = 40.0,
            RoofWidth = 15.0,
            RoofAngleDegrees = 26.565,
            SheetWidth = 3.0,
            SheetOverlap = 0.125,
            RidgeGap = 0.0
        };

        var faceResult = RoofCalculator.CalculateFace(face);
        Console.WriteLine($"Total sheets ({face.Name}): {faceResult.TotalSheets}");
        foreach (var p in faceResult.Panels)
        {
            Console.WriteLine($"{p.Face} - Panel {p.Index}: cover = {p.EffectiveWidth:F3}, length = {p.SheetLength:F3}");
        }

        Console.WriteLine();
        Console.WriteLine("=== Hip Roof (4 faces, same dimensions for now) ===");

        var hipRoof = new HipRoof
        {
            FrontLeft = new RoofFace
            {
                Name = "FrontLeft",
                RoofLength = 40.0,
                RoofWidth = 15.0,
                RoofAngleDegrees = 26.565,
                SheetWidth = 3.0,
                SheetOverlap = 0.125,
                RidgeGap = 0.0
            },
            FrontRight = new RoofFace
            {
                Name = "FrontRight",
                RoofLength = 40.0,
                RoofWidth = 15.0,
                RoofAngleDegrees = 26.565,
                SheetWidth = 3.0,
                SheetOverlap = 0.125,
                RidgeGap = 0.0
            },
            BackLeft = new RoofFace
            {
                Name = "BackLeft",
                RoofLength = 40.0,
                RoofWidth = 15.0,
                RoofAngleDegrees = 26.565,
                SheetWidth = 3.0,
                SheetOverlap = 0.125,
                RidgeGap = 0.0
            },
            BackRight = new RoofFace
            {
                Name = "BackRight",
                RoofLength = 40.0,
                RoofWidth = 15.0,
                RoofAngleDegrees = 26.565,
                SheetWidth = 3.0,
                SheetOverlap = 0.125,
                RidgeGap = 0.0
            }
        };

        var hipResult = RoofCalculator.CalculateHipRoof(hipRoof);
        Console.WriteLine($"Total sheets (hip roof): {hipResult.TotalSheets}");

        foreach (var p in hipResult.Panels)
        {
            Console.WriteLine($"{p.Face} - Panel {p.Index}: cover = {p.EffectiveWidth:F3}, length = {p.SheetLength:F3}");
        }
    }
}
