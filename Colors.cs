using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace WilCommon
{
    public static class Colors
    {

        // from http://stackoverflow.com/a/14917481/758666
        public static Color FromWaveLength(/*this Color color,*/ double wavelength)
        {
            const double Gamma = 0.80;

            double r = 0.0;
            double g = 0.0;
            double b = 0.0;

            if ( /*wavelength>=380 &&*/ wavelength < 440)
            {
                wavelength = Math.Max(wavelength, 380);

                r = -(wavelength - 440) / (440 - 380);
                g = 0.0;
                b = 1.0;
            }
            else if ((wavelength >= 440) && (wavelength < 490))
            {
                r = 0.0;
                g = (wavelength - 440) / (490 - 440);
                b = 1.0;
            }
            else if ((wavelength >= 490) && (wavelength < 510))
            {
                r = 0.0;
                g = 1.0;
                b = -(wavelength - 510) / (510 - 490);
            }
            else if ((wavelength >= 510) && (wavelength < 580))
            {
                r = (wavelength - 510) / (580 - 510);
                g = 1.0;
                b = 0.0;
            }
            else if ((wavelength >= 580) && (wavelength < 645))
            {
                r = 1.0;
                g = -(wavelength - 645) / (645 - 580);
                b = 0.0;
            }
            else if ((wavelength >= 645) /*&& (wavelength < 781)*/ )
            {
                r = 1.0;
                g = 0.0;
                b = 0.0;
            }

            // Let the intensity fall off near the vision limits
            double factor = 0.0;
            if ( /*(wavelength >= 380) && */ (wavelength < 420))
            {
                factor = 0.3 + 0.7 * (wavelength - 380) / (420 - 380);
            }
            else if ((wavelength >= 420) && (wavelength < 701))
            {
                factor = 1.0;
            }
            else if ((wavelength >= 701) /*&& (wavelength < 781)*/ )
            {
                factor = 0.3 + 0.7 * (780 - wavelength) / (780 - 700);
            }

            r = Math.Pow(r * factor, Gamma);
            g = Math.Pow(g * factor, Gamma);
            b = Math.Pow(b * factor, Gamma);

            return Color.FromArgb(255, r.ScaleToByte(), g.ScaleToByte(), b.ScaleToByte());
        }


        // http://blogs.msdn.com/b/cjacks/archive/2006/04/12/575476.aspx
        public static Color FromAhsb(int a, float h, float s, float b)
        {
            float fMax, fMid, fMin;
            int iSextant, iMax, iMid, iMin;

            if (0.5 < b)
            {
                fMax = b - (b * s) + s;
                fMin = b + (b * s) - s;
            }
            else
            {
                fMax = b + (b * s);
                fMin = b - (b * s);
            }

            iSextant = (int)Math.Floor(h / 60f);
            if (300f <= h)
            {
                h -= 360f;
            }
            h /= 60f;
            h -= 2f * (float)Math.Floor(((iSextant + 1f) % 6f) / 2f);
            if (0 == iSextant % 2)
            {
                fMid = h * (fMax - fMin) + fMin;
            }
            else
            {
                fMid = fMin - h * (fMax - fMin);
            }

            iMax = Convert.ToInt32(fMax * 255);
            iMid = Convert.ToInt32(fMid * 255);
            iMin = Convert.ToInt32(fMin * 255);

            switch (iSextant)
            {
                case 1:
                    return Color.FromArgb(a, iMid, iMax, iMin);
                case 2:
                    return Color.FromArgb(a, iMin, iMax, iMid);
                case 3:
                    return Color.FromArgb(a, iMin, iMid, iMax);
                case 4:
                    return Color.FromArgb(a, iMid, iMin, iMax);
                case 5:
                    return Color.FromArgb(a, iMax, iMin, iMid);
                default:
                    return Color.FromArgb(a, iMax, iMid, iMin);
            }
        }
        

    }

}
