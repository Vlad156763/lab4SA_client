using Avalonia.Controls;
using Avalonia.Interactivity;


namespace lab4.MainWindowSpace {
    public partial class MainMenu : UserControl {
        private MainWindow? _MainWindow;
        public MainMenu() {
            InitializeComponent();
        }
        public void setMainWindow(MainWindow _var) => _MainWindow = _var;
    }
}