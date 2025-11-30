using System;
using System.Collections.Generic;
using System.Linq;

namespace RoofSheetMaster.Core;

public class MaterialList
{
    /// <summary>
    /// All individual panels calculated for the roof.
    /// </summary>
    public List<Panel> Panels { get; set; } = new();

    /// <summary>
    /// Convenience count of all panels/sheets.
    /// </summary>
    public int TotalSheets => Panels.Count;

    /// <summary>
    /// Summarised sheet list grouped by (rounded) sheet length.
    /// This is what you would normally hand to the supplier:
    /// e.g. "16.771 ft × 28 pcs".
    /// </summary>
    public List<SheetSummary> SheetSummaries =>
        Panels
            // group by sheet length rounded to 3 decimals to avoid
            // tiny floating point differences creating separate groups
            .GroupBy(p => Math.Round(p.SheetLength, 3))
            .OrderByDescending(g => g.Key)
            .Select(g => new SheetSummary
            {
                SheetLength = g.First().SheetLength,
                Count = g.Count()
            })
            .ToList();
}
