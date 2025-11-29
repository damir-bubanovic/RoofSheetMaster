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

        var materials = RoofCalculator.CalculateSimpleFace(input);

        Console.WriteLine($"Total sheets: {materials.TotalSheets}");
        foreach (var p in materials.Panels)
        {
            Console.WriteLine($"Panel {p.Index}: cover = {p.EffectiveWidth:F3}, length = {p.SheetLength:F3}");
        }
    }
}
