using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia;
using lab4.Models;
using Avalonia.Layout;
using System.Threading.Tasks;
using lab4.CreateStatementWindowSpace;
using lab4.HistoryWindowSpace;

namespace lab4.MainWindowSpace {
    public partial class MainMenu : UserControl {
        private MainWindow? _MainWindow;

        public  MainMenu() {
                InitializeComponent();
                this.Loaded += async (_, __) => {await UpdateStatementsAsync(); };
        }
        public void setMainWindow(MainWindow _var) => _MainWindow = _var;

        public async Task UpdateStatementsAsync() {
            Spin.StartSpinner(UpSpinner);
            var result = await DB.ExecuteQueryResultAsync("select s.id, s.name, s.price, s.amount, s.measurement, u.username as login from statements s join users u on s.id_user = u.id;");
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
            Spin.StopSpinner(UpSpinner);
            UpSpinner.IsVisible = true;
        }


        private async void UpdateButton(object sender, RoutedEventArgs e) {
            await UpdateStatementsAsync();
        }
        private void ShowProduct(object[] product) {
            Logger.debug($"name: {product[1]}");
            Logger.debug($"price: {product[2]}");
            Logger.debug($"amount: {product[3]} {product[4]}");
            Logger.debug($"id_user: {product[5]}");

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
            }
        }
        private async void History(object sender, RoutedEventArgs e) {
            var NewWindow = new HistoryWindow();
            if(_MainWindow != null) {
                await NewWindow.ShowDialog(_MainWindow);
            }
        }
    }
}