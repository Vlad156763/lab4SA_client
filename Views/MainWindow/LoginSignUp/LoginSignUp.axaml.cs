using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Generic;
using System.Threading.Tasks;
using lab4.Models;
using lab4.Interface;

namespace lab4.MainWindowSpace {
    public partial class LoginSignUp : UserControl {
        IDB DB = new DBComponent();
        ISpin Spin = new SpinComponent();
        IMessage Message = new MessageComponent();
        ILogger Logger = new LoggerComponent();
        IPassword IPAssword = new PasswordComponent();
        private MainWindow? _MainWindow; // головне вікно програми

        public LoginSignUp() {
            InitializeComponent();
        }
        
        public void setMainWindow(MainWindow _var) => _MainWindow = _var;

        // зміна форми (логін/реєстрація)
        private void ChangeFormLogin_SignUp() {
            LoginPanel.IsVisible = RegisterPanel.IsVisible;
            RegisterPanel.IsVisible = !LoginPanel.IsVisible;
        }

        // перехід до форми реєстрації
        private void SwitchToSignUp(object? sender, RoutedEventArgs e)  => ChangeFormLogin_SignUp();

        // перехід до форми входу
        private void SwitchToLogin(object? sender, RoutedEventArgs e) => ChangeFormLogin_SignUp();

        // асинхронна перевірка чи є ім'я користувача і пароль у бд
        private async Task<bool> CheckUser_isLoginAsync(string username, string password) {
            password = IPAssword.Encode(password);
            return await DB.ExecuteBoolQueryAsync("select exists (select 1 from users where username = @username and password = @password);", new Dictionary<string, object> {{"@username", username},{"@password", password}});
        }

        // асинхронна перевірка чи є ім'я користувача у бд
        private async Task<bool> CheckUser_isLoginAsync(string username) {
            return await DB.ExecuteBoolQueryAsync("select exists (select 1 from users where username = @username);", new Dictionary<string, object> { {"@username", username} });
        }

        // кнопка зареєструватись
        private async void SignUp(object? sender, RoutedEventArgs e) {
            var Username = RegisterUserName.Text ?? "";
            var Password = RegisterPassword.Text ?? "";
            var RepeatPassword = RegisterRepeatPassword.Text ?? "";
            RegisterUserName.Classes.Remove("ErrorTextBoxStyle");
            RegisterPassword.Classes.Remove("ErrorTextBoxStyle");
            RegisterRepeatPassword.Classes.Remove("ErrorTextBoxStyle");
            if (string.IsNullOrWhiteSpace(Username)) {
                RegisterUserName.Classes.Add("ErrorTextBoxStyle");
                Message.ShowMessage("Логін не може бути порожнім", LogLevel.Error, TextBlock, BorderBlock);
                return;
            } else if (string.IsNullOrWhiteSpace(Password)) {
                RegisterPassword.Classes.Add("ErrorTextBoxStyle");
                Message.ShowMessage("Пароль не може бути порожнім", LogLevel.Error, TextBlock, BorderBlock);
                return;
            } else if (string.IsNullOrWhiteSpace(RepeatPassword)) {
                RegisterRepeatPassword.Classes.Add("ErrorTextBoxStyle");
                Message.ShowMessage("Пароль не може бути порожнім", LogLevel.Error, TextBlock, BorderBlock);
                return;
            } else if (Password != RepeatPassword) {
                RegisterPassword.Classes.Add("ErrorTextBoxStyle");
                RegisterRepeatPassword.Classes.Add("ErrorTextBoxStyle");
                Message.ShowMessage("Паролі не співпадають", LogLevel.Error, TextBlock, BorderBlock);
                return;
            }
            Message.HideMessage(BorderBlock);
            Spin.StartSpinner(Spinner);
            bool UserInDB = await CheckUser_isLoginAsync(Username);
            Spin. StopSpinner(Spinner);
            if (UserInDB) {
                RegisterUserName.Classes.Add("ErrorTextBoxStyle");
                RegisterPassword.Classes.Add("ErrorTextBoxStyle");
                RegisterRepeatPassword.Classes.Add("ErrorTextBoxStyle");
                Message.ShowMessage("Помилка! Логін вже зареєстрований", LogLevel.Error, TextBlock, BorderBlock);
                return;
            }
            Password = IPAssword.Encode(Password);
            Message.HideMessage(BorderBlock);
            Spin.StartSpinner(Spinner);
            await DB.ExecuteQueryAsync("insert into users (username, password) values (@username, @password);", new Dictionary<string, object> {{"@username", Username},{"@password", Password}});
            Spin.StopSpinner(Spinner);
            Message.ShowMessage("Успішно зареєстровано! Тепер увійдіть", LogLevel.Seccess, TextBlock, BorderBlock);
            LoginUserName.Text = Username;
            LoginPassword.Text = IPAssword.Decode(Password);
            ChangeFormLogin_SignUp();
        }

        // кнопка вхід
        private async void Login(object? sender, RoutedEventArgs e) {
            var Username = LoginUserName.Text ?? "";
            var Password = LoginPassword.Text ?? "";
            LoginUserName.Classes.Remove("ErrorTextBoxStyle");
            LoginPassword.Classes.Remove("ErrorTextBoxStyle");
            if (string.IsNullOrWhiteSpace(Username)) {
                LoginUserName.Classes.Add("ErrorTextBoxStyle");
                Message.ShowMessage("Логін не може бути порожнім", LogLevel.Error, TextBlock, BorderBlock);
                return;
            } else if (string.IsNullOrWhiteSpace(Password)) {
                LoginPassword.Classes.Add("ErrorTextBoxStyle");
                Message.ShowMessage("Пароль не може бути порожнім", LogLevel.Error, TextBlock, BorderBlock);
                return;
            }
            Message.HideMessage(BorderBlock); 
            Spin.StartSpinner(Spinner);
            bool UserInDB = await CheckUser_isLoginAsync(Username, Password);
            
            if (!UserInDB) {
                Spin.StopSpinner(Spinner);
                LoginUserName.Classes.Add("ErrorTextBoxStyle");
                LoginPassword.Classes.Add("ErrorTextBoxStyle");
                Message.ShowMessage("Помилка! Не правельний логін або пароль", LogLevel.Error, TextBlock, BorderBlock);
                return;
            }
            Session.Username = Username;
            Session.Password = Password;
            var menu = await MenuFactory.CreateAsync(_MainWindow);
            if (_MainWindow != null) {
                _MainWindow.SetMainMenu(menu);
            }
            Spin.StopSpinner(Spinner);
            if (_MainWindow != null) {
                _MainWindow.ShowMainMenu();
            } else {
                Logger.error("_MainWindow = null. Метод setMainWindow не було викликано для LoginSignUp");
            }
        }
    }
}