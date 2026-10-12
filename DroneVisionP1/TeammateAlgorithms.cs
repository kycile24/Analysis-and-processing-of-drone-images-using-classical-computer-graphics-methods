using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace DroneVisionP1;

// Адаптація алгоритмів учасника №2: Оцу, відкриття, HSV-сегментація та ерозія.
// Морфологія використовує хрестоподібний структурний елемент 3x3, як в оригіналі.
public static class TeammateAlgorithms
{
    public static int Otsu(byte[,] gray)
    {
        int[] histogram = new int[256];
        foreach (byte pixel in gray) histogram[pixel]++;
        long sum = 0;
        for (int i = 0; i < 256; i++) sum += (long)i * histogram[i];
        int background = 0;
        long backgroundSum = 0;
        double bestVariance = -1;
        int best = 0;
        int total = gray.Length;
        for (int t = 0; t < 256; t++)
        {
            background += histogram[t];
            backgroundSum += (long)t * histogram[t];
            if (background == 0) continue;
            int foreground = total - background;
            if (foreground == 0) break;
            double difference = (double)backgroundSum / background - (double)(sum - backgroundSum) / foreground;
            double variance = (double)background * foreground * difference * difference;
            if (variance > bestVariance) { bestVariance = variance; best = t; }
        }
        return best;
    }

    public static byte[,] CrossErode(byte[,] src)
    {
        int h = src.GetLength(0), w = src.GetLength(1);
        var dst = new byte[h, w];
        Parallel.For(1, h - 1, y =>
        {
            for (int x = 1; x < w - 1; x++)
                if (src[y, x] >= 128 && src[y - 1, x] >= 128 && src[y + 1, x] >= 128 &&
                    src[y, x - 1] >= 128 && src[y, x + 1] >= 128) dst[y, x] = 255;
        });
        return dst;
    }

    public static byte[,] CrossDilate(byte[,] src)
    {
        int h = src.GetLength(0), w = src.GetLength(1);
        var dst = new byte[h, w];
        Parallel.For(0, h, y =>
        {
            for (int x = 0; x < w; x++)
                if (src[y, x] >= 128 || (y > 0 && src[y - 1, x] >= 128) ||
                    (y + 1 < h && src[y + 1, x] >= 128) ||
                    (x > 0 && src[y, x - 1] >= 128) ||
                    (x + 1 < w && src[y, x + 1] >= 128)) dst[y, x] = 255;
        });
        return dst;
    }

    public static List<(string Name, byte[,] Data)> RunOtsu(byte[,] gray)
    {
        int threshold = Otsu(gray);
        byte[,] binary = ImageAlgorithms.Threshold(gray, threshold + 1);
        byte[,] opening = CrossDilate(CrossErode(binary));
        return new() { ("01_gray", gray), ($"02_otsu_{threshold}", binary), ("03_opening", opening) };
    }

    // Налаштування HSV задаються в градусах і відсотках, як у проєкті напарника.
    public static List<(string Name, byte[,] Data)> RunHsv(Bitmap source,
        double hMin, double hMax, double sMin, double sMax, double vMin, double vMax, bool invert)
    {
        int w = source.Width, h = source.Height;
        var saturationPreview = new byte[h, w];
        var mask = new byte[h, w];
        using var rgb = new Bitmap(w, h, PixelFormat.Format24bppRgb);
        using (var graphics = Graphics.FromImage(rgb)) graphics.DrawImage(source, 0, 0, w, h);
        var rect = new Rectangle(0, 0, w, h);
        var bits = rgb.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        try
        {
            byte[] bytes = new byte[bits.Stride * h];
            Marshal.Copy(bits.Scan0, bytes, 0, bytes.Length);
            Parallel.For(0, h, y =>
            {
                for (int x = 0; x < w; x++)
                {
                    int i = y * bits.Stride + x * 3;
                    double r = bytes[i + 2] / 255.0, g = bytes[i + 1] / 255.0, b = bytes[i] / 255.0;
                    double max = Math.Max(r, Math.Max(g, b)), min = Math.Min(r, Math.Min(g, b));
                    double delta = max - min;
                    double value = max * 100;
                    double saturation = max == 0 ? 0 : delta / max * 100;
                    double hue = 0;
                    if (delta != 0)
                    {
                        if (max == r) hue = 60 * ((g - b) / delta);
                        else if (max == g) hue = 60 * ((b - r) / delta + 2);
                        else hue = 60 * ((r - g) / delta + 4);
                        if (hue < 0) hue += 360;
                    }
                    saturationPreview[y, x] = (byte)Math.Clamp((int)Math.Round(saturation * 2.55), 0, 255);
                    bool selected = (hMin <= hMax ? hue >= hMin && hue <= hMax : hue >= hMin || hue <= hMax)
                        && saturation >= sMin && saturation <= sMax && value >= vMin && value <= vMax;
                    if (invert) selected = !selected;
                    mask[y, x] = selected ? (byte)255 : (byte)0;
                }
            });
        }
        finally { rgb.UnlockBits(bits); }
        return new() { ("01_hsv_saturation_preview", saturationPreview), ("02_hsv_mask", mask), ("03_hsv_erosion", CrossErode(mask)) };
    }
}
