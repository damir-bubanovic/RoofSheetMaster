namespace RoofSheetMaster.Core
{
    /// <summary>
    /// Summary for flashings / accessories in core.
    /// Length is in the same units as panel lengths.
    /// </summary>
    public class FlashingSummary
    {
        /// <summary>
        /// Name / type, e.g. "Ridge cap", "Barge / verge", "Valley tray", "Screws".
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Total running length (for things measured in length, e.g. ridge, barge, valley).
        /// For items like screws, this will be 0 and Count will be used instead.
        /// </summary>
        public double TotalLength { get; set; }

        /// <summary>
        /// Optional count (e.g. number of screws).
        /// For pure length items, this can stay 0.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Extra info for the user (e.g. "approx. 7 screws / m²").
        /// </summary>
        public string Notes { get; set; } = string.Empty;
    }
}
