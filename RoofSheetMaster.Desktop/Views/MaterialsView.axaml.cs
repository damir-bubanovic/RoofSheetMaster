using Avalonia.Controls;

namespace RoofSheetMaster.Desktop.Views;

public partial class MaterialsView : UserControl
{
    public MaterialsView()
    {
        InitializeComponent();
    }

    // Expose the internal list boxes so MainWindow can bind to them.
    public ListBox PanelsListBoxControl => PanelListBox;
    public ListBox SheetSummaryListBoxControl => SheetSummaryListBox;
    public ListBox FlashingListBoxControl => FlashingListBox;
}
