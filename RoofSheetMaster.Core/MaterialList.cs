using System.Collections.Generic;

namespace RoofSheetMaster.Core;

public class MaterialList
{
    public List<Panel> Panels { get; set; } = new();
    public int TotalSheets => Panels.Count;
}
