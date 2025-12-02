using Avalonia.Controls;

namespace RoofSheetMaster.Desktop;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Load last-used inputs (if available) and auto-calculate
        TryLoadLastProjectSilently();

        // Persist current inputs on close
        this.Closing += OnMainWindowClosing;
    }
}
