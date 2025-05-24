using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia;


namespace lab4 {
    public partial class App : Application {
        public override void Initialize() {
            AvaloniaXamlLoader.Load(this);
        }
        public override void OnFrameworkInitializationCompleted() {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                var args = Program.CmdArgs;
                if (args.Length >= 2) {
                    var login = args[0];
                    var password = args[1];
                    desktop.MainWindow = new MainWindow(login, password);
                }
                else {
                    desktop.MainWindow = new MainWindow();
                }
            }
            base.OnFrameworkInitializationCompleted();
        }
        
    }
}


