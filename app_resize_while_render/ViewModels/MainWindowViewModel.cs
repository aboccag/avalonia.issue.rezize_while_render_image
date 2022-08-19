using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using ReactiveUI;
using SkiaSharp;

namespace app_resize_while_render.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            DispatcherTimer timer = new DispatcherTimer(TimeSpan.FromMilliseconds(10), DispatcherPriority.Render, OnTick);
            timer.Start();
        }

        private async void OnTick(object? sender, EventArgs e)
        {
            await LoadNewImageAsync();
        }

        private SKBitmap _mySkBitmap = SKBitmap.Decode(System.IO.File.ReadAllBytes("lenna.jpg"));
        private WriteableBitmap? _prevRef;
        private WriteableBitmap? _currentRef;
        private WriteableBitmap? finalBitmap;

        private SKBitmap MySkBitmap => _mySkBitmap.Copy();



        async Task LoadNewImageAsync()
        {
            await Task.Run(() =>
            {
                using var skImage = SKImage.FromBitmap(MySkBitmap);
                using var data = skImage.Encode(SKEncodedImageFormat.Jpeg, 100);
                using var stream = data.AsStream();
                var writeableBitmap = WriteableBitmap.Decode(stream);
                MySkBitmap.Dispose();


                if (_prevRef != FinalBitmap && _prevRef is not null)
                {
                    _prevRef.Dispose();
                }

                _prevRef = finalBitmap;

                finalBitmap = writeableBitmap;
            });

            this.RaisePropertyChanged(nameof(FinalBitmap));
        }

        public WriteableBitmap? FinalBitmap => finalBitmap;
    }
}
