using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using lab4.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using lab4.Interface;
namespace lab4.BuyStatementWindow;


public partial class BuyStatement : Window {
    IDB DB = new DBComponent();
    ISpin Spin = new SpinComponent();
    IMessage Message = new MessageComponent();
    ILogger Logger = new LoggerComponent();
    private object[]? product;
    public BuyStatement() {
        InitializeComponent();
    }
    public void SetProduct(object[] product) {
        this.product = product;
    }
    private async void CreateStatement(object sender, RoutedEventArgs e) {
        
        PriceProduct.Classes.Remove("ErrorTextBoxStyle");
        AmountProduct.Classes.Remove("ErrorTextBoxStyle");
        if (product == null) {
            Logger.error("product == null"); 
            return;
        }
        var intPriceProdaveh = Int32.Parse(product[2].ToString() ?? "");
        var intAmountProdaveh = Int32.Parse(product[3].ToString() ?? "");

        if (string.IsNullOrWhiteSpace(PriceProduct.Text)) {
            PriceProduct.Classes.Add("ErrorTextBoxStyle");
            Message.ShowMessage("Ціна не може бути пустою", LogLevel.Error, TextBlock, BorderBlock);
            return;
        } else if (string.IsNullOrWhiteSpace(AmountProduct.Text)) {
            AmountProduct.Classes.Add("ErrorTextBoxStyle");
            Message.ShowMessage("Кількість не може бути пустою", LogLevel.Error, TextBlock, BorderBlock);
            return;
        }
        var intPriceBuyer = Int32.Parse(PriceProduct.Text ?? "");
        var intAmountBuyer = Int32.Parse(AmountProduct.Text ?? "");

        if (intPriceProdaveh >= intPriceBuyer) {
            PriceProduct.Classes.Add("ErrorTextBoxStyle");
            Message.ShowMessage("Ставка менша за вказану продавцем", LogLevel.Error, TextBlock, BorderBlock);
            return;
        } else if (intAmountBuyer > intAmountProdaveh) {
            PriceProduct.Classes.Add("ErrorTextBoxStyle");
            Message.ShowMessage("Вказано товару, більше, ніж є у продавця", LogLevel.Error, TextBlock, BorderBlock);
            return;
        }
        Message.HideMessage(BorderBlock);
        Spin.StartSpinner(Spinner);
        //вставка інфи у bids
        await DB.ExecuteQueryAsync("insert into bids ( statements_id, buyer_id, amount, price) values ( @st_id, (select id from users where username = @br_id), @amount, @price);", new Dictionary<string, object> {
            {"st_id", Int32.Parse(product[0].ToString() ?? "")},
            {"br_id", Session.Username},
            {"amount", intAmountBuyer},
            {"price", intPriceBuyer}
        });
        Spin.StopSpinner(Spinner);
        this.Close();
    }
}