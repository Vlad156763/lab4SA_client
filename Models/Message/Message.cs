using Avalonia.Controls;
using Avalonia.Media;
namespace lab4.Models {
    public static class Message {
        public enum LogLevel {
            Debug,
            Warning,
            Error,
            Seccess
        }
        public static void ShowMessage(string msg, LogLevel type, TextBlock TextBlock, Border BorderBlock ) {
            TextBlock.Text = msg;
            string colorHex = "";
            string bgHex = "";
            if (type == LogLevel.Debug) {
                colorHex = "#2196F3";
                bgHex = "#222196F3";
            } else if (type == LogLevel.Warning) {
                colorHex = "#FFC107";
                bgHex = "#22FFC107";
            } else if (type == LogLevel.Error) {
                colorHex = "#FF4B4B";
                bgHex = "#22FF4B4B";
            } else if (type == LogLevel.Seccess) {
                colorHex = "#38FF85";
                bgHex = "#2238FF85";
            }
            TextBlock.Foreground = new SolidColorBrush(Color.Parse(colorHex));
            BorderBlock.Background = new SolidColorBrush(Color.Parse(bgHex));
            BorderBlock.IsVisible = true;
        }
        public static void HideMessage(Border BorderBlock) {
            BorderBlock.IsVisible = false;
        }
    }
}