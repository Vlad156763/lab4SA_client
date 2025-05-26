using Avalonia.Threading;
using System;
using Avalonia.Media;
using Avalonia.Controls;

namespace lab4.Models {
    public static class Spin {
        private static DispatcherTimer? _spinnerTimer;

        // запуск крутіння спіна
        public static void StartSpinner(Image Spinner) {
            Spinner.IsVisible = true;
            _spinnerTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
            if (Spinner.RenderTransform != null) {
                var transform = (RotateTransform)Spinner.RenderTransform;
                _spinnerTimer.Tick += (_, _) => {
                    if(transform != null)
                        transform.Angle = (transform.Angle + 4) % 360;
                };
            }
            _spinnerTimer.Start();
        }
        //зупитка спіна
        public static void StopSpinner(Image Spinner) {
            Spinner.IsVisible = false;
            _spinnerTimer?.Stop();
            _spinnerTimer = null;
        }
    }
}