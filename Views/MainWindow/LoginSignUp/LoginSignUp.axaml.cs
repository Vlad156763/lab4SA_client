using Avalonia.Controls;
using Avalonia.Interactivity;
using lab4.Models;
using System.Collections.Generic;
using Avalonia.Media;
using Avalonia.Threading;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using System.Text;


namespace lab4.MainWindowSpace {
    public partial class LoginSignUp : UserControl {
        private MainWindow? _MainWindow; // головне вікно програми
        private DispatcherTimer? _spinnerTimer; // таймер для оновлення спіна

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
        
        //повідомлення
        private void ShowMessage(string msg, bool isError) {
            TextBlock.Text = msg;
            string colorHex = "";
            string bgHex = "";
            if (isError) {
                colorHex = "#FF4B4B";
                bgHex = "#22FF4B4B";
            } else {
                colorHex = "#38FF85";
                bgHex = "#2238FF85";
            }
            TextBlock.Foreground = new SolidColorBrush(Color.Parse(colorHex));
            BorderBlock.Background = new SolidColorBrush(Color.Parse(bgHex));
            BorderBlock.IsVisible = true;
        }
        private void HideMessage() {
            BorderBlock.IsVisible = false;
        }

        // асинхронна перевірка чи є ім'я користувача і пароль у бд
        private async Task<bool> CheckUser_isLoginAsync(string username, string password) {
            password = PasswordEncoder.set(password);
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
                ShowMessage("Логін не може бути порожнім", true);
                return;
            } else if (string.IsNullOrWhiteSpace(Password)) {
                RegisterPassword.Classes.Add("ErrorTextBoxStyle");
                ShowMessage("Пароль не може бути порожнім", true);
                return;
            } else if (string.IsNullOrWhiteSpace(RepeatPassword)) {
                RegisterRepeatPassword.Classes.Add("ErrorTextBoxStyle");
                ShowMessage("Пароль не може бути порожнім", true);
                return;
            } else if (Password != RepeatPassword) {
                RegisterPassword.Classes.Add("ErrorTextBoxStyle");
                RegisterRepeatPassword.Classes.Add("ErrorTextBoxStyle");
                ShowMessage("Паролі не співпадають", true);
                return;
            }
            HideMessage();
            StartSpinner();
            bool UserInDB = await CheckUser_isLoginAsync(Username);
            StopSpinner();
            if (UserInDB) {
                RegisterUserName.Classes.Add("ErrorTextBoxStyle");
                RegisterPassword.Classes.Add("ErrorTextBoxStyle");
                RegisterRepeatPassword.Classes.Add("ErrorTextBoxStyle");
                ShowMessage("Помилка! Логін вже зареєстрований", true);
                return;
            }
            Password = PasswordEncoder.set(Password);
            HideMessage();
            StartSpinner();
            await DB.ExecuteQueryAsync("insert into users (username, password) values (@username, @password);", new Dictionary<string, object> {{"@username", Username},{"@password", Password}});
            StopSpinner();
            ShowMessage("Успішно зареєстровано! Тепер увійдіть", false);
            LoginUserName.Text = Username;
            LoginPassword.Text = PasswordDecoder.set(Password);
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
                ShowMessage("Логін не може бути порожнім", true);
                return;
            } else if (string.IsNullOrWhiteSpace(Password)) {
                LoginPassword.Classes.Add("ErrorTextBoxStyle");
                ShowMessage("Пароль не може бути порожнім", true);
                return;
            }
            HideMessage(); 
            StartSpinner();
            bool UserInDB = await CheckUser_isLoginAsync(Username, Password);
            StopSpinner();
            if (!UserInDB) {
                LoginUserName.Classes.Add("ErrorTextBoxStyle");
                LoginPassword.Classes.Add("ErrorTextBoxStyle");
                ShowMessage("Помилка! Не правельний логін або пароль", true);
                return;
            }
            if (_MainWindow != null) {
                Session.Username = Username;
                Session.Password = Password;
                _MainWindow.ShowMainMenu();
            } else {
                Logger.error("_MainWindow = null. Метод setMainWindow не було викликано для LoginSignUp");
            }
        }

        // запуск крутіння спіна
        private void StartSpinner() {
            Spinner.IsVisible = true;
            _spinnerTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
            if (Spinner.RenderTransform != null) {
                var transform = (RotateTransform)Spinner.RenderTransform;
                _spinnerTimer.Tick += (_, _) => {
                    if(transform != null)
                        transform.Angle = (transform.Angle + 4) % 360;
                };
            }
            _spinnerTimer.Start();
        }
        
        //зупитка спіна
        private void StopSpinner() {
            Spinner.IsVisible = false;
            _spinnerTimer?.Stop();
            _spinnerTimer = null;
        }
    }
}