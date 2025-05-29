using System.Collections.Generic;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Controls;
using System.Threading.Tasks;
using lab4.Models;
using lab4.Interface;

namespace lab4.NotificationsWindowSpase {
    public partial class NotificationsWindow : Window {
        IDB DB = new DBComponent();
        ISpin Spin = new SpinComponent();
        private MainWindow? _MainWindow;
        public NotificationsWindow() {
            InitializeComponent();
        }
        public void setMainWindow(MainWindow _MainWindow) {
            this._MainWindow = _MainWindow;
        }
        public async Task UpdateNotificationsAsync(bool SpinnerStart=true) {
            /*
            1. зробити запит до бд щоб отримати всі повідомлення для Session.Username
            2. додати GUI щоб юзер міг "скасувати заявку" "підтвердити заявку"
            3. коли натиснуто "підтвердити заявку у statements від amount віднімається bids_id -> bids.amount, та з bids видаляється поточний юзер (слід правильно його видалити (і по id_user і по id_bids перевіряти)). Також у statements додається profit з notifications.price
            4. Коли натиснуто "скасувати заявку" з bids видаляється ставка і у statements.is_active == true 
            5. TODO: незабути додати оновлення statements.is_active коли видалення відбувається з historyWindow
            */
            if(SpinnerStart) {
                Spin.StartSpinner(Spinner);
            }
            var result = await DB.ExecuteQueryResultAsync("select * from notifications where user_id = (select id from users where username = @username);", new Dictionary<string, object> {{"username", Session.Username}});
            BlocksPanel.Children.Clear();
        foreach (var row in result) {
            var block = new Border {
                Background = new SolidColorBrush(Color.FromRgb(70, 70, 70)),
                Margin = new Thickness(0, 5, 0, 5),
                Padding = new Thickness(10),
                Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
            };
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // назва
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star)); // ціна
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // підтвердити
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // скасувати

            var nameText = new TextBlock {
                Text = row[2].ToString(),
                FontSize = 16,
                VerticalAlignment = VerticalAlignment.Center
            };
            var priceText = new TextBlock {
                Text = new string("До сплати: " + row[5].ToString()),
                FontSize = 16,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Right
            };
            var deleteButton = new Button {
                Content = "Скасувати",
                FontSize = 16,
                Background = new SolidColorBrush(Color.FromArgb(0x22, 0xFF, 0x4B, 0x4B)),
                Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0x4B, 0x4B)),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 10, 0)
            };
            var ConfirmButton = new Button {
                Content = "Підтвердити",
                FontSize = 16,
                Background = new SolidColorBrush(Color.FromArgb(0x22, 0x38, 0xFF, 0x85)),
                Foreground = new SolidColorBrush(Color.FromRgb(0x38, 0xFF, 0x85)),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            deleteButton.Click += async (s,args) => {
                Spin.StartSpinner(Spinner);
                await DB.ExecuteQueryAsync("delete from bids where id = @id;", new Dictionary<string, object> {{"id", row[4]}});
                await DB.ExecuteQueryAsync("delete from notifications where id = @id;", new Dictionary<string, object> {{"id", row[0]}});
                await DB.ExecuteQueryAsync("update statements set is_active = true where id = @id", new Dictionary<string, object> {{"id", row[3]}});
                Spin.StopSpinner(Spinner);
                await UpdateNotificationsAsync();
            };
            ConfirmButton.Click += async (s,args) => {
                Spin.StartSpinner(Spinner);
                await DB.ExecuteQueryAsync("update statements set amount = amount - (select amount from bids where id = @id_bids)::integer, profit = profit + @profit where id = @id", new Dictionary<string, object> {{"id",row[3] }, {"id_bids", row[4]}, {"profit", row[5]}});
                await DB.ExecuteQueryAsync("delete from notifications where id = @id;", new Dictionary<string, object> {{"id", row[0]}});
                await DB.ExecuteQueryAsync("delete from bids where id = @id;", new Dictionary<string, object> {{"id", row[4]}});
                Spin.StopSpinner(Spinner);
                await UpdateNotificationsAsync();
            };
            Grid.SetColumn(nameText, 0);
            Grid.SetColumn(priceText, 1);
            Grid.SetColumn(deleteButton, 2);
            Grid.SetColumn(ConfirmButton, 3);

            grid.Children.Add(nameText);
            grid.Children.Add(priceText);
            grid.Children.Add(deleteButton);
            grid.Children.Add(ConfirmButton);

            block.Child = grid;

            BlocksPanel.Children.Add(block);
        }
            if(SpinnerStart) {
                Spin.StopSpinner(Spinner);
            }
        }
    }

    
    public static class NotificationsFactory {
        public static async Task<NotificationsWindow> CreateAsync(MainWindow? mw) {
            var menu = new NotificationsWindow();
            if(mw != null) {
                menu.setMainWindow(mw);
            }
            await menu.UpdateNotificationsAsync(false); // це preload
            return menu;
        }
    }
}