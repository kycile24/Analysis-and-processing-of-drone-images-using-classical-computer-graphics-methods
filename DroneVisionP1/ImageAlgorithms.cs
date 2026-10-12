using System.Drawing;
using System.Drawing.Imaging;

namespace DroneVisionP1;

// Кожен алгоритм реалізований вручну, без готових фільтрів бібліотек.
public static class ImageAlgorithms
{
    // Швидке читання пікселів через LockBits замість повільного GetPixel.
    public static byte[,] Grayscale(Bitmap source)
    {
        int w = source.Width, h = source.Height;
        var result = new byte[h, w];
        using var converted = new Bitmap(w, h, PixelFormat.Format24bppRgb);
        using (var graphics = Graphics.FromImage(converted))
            graphics.DrawImage(source, 0, 0, w, h);
        var rect = new Rectangle(0, 0, w, h);
        var bits = converted.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        try
        {
            int stride = bits.Stride;
            byte[] pixels = new byte[stride * h];
            System.Runtime.InteropServices.Marshal.Copy(bits.Scan0, pixels, 0, pixels.Length);
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    int i = y * stride + x * 3;
                    int b = pixels[i], g = pixels[i + 1], r = pixels[i + 2];
                    result[y, x] = (byte)((299 * r + 587 * g + 114 * b + 500) / 1000);
                }
        }
        finally { converted.UnlockBits(bits); }
        return result;
    }

    public static byte[,] Equalize(byte[,] src)
    {
        int h = src.GetLength(0), w = src.GetLength(1);
        var hist = new int[256];
        foreach (byte v in src) hist[v]++;
        var cdf = new int[256];
        int sum = 0;
        for (int i = 0; i < 256; i++) { sum += hist[i]; cdf[i] = sum; }
        int first = Array.Find(cdf, v => v > 0);
        var dst = new byte[h, w];
        if (first == h * w) return (byte[,])src.Clone();
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                dst[y, x] = (byte)Math.Clamp((int)Math.Round((cdf[src[y, x]] - first) * 255.0 / (h * w - first)), 0, 255);
        return dst;
    }

    // Оптимізований оператор Прюітта / Собеля: прямі сусіди замість масивів ядер,
    // паралельна обробка рядків, квадрат градієнта без sqrt у першому проході.
    public static byte[,] Gradient(byte[,] src, bool sobel)
    {
        int h = src.GetLength(0), w = src.GetLength(1);
        var result = new byte[h, w];
        if (h < 3 || w < 3) return result;
        var squared = new int[h, w];
        int globalMax = 0;
        object gate = new();
        int weight = sobel ? 2 : 1;
        Parallel.For(1, h - 1, () => 0, (y, _, localMax) =>
        {
            for (int x = 1; x < w - 1; x++)
            {
                int tl = src[y - 1, x - 1], t = src[y - 1, x], tr = src[y - 1, x + 1];
                int l = src[y, x - 1], r = src[y, x + 1];
                int bl = src[y + 1, x - 1], b = src[y + 1, x], br = src[y + 1, x + 1];
                int gx = (tr + weight * r + br) - (tl + weight * l + bl);
                int gy = (bl + weight * b + br) - (tl + weight * t + tr);
                int v = gx * gx + gy * gy;
                squared[y, x] = v;
                if (v > localMax) localMax = v;
            }
            return localMax;
        }, localMax => { lock (gate) if (localMax > globalMax) globalMax = localMax; });
        if (globalMax == 0) return result;
        double scale = 255.0 / Math.Sqrt(globalMax);
        Parallel.For(1, h - 1, y =>
        {
            for (int x = 1; x < w - 1; x++)
                result[y, x] = (byte)Math.Min(255, (int)(Math.Sqrt(squared[y, x]) * scale + 0.5));
        });
        return result;
    }

    public static byte[,] Threshold(byte[,] src, int level)
    {
        int h = src.GetLength(0), w = src.GetLength(1);
        var dst = new byte[h, w];
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++) dst[y, x] = src[y, x] >= level ? (byte)255 : (byte)0;
        return dst;
    }

    // Бінарна морфологія 3×3 з раннім виходом і паралельними рядками.
    public static byte[,] Dilate(byte[,] src)
    {
        int h = src.GetLength(0), w = src.GetLength(1);
        var dst = new byte[h, w];
        Parallel.For(0, h, y =>
        {
            for (int x = 0; x < w; x++)
            {
                bool found = false;
                for (int yy = Math.Max(0, y - 1); yy <= Math.Min(h - 1, y + 1) && !found; yy++)
                    for (int xx = Math.Max(0, x - 1); xx <= Math.Min(w - 1, x + 1); xx++)
                        if (src[yy, xx] == 255) { found = true; break; }
                dst[y, x] = found ? (byte)255 : (byte)0;
            }
        });
        return dst;
    }

    public static byte[,] Erode(byte[,] src)
    {
        int h = src.GetLength(0), w = src.GetLength(1);
        var dst = new byte[h, w];
        Parallel.For(1, h - 1, y =>
        {
            for (int x = 1; x < w - 1; x++)
            {
                bool all = true;
                for (int yy = y - 1; yy <= y + 1 && all; yy++)
                    for (int xx = x - 1; xx <= x + 1; xx++)
                        if (src[yy, xx] != 255) { all = false; break; }
                dst[y, x] = all ? (byte)255 : (byte)0;
            }
        });
        return dst;
    }

    public static byte[,] Closing(byte[,] binary) => Erode(Dilate(binary));

    public static List<(string Name, byte[,] Data)> Run(Bitmap source, int pipeline, int threshold)
    {
        if (pipeline == 4) return TeammateAlgorithms.RunHsv(source, 0, 360, 0, 100, 0, 100, false);
        return Run(Grayscale(source), pipeline, threshold);
    }

    public static List<(string Name, byte[,] Data)> Run(byte[,] gray, int pipeline, int threshold)
    {
        if (pipeline == 1)
        {
            byte[,] edges = Gradient(gray, false);
            return new() { ("01_gray", gray), ("02_prewitt", edges), ("03_threshold", Threshold(edges, threshold)) };
        }
        if (pipeline == 2)
        {
            byte[,] equalized = Equalize(gray);
            byte[,] edges = Gradient(equalized, true);
            return new() { ("01_equalized", equalized), ("02_sobel", edges), ("03_closing", Closing(Threshold(edges, threshold))) };
        }
        if (pipeline == 3) return TeammateAlgorithms.RunOtsu(gray);
        throw new ArgumentOutOfRangeException(nameof(pipeline));
    }

    public static Bitmap ToBitmap(byte[,] data)
    {
        int h = data.GetLength(0), w = data.GetLength(1);
        var bitmap = new Bitmap(w, h, PixelFormat.Format24bppRgb);
        var rect = new Rectangle(0, 0, w, h);
        BitmapData bits = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
        try
        {
            int stride = bits.Stride;
            var bytes = new byte[stride * h];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    int i = y * stride + x * 3;
                    bytes[i] = bytes[i + 1] = bytes[i + 2] = data[y, x];
                }
            System.Runtime.InteropServices.Marshal.Copy(bytes, 0, bits.Scan0, bytes.Length);
        }
        finally { bitmap.UnlockBits(bits); }
        return bitmap;
    }

    // Глобальний SSIM (не локальний SSIM з вікнами).
    // Метрики порівняння з оригіналом НЕ оцінюють точність виявлення дронів.
    public static (double Mse, double Psnr, double Ssim) Metrics(byte[,] a, byte[,] b)
    {
        int h = a.GetLength(0), w = a.GetLength(1), n = h * w;
        if (b.GetLength(0) != h || b.GetLength(1) != w) throw new ArgumentException("Розміри не збігаються");
        double sa = 0, sb = 0, mse = 0;
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++)
        { sa += a[y, x]; sb += b[y, x]; double d = a[y, x] - b[y, x]; mse += d * d; }
        double ma = sa / n, mb = sb / n;
        mse /= n;
        double va = 0, vb = 0, cov = 0;
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++)
        {
            double da = a[y, x] - ma, db = b[y, x] - mb;
            va += da * da; vb += db * db; cov += da * db;
        }
        va /= n; vb /= n; cov /= n;
        double c1 = Math.Pow(.01 * 255, 2), c2 = Math.Pow(.03 * 255, 2);
        double ssim = ((2 * ma * mb + c1) * (2 * cov + c2)) /
                      ((ma * ma + mb * mb + c1) * (va + vb + c2));
        double psnr = mse == 0 ? double.PositiveInfinity : 10 * Math.Log10(255 * 255 / mse);
        return (mse, psnr, ssim);
    }
}
