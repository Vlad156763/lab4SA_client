using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia;
using Avalonia.Layout;
using lab4.Models;
using System.Collections.Generic;


namespace lab4.HistoryWindowSpace;
public partial class HistoryWindow : Window {
    bool is_createdHistory = true;
    public HistoryWindow() {
        InitializeComponent();
        ButtonHistoryName.Content = "Оновити";
    }
    private async void BlocksHistory() {
        Spin.StartSpinner(Spinner);
        var result = await DB.ExecuteQueryResultAsync("select * from statements where id_user = (select id from users where username = @username);", new Dictionary<string, object> {{"username", Session.Username}});
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
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // ціна
            var nameText = new TextBlock {
                Text = row[1].ToString(),
                FontSize = 16,
                VerticalAlignment = VerticalAlignment.Center
            };
            var priceText = new TextBlock {
                Text = row[2].ToString(),
                FontSize = 16,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Right
            };
            var deleteButton = new Button {
                Content = "Видалити",
                FontSize = 16,
                Background = new SolidColorBrush(Color.FromArgb(0x22, 0xFF, 0x4B, 0x4B)),
                Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0x4B, 0x4B)),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            deleteButton.Click += async (s,args) => {
                Spin.StopSpinner(Spinner);
                Spin.StartSpinner(Spinner);
                await DB.ExecuteQueryAsync("delete from statements where id = @id;", new Dictionary<string, object> {{"@id", row[0]}});
                Spin.StopSpinner(Spinner);
                BlocksHistory();
            };
            Grid.SetColumn(nameText, 0);
            Grid.SetColumn(deleteButton, 2);
            Grid.SetColumn(priceText, 1);

            grid.Children.Add(nameText);
            grid.Children.Add(deleteButton);
            grid.Children.Add(priceText);
            block.Child = grid;

            BlocksPanel.Children.Add(block);
        }
        Spin.StopSpinner(Spinner);
    }
    private void ButtonHistory(object sender, RoutedEventArgs e) {
        is_createdHistory = !is_createdHistory;
        Spin.StopSpinner(Spinner);
        BlocksPanel.Children.Clear();
        if (is_createdHistory) {
            ButtonHistoryName.Content = "Переглянути історію куплених заявок";
            BlocksHistory();
        } else {
            ButtonHistoryName.Content = "Переглянути історію створених заявок";
        }
    }
}
