using Avalonia.Controls;
using lab4.MainWindowSpace;

namespace lab4 {
    public partial class MainWindow : Window {
        private UserControl? _mainMenu;
        public MainWindow() {
            InitializeComponent();
            ShowAutentification();
        }
        public void ShowMainMenu() {
            Width = 800;
            Height = 800;
            if (_mainMenu != null)
                Autentification.Content = _mainMenu;
            else {
                var view = new MainMenu();
                view.setMainWindow(this);
                Autentification.Content = view;
            }
        }
        public void SetMainMenu(UserControl menu) {
            _mainMenu = menu;
        }
        public void ShowAutentification() {
            Width=450;
            Height=650;
            var view = new LoginSignUp();
            view.setMainWindow(this);
            Autentification.Content = view;
        }
    }
}