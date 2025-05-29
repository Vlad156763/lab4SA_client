using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Generic;
using lab4.Models;
using System;
using lab4.Interface;
namespace lab4.CreateStatementWindowSpace;

public partial class CreateStatementWindow : Window {
    IDB DB = new DBComponent();
    ISpin Spin = new SpinComponent();
    IMessage Message = new MessageComponent();
    public CreateStatementWindow() {
        InitializeComponent();
    }
    private async void CreateStatement(object? sender, RoutedEventArgs e) {
        string name = NameProduct.Text ?? "";
        string amount = AmountProduct.Text ?? "";
        string price = PriceProduct.Text ?? "";
        string measurement = MeasurementProduct.Text ?? "";

        NameProduct.Classes.Remove("ErrorTextBoxStyle");
        AmountProduct.Classes.Remove("ErrorTextBoxStyle");
        PriceProduct.Classes.Remove("ErrorTextBoxStyle");
        MeasurementProduct.Classes.Remove("ErrorTextBoxStyle");
        if (string.IsNullOrWhiteSpace(name)) {
            NameProduct.Classes.Add("ErrorTextBoxStyle");
            Message.ShowMessage("Назва товару не може бути пустою", LogLevel.Error, TextBlock, BorderBlock);
            return;
        } else if (string.IsNullOrWhiteSpace(amount)) {
            AmountProduct.Classes.Add("ErrorTextBoxStyle");
            Message.ShowMessage("Кількість товару не може бути пустим", LogLevel.Error, TextBlock, BorderBlock);
            return;
        } else if (string.IsNullOrWhiteSpace(price)) {
            PriceProduct.Classes.Add("ErrorTextBoxStyle");
            Message.ShowMessage("Ціна не може бути пустою", LogLevel.Error, TextBlock, BorderBlock);
            return;
        } else if (string.IsNullOrWhiteSpace(measurement)) {
            MeasurementProduct.Classes.Add("ErrorTextBoxStyle");
            Message.ShowMessage("Одиниці вимінювання не може бути пустими", LogLevel.Error, TextBlock, BorderBlock);
            return;
        }
        Message.HideMessage(BorderBlock);
        Spin.StartSpinner(Spinner);
        await DB.ExecuteQueryAsync(
            "insert into statements(name, price, amount, measurement, id_user) " +
            "values (@name, @price, @amount, @measurement, (select id from users where username = @username) );" , 
            new Dictionary<string, object> {
                {"@name", name},
                {"@price", price},
                {"@amount", Int32.Parse(amount)},
                {"@measurement", measurement},
                {"@username", Session.Username}
            }
        );
        Spin.StopSpinner(Spinner);
        Close();
    }
}
