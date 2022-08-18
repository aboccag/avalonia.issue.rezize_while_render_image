using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using Avalonia.Media.Imaging;
using SkiaSharp;

namespace app_resize_while_render.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private SKBitmap _mySkBitmap = SKBitmap.Decode(System.IO.File.ReadAllBytes("lenna.jpg"));
        private WriteableBitmap? _prevRef;
        private SKBitmap MySkBitmap => _mySkBitmap.Copy();
        public IObservable<Bitmap?> DisplayInputImageObservable => Observable.Interval(TimeSpan.FromMilliseconds(10)).Select(_ => MySkBitmap)
            .Select(img =>
            {
                using var skImage = SKImage.FromBitmap(img);
                using var data = skImage.Encode(SKEncodedImageFormat.Jpeg, 100);
                using var stream = data.AsStream();
                var writeableBitmap = WriteableBitmap.Decode(stream);
                img.Dispose();


                if (_prevRef != writeableBitmap && _prevRef is not null)
                {
                    _prevRef.Dispose();
                }

                _prevRef = writeableBitmap;

                return writeableBitmap;
            })
            .Catch<Bitmap?, Exception>(e => { return Observable.Empty<Bitmap>(); });
    }
}
