using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
//using System.Windows.Interop.Imaging;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace WilCommon
{
    public static class Extensions
    {
        // maps a [0,1] double to a [0,255] byte
        public static byte ScaleToByte(this double value)
        {
            return Convert.ToByte((255 * value).Clamp0_255());
        }

        public static int Clamp0_255(this int value)
        {
            return value.Clamp(0, 255);
        }

        public static float Clamp0_255(this float value)
        {
            return value.Clamp(0, 255);
        }

        public static double Clamp0_255(this double value)
        {
            return value.Clamp(0, 255);
        }

        public static int Clamp0_65535(this int value)
        {
            return value.Clamp(0, 65535);
        }

        public static double ANormalized(this System.Drawing.Color color) { return (double)color.A / 255; }
        public static double RNormalized(this System.Drawing.Color color) { return (double)color.R / 255; }
        public static double GNormalized(this System.Drawing.Color color) { return (double)color.G / 255; }
        public static double BNormalized(this System.Drawing.Color color) { return (double)color.B / 255; }


        //http://stackoverflow.com/a/2683487/758666
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }   

        public static byte ToByte(this double val)
        {
            return Convert.ToByte(val.Clamp(0, 255));
        }

        // http://stackoverflow.com/a/1546121/758666
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        public static BitmapSource ToBitmapSource(this Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            try 
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap, IntPtr.Zero, Int32Rect.Empty, 
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
            finally 
            {
                DeleteObject(hBitmap);
            }
        }


    }
}
