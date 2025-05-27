using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia;
using lab4.Models;
using Avalonia.Layout;
using System.Threading.Tasks;
using lab4.CreateStatementWindowSpace;
using lab4.HistoryWindowSpace;
using lab4.BuyStatementWindow;
using System.Collections.Generic;

namespace lab4.MainWindowSpace {
    public partial class MainMenu : UserControl {
        private MainWindow? _MainWindow;

        public  MainMenu() {
                InitializeComponent();
        }
        public void setMainWindow(MainWindow _var) => _MainWindow = _var;

        public async Task UpdateStatementsAsync(bool SpinerStart=true) {
            if(SpinerStart) {
                Spin.StartSpinner(UpSpinner);
            }
            var result = await DB.ExecuteQueryResultAsync("select s.id, s.name, s.price, s.amount, s.measurement, u.username as login from statements s  join users u on s.id_user = u.id where s.is_active = true and s.id_user != (select id from users where username = @username);", new Dictionary<string, object> {{"username", Session.Username}});
            BlocksPanel.Children.Clear();
            foreach (var row in result) {
                var block = new Border {
                    Background = new SolidColorBrush(Color.FromRgb(70, 70, 70)),
                    Margin = new Thickness(0, 5, 0, 5),
                    Padding = new Thickness(10),
                    Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
                };
                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star)); // назва
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // автор
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star)); // ціна
                var nameText = new TextBlock {
                    Text = row[1].ToString(),
                    FontSize = 16,
                    VerticalAlignment = VerticalAlignment.Center
                };
                var priceText = new TextBlock {
                    Text = row[2].ToString(),
                    FontSize = 16,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    TextAlignment = TextAlignment.Right
                };
                var avtor = new TextBlock {
                    Text = row[5].ToString(),
                    FontSize = 16,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TextAlignment = TextAlignment.Center
                };
                Grid.SetColumn(nameText, 0);
                Grid.SetColumn(avtor, 1);
                Grid.SetColumn(priceText, 2);
                grid.Children.Add(nameText);
                grid.Children.Add(avtor);
                grid.Children.Add(priceText);
                block.Child = grid;
                block.PointerPressed += (s, args) => {
                    ShowProduct(row);
                };
                BlocksPanel.Children.Add(block);
            }
            if(SpinerStart) {
                Spin.StopSpinner(UpSpinner);
            }
            UpSpinner.IsVisible = true;
        }


        private async void UpdateButton(object sender, RoutedEventArgs e) {
            Spin.StopSpinner(UpSpinner);
            await UpdateStatementsAsync();
        }
        private void ShowProduct(object[] product) {
            Logger.debug($"name: {product[1]}");
            Logger.debug($"price: {product[2]}");
            Logger.debug($"amount: {product[3]} {product[4]}");
            Logger.debug($"id_user: {product[5]}");
            var buyWindow = new BuyStatement();
            buyWindow.SetProduct(product);
            if (_MainWindow != null)
                buyWindow.ShowDialog(_MainWindow);
        }
        private void ExitToSignUp(object sender, RoutedEventArgs e) {
            Session.Username = "";
            Session.Password = "";
            if (_MainWindow != null) {
                _MainWindow.ShowAutentification();
            } else {
                Logger.error("_MainWindow == null");
            }
        }
        private async void CreateNewStatementPurch(object sender, RoutedEventArgs e) {
            var NewWindow = new CreateStatementWindow();
            if(_MainWindow != null) {
                await NewWindow.ShowDialog(_MainWindow);
                Spin.StartSpinner(SpinnerDown);
                await UpdateStatementsAsync();
                Spin.StopSpinner(SpinnerDown);
            }
        }
        private async void History(object sender, RoutedEventArgs e) {
            Spin.StartSpinner(SpinnerDown);
            var preloadWindow = await HistoryFactory.CreateAsync();
            Spin.StopSpinner(SpinnerDown);
            if(_MainWindow != null) {
                await preloadWindow.ShowDialog(_MainWindow);
                Spin.StartSpinner(SpinnerDown);
                await UpdateStatementsAsync();
                Spin.StopSpinner(SpinnerDown);
            }
        }
    }
    public static class MenuFactory {
        public static async Task<MainMenu> CreateAsync(MainWindow? mw) {
            var menu = new MainMenu();
            if(mw != null) {
                menu.setMainWindow(mw);
            }
            await menu.UpdateStatementsAsync(false); // це preload
            return menu;
        }
    }
}