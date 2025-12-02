using System;
using System.Collections.Generic;
using System.Linq;

namespace RoofSheetMaster.Core
{
    public static class FlashingCalculator
    {
        /// <summary>
        /// Produce approximate flashing lengths and screw count
        /// based only on panel layout.
        /// This is deliberately simple for Chapter 9.
        /// </summary>
        public static List<FlashingSummary> CalculateFlashings(MaterialList materials)
        {
            var results = new List<FlashingSummary>();

            if (materials == null || materials.Panels.Count == 0)
                return results;

            // Group by face name (single-face panels get "Face")
            var groups = materials
                .Panels
                .GroupBy(p => string.IsNullOrWhiteSpace(p.Face) ? "Face" : p.Face!)
                .ToList();

            //
            // 1) Ridge cap – very rough approximation:
            // if we have at least two faces, take the smallest "max sheet length"
            // as an approximate ridge length.
            //
            if (groups.Count >= 2)
            {
                var maxLengths = groups
                    .Select(g => g.Max(p => p.SheetLength))
                    .ToList();

                var ridgeLength = maxLengths.Min();

                if (ridgeLength > 0)
                {
                    results.Add(new FlashingSummary
                    {
                        Name = "Ridge cap",
                        TotalLength = ridgeLength,
                        Notes = "Approximate main ridge length."
                    });
                }
            }

            //
            // 2) Barge / verge flashings – for each face, one barge equal to max sheet length.
            //
            foreach (var g in groups)
            {
                var bargeLength = g.Max(p => p.SheetLength);
                if (bargeLength <= 0)
                    continue;

                results.Add(new FlashingSummary
                {
                    Name = $"Barge / verge ({g.Key})",
                    TotalLength = bargeLength,
                    Notes = "Approximate barge length for this face."
                });
            }

            //
            // 3) Valley trays – if we have faces named Upper/Lower, approximate one valley.
            //
            var valleyGroups = groups
                .Where(g => g.Key.Contains("Upper", StringComparison.OrdinalIgnoreCase) ||
                            g.Key.Contains("Lower", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (valleyGroups.Count >= 2)
            {
                var valleyMaxLengths = valleyGroups
                    .Select(g => g.Max(p => p.SheetLength))
                    .ToList();

                var valleyLength = valleyMaxLengths.Min();

                if (valleyLength > 0)
                {
                    results.Add(new FlashingSummary
                    {
                        Name = "Valley tray",
                        TotalLength = valleyLength,
                        Notes = "Approximate valley tray length (simplified)."
                    });
                }
            }

            //
            // 4) Screws / fasteners – approximate from total panel area.
            //
            const double screwsPerArea = 7.0; // very rough rule of thumb

            var totalArea = materials.Panels.Sum(p => p.EffectiveWidth * p.SheetLength);
            var screwCount = (int)Math.Ceiling(totalArea * screwsPerArea);

            if (screwCount > 0)
            {
                results.Add(new FlashingSummary
                {
                    Name = "Screws / fasteners",
                    Count = screwCount,
                    Notes = $"Approx. {screwsPerArea:0} per area unit based on panel coverage."
                });
            }

            return results;
        }
    }
}
