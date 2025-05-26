using Avalonia.Controls;
using lab4.Models;
using lab4.MainWindowSpace;
using System.Threading.Tasks;

namespace lab4 {
    public partial class MainWindow : Window {
        public MainWindow(string login, string password) {
            InitializeComponent();
            Session.Password = password;
            Session.Username = login;
            Logger.debug(login);
            Logger.debug(password);
            ShowMainMenu();
        }
        public MainWindow() {
            InitializeComponent();
            ShowAutentification();
        }
        public void ShowMainMenu() {
            Width = 800;
            Height = 800;
            var view = new MainMenu();
            view.setMainWindow(this);
            Autentification.Content = view;
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