using Avalonia.Threading;
using System;
using Avalonia.Media;
using Avalonia.Controls;
using lab4.Interface;

namespace lab4.Models {
    public class SpinComponent : ISpin {
        private static DispatcherTimer? _spinnerTimer;

        // запуск крутіння спіна
        public void StartSpinner(Image Spinner) {
            Spinner.IsVisible = true;
            _spinnerTimer?.Stop();
            _spinnerTimer = null;
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
        public void StopSpinner(Image Spinner) {
            Spinner.IsVisible = false;
            _spinnerTimer?.Stop();
            _spinnerTimer = null;
        }
    }
}