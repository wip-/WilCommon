using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace WilCommon
{
    public static class Helpers
    {
        static public int GetBytesPerPixel(PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    return 1;

                case PixelFormat.Format24bppRgb:
                    return 3;

                case PixelFormat.Format32bppArgb:
                    return 4;

                case PixelFormat.Format64bppArgb:
                    return 8;

                default:
                    Debug.Assert(false);
                    return 0;
            }
        }

        static public bool HasAlphaChannel(PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                case PixelFormat.Format24bppRgb:
                    return false;

                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format64bppArgb:
                    return true;

                default:
                    Debug.Assert(false);
                    return false;
            }
        }

        static public Color GetColorDiff(Color left, Color right)
        {
            int a = Math.Abs(left.A - right.A);
            int r = Math.Abs(left.R - right.R);
            int g = Math.Abs(left.G - right.G);
            int b = Math.Abs(left.B - right.B);

            return Color.FromArgb(
                Convert.ToByte(a.Clamp0_255()),
                Convert.ToByte(r.Clamp0_255()),
                Convert.ToByte(g.Clamp0_255()),
                Convert.ToByte(b.Clamp0_255()));
        }

        /// <summary>
        /// returns (1-ratio)*color0 + (ratio)*color1
        /// </summary>
        static public Color Lerp(Color color0, Color color1, double ratio)
        {
            double a = (1 - ratio) * color0.A + (ratio) * color1.A;
            double r = (1 - ratio) * color0.R + (ratio) * color1.R;
            double g = (1 - ratio) * color0.G + (ratio) * color1.G;
            double b = (1 - ratio) * color0.B + (ratio) * color1.B;

            return Color.FromArgb(
                Convert.ToByte(a.Clamp0_255()),
                Convert.ToByte(r.Clamp0_255()),
                Convert.ToByte(g.Clamp0_255()),
                Convert.ToByte(b.Clamp0_255()));
        }

        /// <summary>
        /// returns ratio0*color0 + ratio1*color1
        /// </summary>
        static public Color Weight(double ratio0, Color color0, double ratio1, Color color1)
        {
            double a = ratio0 * color0.A + ratio1 * color1.A;
            double r = ratio0 * color0.R + ratio1 * color1.R;
            double g = ratio0 * color0.G + ratio1 * color1.G;
            double b = ratio0 * color0.B + ratio1 * color1.B;

            return Color.FromArgb(
                Convert.ToByte(a.Clamp0_255()),
                Convert.ToByte(r.Clamp0_255()),
                Convert.ToByte(g.Clamp0_255()),
                Convert.ToByte(b.Clamp0_255()));
        }


        /// <summary>
        /// Linearly interpolates over the value x between the points (xMin, yMin) and (xMax, yMax).
        /// </summary>
        public static double Lerp(
            double x,
            double xMin, double xMax,
            double yMin, double yMax)
        {
            double ratio = (x - xMin) / (xMax - xMin);
            return yMin + ratio * (yMax - yMin);
        }

        /// <summary>
        /// Do not use in ASP.NET or whatever scenario that would do not support a MessageBox display
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static void MyCatch(System.Exception ex)
        {
            var st = new StackTrace(ex, true);      // stack trace for the exception with source file information
            var frame = st.GetFrame(0);             // top stack frame
            String sourceMsg = String.Format("{0}({1})", frame.GetFileName(), frame.GetFileLineNumber());
            Console.WriteLine(sourceMsg);
            MessageBox.Show(ex.Message + Environment.NewLine + sourceMsg);
            Debugger.Break();
        }


        internal static double Min(double p1, double p2, double p3)
        {
            return Math.Min(Math.Min(p1, p2), p3);
        }

        internal static double Max(double p1, double p2, double p3)
        {
            return Math.Max(Math.Max(p1, p2), p3);
        }

        // plot
        public static BitmapInfo ToBitmapInfo(this byte[] array)
        {
            BitmapInfo dest = new BitmapInfo(512, 512, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            var white = System.Drawing.Color.FromArgb(255, 255, 255, 255);
            var black = System.Drawing.Color.FromArgb(255, 0, 0, 0);

            for (int x = 0; x < dest.Width; x++)
            {
                for (int y = 0; y < dest.Height; ++y)
                {
                    int yUp = (dest.Height - 1) - y;
                    if (y == (int)(2 * array[x]))
                        dest.SetPixelColor(x, yUp, black);
                    else
                        dest.SetPixelColor(x, yUp, white);
                }
            }

            return dest;
        }


        public struct WavelengthRange
        {
            public double Start;
            public double End;
        }

        public static BitmapInfo ToSnakeCurve(this double[] array, int size, WavelengthRange? range = null)
        {
            int binsCount = array.Length;

            BitmapInfo bitmapInfo = new BitmapInfo(size, size, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // render white background
            for (var x = 0; x < size; ++x)
                for (var y = 0; y < size; ++y)
                    bitmapInfo.SetPixelColor(x, y, Color.White);

            int binWidth = (int)Math.Floor((double)size / binsCount);
            int margin = (size - binWidth * binsCount) / 2;

            for (var i = 0; i < binsCount; ++i)
            {
                var val = array[i];
                int yCurrent = (int)Helpers.Lerp(val, 0, 1, size - margin, margin);

                for (var j = 0; j < binWidth; ++j)
                {
                    int x = margin + binWidth * i + j;

                    if (range != null)
                    {
                        double lambda = Lerp(x, margin, size-margin, range.Value.Start, range.Value.End);
                        Color bkg = Colors.FromWaveLength(lambda);
                        float h = bkg.GetHue();
                        float s = bkg.GetSaturation();
                        float b = bkg.GetBrightness();

                        Color top = Colors.FromAhsb(255, h, s, 0.75f);
                        for (var y = margin; y <= yCurrent; ++y )
                        {
                            bitmapInfo.SetPixelColor(x, y, top);
                        }

                        Color bottom = Colors.FromAhsb(255, h, s, 0.50f);
                        for (var y = yCurrent + 1; y <= size - margin; ++y)
                        {
                            bitmapInfo.SetPixelColor(x, y, bottom);
                        }
                    }

                    bitmapInfo.SetPixelColor(x, yCurrent, Color.Black);
                    if (j == (binWidth - 1) && i != (binsCount - 1))
                    {
                        var sNext = array[i + 1];
                        int yNext = (int)Helpers.Lerp(sNext, 0, 1, size - margin, margin);
                        int yStart = Math.Min(yCurrent, yNext);
                        int yEnd = Math.Max(yCurrent, yNext);
                        for (var y = yStart; y <= yEnd; ++y)
                        {
                            bitmapInfo.SetPixelColor(x, y, Color.Black);
                        }
                    }
                }
            }

            return bitmapInfo;
        }












    }
}
