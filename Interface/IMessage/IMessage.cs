using Avalonia.Controls;
using lab4.Models;

namespace lab4.Interface;
public interface IMessage {
    void ShowMessage(string msg, LogLevel type, TextBlock TextBlock, Border BorderBlock );
    void HideMessage(Border BorderBlock);
}