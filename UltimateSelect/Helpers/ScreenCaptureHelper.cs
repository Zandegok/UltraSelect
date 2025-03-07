using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Interop;


namespace UltimateSelect.Helpers
{
    public static class ScreenCaptureHelper
    {
        [DllImport("gdi32.dll")]
        static extern bool DeleteObject(IntPtr hObject);

public static BitmapSource CaptureScreen(Rect region)
{
    double scaleX = 1.0, scaleY = 1.0;
    // Если MainWindow отсутствует, используем Graphics для получения DPI
    var mainWindow = Application.Current?.MainWindow;
    if (mainWindow != null)
    {
        var source = PresentationSource.FromVisual(mainWindow);
        if (source != null)
        {
            var transform = source.CompositionTarget.TransformToDevice;
            scaleX = transform.M11;
            scaleY = transform.M22;
        }
    }
    else
    {
        using (var g = Graphics.FromHwnd(IntPtr.Zero))
        {
            scaleX = g.DpiX / 96.0;
            scaleY = g.DpiY / 96.0;
        }
    }

    int pixelX = (int)(region.X * scaleX);
    int pixelY = (int)(region.Y * scaleY);
    int pixelWidth = (int)(region.Width * scaleX);
    int pixelHeight = (int)(region.Height * scaleY);

    using (Bitmap bmp = new Bitmap(pixelWidth, pixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
    {
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.CopyFromScreen(pixelX, pixelY, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
        }
        IntPtr hBitmap = bmp.GetHbitmap();
        try
        {
            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap, IntPtr.Zero, Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            return bitmapSource;
        }
        finally
        {
            DeleteObject(hBitmap);
        }
    }
}
    }
}
