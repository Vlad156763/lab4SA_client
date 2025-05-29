using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia;
using Avalonia.Layout;

using lab4.Models;
using lab4.CreateStatementWindowSpace;
using lab4.HistoryWindowSpace;
using lab4.BuyStatementWindow;
using lab4.NotificationsWindowSpase;
using lab4.Interface;

using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;


namespace lab4.MainWindowSpace {
    public partial class MainMenu : UserControl {
        IDB DB = new DBComponent();
        ISpin Spin = new SpinComponent();
        IMessage Message = new MessageComponent();
        ILogger Logger = new LoggerComponent();
        
        private MainWindow? _MainWindow;
        CancellationTokenSource? loadingToken;
        public  MainMenu() {
            InitializeComponent();
        }
        public void setMainWindow(MainWindow _var) => _MainWindow = _var;
        public void setAdminButton() {
            Admin.IsVisible = true;
        }

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
        private async void ShowProduct(object[] product) {
            var buyWindow = new BuyStatement();
            buyWindow.SetProduct(product);
            if (_MainWindow != null) {
                await buyWindow.ShowDialog(_MainWindow);
            }
            await UpdateStatementsAsync();
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
        private async Task Loading( CancellationToken token) {
            int i = 0;
            string[] loading = { "[|]", "[/]","[-]", "[\\]"};

            while(!token.IsCancellationRequested) {
                i = (i + 1) % loading.Length;
                Message.ShowMessage("Почекайте " + loading[i], LogLevel.Debug, TextBlock, BorderBlock);
                await Task.Delay(300);
            }
        }
        private async void AdminButtonPressed(object sender, RoutedEventArgs e) {
            if(_MainWindow != null)
                _MainWindow.IsEnabled = false;
            loadingToken = new CancellationTokenSource();
            var loadingTask = Loading(loadingToken.Token);
            
            /*
            1. отримати з бд всі statements
            2. для n-ї заявки у statements, отримати таблицю покупців з bids
            3. для кожного покупця з bids виписати скільки він пропонує грошей за одиницю товару
            4. віднімати від кількості загального товару, кількість товару яку хочуть покупці доти, доки не буде 0
            5. якщо у п. 4 вийшло 0, для поточної заявки змінюю поле is_active на false
            6. віднімаю від amount стільки товару скільки хочуть покупці (змінюю це значення у statements.amount)
            7. створити у таблиці  notifications поля для тих покупців, які запропонували найбільше, і можуть отримати товари згідно з п. 4-6
            
            */
            await DB.ExecuteQueryAsync("delete from notifications; ALTER SEQUENCE notifications_id_seq RESTART WITH 1;");
            var stAll = await DB.ExecuteQueryResultAsync("select * from statements where is_active = true;");
            
            foreach(var cSt in stAll) { //cSt - n заявка
                var idCurrentStatement = Int32.Parse(cSt[0].ToString()??"");
                var amountCurrentStatement = Int32.Parse(cSt[3].ToString()??"");
                
                var tb_bids = await DB.ExecuteQueryResultAsync("select * from bids where statements_id = @st_id;", new Dictionary<string, object> {
                    {"st_id", idCurrentStatement}
                });
                var profits = new List<(int buyerIndex, int profit, int price)>();

                for (int i = 0; i < tb_bids.Count; i++) {
                    int price = Int32.Parse(tb_bids[i][4].ToString() ?? "");
                    int amount = Int32.Parse(tb_bids[i][3].ToString() ?? "");
                    int profit = price;
                    profits.Add((i, profit, amount * price));
                }
                profits = profits.OrderByDescending(p => p.profit).ToList();
                int i_ = 0;
                for (; i_ < profits.Count; i_++) {
                    if (amountCurrentStatement == 0) {
                        //якщо юзерів більше 
                        await DB.ExecuteQueryAsync("update statements set is_active = false where id = @id;", new Dictionary<string, object>{{"id", cSt[0]}});
                        break;
                    };
                    amountCurrentStatement -= Int32.Parse( tb_bids[profits[i_].buyerIndex][3].ToString() ?? "");
                }
                for (int i = 0; i < i_; i++) {
                    await DB.ExecuteQueryAsync("insert into notifications (user_id, name_product, statements_id, bids_id, price) values (@idUsr, @name, @idSt, @idBids, @price);", new Dictionary<string, object> {
                        {"idUsr", tb_bids[profits[i].buyerIndex][2]},
                        {"name", cSt[1]},
                        {"idSt", cSt[0]},
                        {"idBids", tb_bids[profits[i].buyerIndex][0]},
                        {"price", profits[i].price}
                    });
                }
            }
            loadingToken.Cancel();
            await loadingTask;
            if(_MainWindow != null)
                _MainWindow.IsEnabled = true;
            Message.ShowMessage("Готово", LogLevel.Debug, TextBlock, BorderBlock);
        }
        private void PressedClose(object sender, RoutedEventArgs e) {
            Message.HideMessage(BorderBlock);
        }
        private async void Notifications(object sender, RoutedEventArgs e) {
            Spin.StartSpinner(SpinnerDown);
            var preloadWindow = await NotificationsFactory.CreateAsync(_MainWindow);
            Spin.StopSpinner(SpinnerDown);
            if(_MainWindow != null) {
                await preloadWindow.ShowDialog(_MainWindow);
                Spin.StartSpinner(SpinnerDown);
                await preloadWindow.UpdateNotificationsAsync();
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
            //адмін
            if(Session.Username == "vlad") {
                menu.setAdminButton();
            }
            await menu.UpdateStatementsAsync(false); // це preload
            return menu;
        }
    }
}