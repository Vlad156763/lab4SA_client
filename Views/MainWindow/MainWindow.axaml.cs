using Avalonia.Controls;
using lab4.MainWindowSpace;

namespace lab4 {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            ShowAutentification();
        }
        public void ShowMainMenu() {
            var view = new MainMenu();
            view.setMainWindow(this);
            Autentification.Content = view;
        }
        public void ShowAutentification() {
            var view = new LoginSignUp();
            view.setMainWindow(this);
            Autentification.Content = view;
        }
    }
}