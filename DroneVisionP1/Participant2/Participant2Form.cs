using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace DroneVisionP1
{
    public partial class Participant2Form : Form
    {
        // Пейплайн 1
        private string[] stemImageFiles = Array.Empty<string>(); // шляхи до фотографій дронів
        private Bitmap? stemOriginalImage;
        private Bitmap? stemGrayImage;
        private Bitmap? stemOtsuImage;
        private Bitmap? stemOpenedImage;

        // Пейплайн 2
        private Bitmap? stemHSVMaskImage;   // результат кольорової сегментації
        private Bitmap? stemHSVErodedImage; // результат ерозії

        public Participant2Form()
        {
            InitializeComponent();
        }

        // Методи обробки
        // Пейплайн 1

        private Bitmap ConvertToGrayscale(Bitmap source) // перетворення RGB у відтінки сірого
        {
            Bitmap result = new Bitmap(source.Width, source.Height); // створюємо нове зображення

            for (int y = 0; y < source.Height; y++) // проходимо всі рядки
            {
                for (int x = 0; x < source.Width; x++) // проходимо всі стовпці
                {
                    Color pixel = source.GetPixel(x, y); // отримуємо колір поточного пікселя

                    int gray = (int)Math.Round(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B); // обчислюємо яскравість за компонентами RGB

                    Color grayColor = Color.FromArgb(gray, gray, gray); // створюємо сірий колір з однаковими компонентами

                    result.SetPixel(x, y, grayColor); // записуємо піксель у результат
                }
            }

            return result; // повертаємо готове зображення
        }

        private int CalculateOtsuThreshold(Bitmap grayImage) // автоматичне визначення порога методом Оцу
        {
            int[] histogram = new int[256]; // створюємо масив для 256 рівнів яскравості

            int width = grayImage.Width;
            int height = grayImage.Height;

            int totalPixels = width * height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = grayImage.GetPixel(x, y);

                    int brightness = pixel.R; // у сірому зображенні R = G = B

                    histogram[brightness]++; // збільшуємо кількість пікселів цієї яскравості
                }
            }

            long totalSum = 0; // сума яскравостей усіх пікселів

            for (int i = 0; i < 256; i++) // проходимо всі рівні яскравості
            {
                totalSum += (long)i * histogram[i]; // накопичуємо загальну суму
            }

            int backgroundCount = 0; // кількість пікселів першого класу
            long backgroundSum = 0; // сума яскравостей першого класу

            double maxVariance = -1; // найбільша знайдена міжкласова дисперсія
            int bestThreshold = 0; // найкращий поріг

            for (int t = 0; t < 256; t++) // перевіряємо всі можливі пороги
            {
                backgroundCount += histogram[t]; // додаємо пікселі поточного рівня до першого класу

                if (backgroundCount == 0) // якщо перший клас порожній
                {
                    continue;
                }

                int foregroundCount = totalPixels - backgroundCount; // кількість пікселів другого класу

                if (foregroundCount == 0)
                {
                    break;
                }

                backgroundSum += (long)t * histogram[t]; // сума яскравостей першого класу

                double meanBackground = (double)backgroundSum / backgroundCount; // середня яскравість першого класу

                double meanForeground = (double)(totalSum - backgroundSum) / foregroundCount; // середня яскравість другого класу

                double difference = meanBackground - meanForeground; // різниця середніх яскравостей

                double variance = (double)backgroundCount * foregroundCount * difference * difference; // обчислюємо величину, пропорційну міжкласовій дисперсії

                if (variance > maxVariance) // якщо поточний поріг кращий
                {
                    maxVariance = variance; // запам'ятовуємо новий максимум

                    bestThreshold = t; // зберігаємо найкращий поріг
                }
            }

            return bestThreshold; // повертаємо автоматично знайдений поріг
        }

        private Bitmap ApplyErosion(Bitmap source) // морфологічна ерозія
        {
            Bitmap result = new Bitmap(source.Width, source.Height); // створюємо нове зображення

            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    bool keepWhite = true; // спочатку припускаємо, що піксель залишиться білим

                    for (int dy = -1; dy <= 1 && keepWhite; dy++) // перевіряємо сусідні рядки
                    {
                        for (int dx = -1; dx <= 1; dx++) // перевіряємо сусідні стовпці
                        {
                            if (Math.Abs(dx) + Math.Abs(dy) > 1)
                            {
                                continue; // пропускаємо кути, використовуємо хрест 3×3
                            }

                            int nx = x + dx; // координата сусіда X
                            int ny = y + dy; // координата сусіда Y

                            if (nx < 0 || nx >= source.Width || ny < 0 || ny >= source.Height)
                            {
                                keepWhite = false; // за межами зображення вважаємо фон чорним
                                break;
                            }

                            Color pixel = source.GetPixel(nx, ny); // отримуємо сусідній піксель

                            if (pixel.R < 128) // якщо сусід чорний
                            {
                                keepWhite = false; // центральний піксель не зберігаємо білим
                                break;
                            }
                        }
                    }

                    Color newColor = keepWhite ? Color.White : Color.Black; // визначаємо новий колір

                    result.SetPixel(x, y, newColor); // записуємо результат
                }
            }

            return result; // повертаємо зображення після ерозії
        }

        private Bitmap ApplyDilation(Bitmap source) // морфологічна дилатація
        {
            Bitmap result = new Bitmap(source.Width, source.Height);

            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    bool makeWhite = false; // спочатку вважаємо піксель чорним

                    for (int dy = -1; dy <= 1 && !makeWhite; dy++)
                    {
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            if (Math.Abs(dx) + Math.Abs(dy) > 1)
                            {
                                continue;
                            }

                            int nx = x + dx;
                            int ny = y + dy;

                            if (nx < 0 || nx >= source.Width || ny < 0 || ny >= source.Height)
                            {
                                continue; // пропускаємо координати за межами зображення
                            }

                            Color pixel = source.GetPixel(nx, ny);

                            if (pixel.R >= 128) // якщо знайдено хоча б один білий піксель
                            {
                                makeWhite = true; // центральний піксель робимо білим
                                break;
                            }
                        }
                    }

                    Color newColor = makeWhite ? Color.White : Color.Black; // вибираємо колір результату

                    result.SetPixel(x, y, newColor); // записуємо новий піксель
                }
            }

            return result; // повертаємо зображення після дилатації
        }

        // Пейплайн 2
        private void ConvertRGBtoHSV(Color pixel, out double hue, out double saturation, out double value) // перетворення RGB у HSV
        {
            double r = pixel.R / 255.0; // переводимо RGB із діапазону 0–255 у 0–1
            double g = pixel.G / 255.0;
            double b = pixel.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b)); // знаходимо найбільший і найменший компоненти
            double min = Math.Min(r, Math.Min(g, b));


            double delta = max - min; // різниця між максимумом і мінімумом

            value = max * 100.0; // визначаємо яскравість V

            saturation = max == 0 ? 0 : delta / max * 100.0; // визначаємо насиченість S

            hue = 0; // початкове значення колірного тону

            if (delta == 0) // для сірого кольору тон не визначений, залишаємо 0
            {
                return;
            }

            if (max == r) // обчислюємо тон залежно від найбільшого каналу
            {
                hue = 60 * ((g - b) / delta);
            }
            else if (max == g)
            {
                hue = 60 * ((b - r) / delta + 2);
            }
            else
            {
                hue = 60 * ((r - g) / delta + 4);
            }

            if (hue < 0) // переводимо від'ємний кут у діапазон 0–360
            {
                hue += 360;
            }
        }

        // Робота інтерфейсу
        // Пейплайн 1

        private void btnStemLoad_Click(object sender, EventArgs e) // завантаження папки з фотографіями
        {
            using FolderBrowserDialog dialog = new FolderBrowserDialog();

            dialog.Description = "Виберіть папку з фотографіями дронів";

            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            string folderPath = dialog.SelectedPath; // отримуємо шлях до папки

            string[] allFiles = Directory.GetFiles(folderPath); // отримуємо всі файли папки

            List<string> photoFiles = new List<string>(); // створюємо список фотографій

            foreach (string file in allFiles) // перебираємо всі знайдені файли
            {
                string extension = Path.GetExtension(file).ToLowerInvariant(); // отримуємо розширення

                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".bmp")
                {
                    photoFiles.Add(file); // додаємо файл до списку фотографій
                }
            }

            if (photoFiles.Count == 0)
            {
                MessageBox.Show("У вибраній папці немає зображень.");
                return;
            }

            photoFiles.Sort(StringComparer.OrdinalIgnoreCase); // сортуємо фотографії за назвами

            stemImageFiles = photoFiles.ToArray(); // перетворюємо список на масив шляхів

            cmbStemImage.Items.Clear();

            foreach (string file in stemImageFiles) // проходимо всі фотографії
            {
                cmbStemImage.Items.Add(Path.GetFileName(file)); // додаємо назву файла до ComboBox
            }

            cmbStemImage.SelectedIndex = 0; // вибираємо перше зображення

            MessageBox.Show($"Завантажено фотографій: {stemImageFiles.Length}");
        }

        private void btnStemGrayscale_Click(object sender, EventArgs e) //  перетворення у відтінки сірого
        {
            if (stemOriginalImage == null)
            {
                MessageBox.Show("Спочатку виберіть зображення.");
                return;
            }

            pictureBoxStemGray.Image = null;
            pictureBoxStemOtsu.Image = null;
            pictureBoxStemOpening.Image = null;

            stemGrayImage?.Dispose();
            stemOtsuImage?.Dispose();
            stemOpenedImage?.Dispose();

            stemOtsuImage = null;
            stemOpenedImage = null;

            stemGrayImage = ConvertToGrayscale(stemOriginalImage); // запускаємо алгоритм

            pictureBoxStemGray.Image = stemGrayImage;
        }

        private void cmbStemImage_SelectedIndexChanged(object sender, EventArgs e) // перемикання фотографій
        {
            int index = cmbStemImage.SelectedIndex; // номер вибраної фотографії

            if (index < 0 || index >= stemImageFiles.Length)
            {
                return;
            }

            try
            {
                using Bitmap source = new Bitmap(stemImageFiles[index]); // відкриваємо фотографію

                Bitmap newOriginal = new Bitmap(source); // створюємо копію зображення

                pictureBoxStemOriginal.Image = null; // очищаємо зображення в інтерфейсі
                pictureBoxStemGray.Image = null;
                pictureBoxStemOtsu.Image = null;
                pictureBoxStemOpening.Image = null;

                pictureBoxStemHSVMask.Image = null;
                pictureBoxStemHSVEroded.Image = null;

                stemOriginalImage?.Dispose(); // звільняємо пам'ять
                stemGrayImage?.Dispose();
                stemOtsuImage?.Dispose();
                stemOpenedImage?.Dispose();

                stemHSVMaskImage?.Dispose();
                stemHSVErodedImage?.Dispose();

                stemOriginalImage = newOriginal; // встановлюємо нове зображення

                stemGrayImage = null; // скидаємо результати пайплайна №3
                stemOtsuImage = null;
                stemOpenedImage = null;

                stemHSVMaskImage = null; // скидаємо результати пайплайна №4
                stemHSVErodedImage = null;

                pictureBoxStemOriginal.Image = stemOriginalImage; // показуємо нову фотографію

                lblStemOtsuThreshold.Text = "Поріг Оцу: -"; // скидаємо старий поріг
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не вдалося відкрити зображення: " + ex.Message);
            }
        }

        private void btnStemOtsu_Click(object sender, EventArgs e) // метод оцу
        {
            if (stemGrayImage == null) // перевіряємо наявність сірого зображення
            {
                MessageBox.Show("Спочатку перетворіть зображення у відтінки сірого.");
                return;
            }

            int threshold = CalculateOtsuThreshold(stemGrayImage); // знаходимо поріг методом Оцу

            Bitmap result = new Bitmap(stemGrayImage.Width, stemGrayImage.Height); // створюємо нове зображення

            for (int y = 0; y < stemGrayImage.Height; y++)
            {
                for (int x = 0; x < stemGrayImage.Width; x++)
                {
                    Color pixel = stemGrayImage.GetPixel(x, y);

                    int brightness = pixel.R;

                    Color binaryColor;

                    if (brightness <= threshold) // якщо піксель темніший за поріг
                    {
                        binaryColor = Color.White; // позначаємо його як передній план
                    }
                    else
                    {
                        binaryColor = Color.Black; // світлі пікселі відносимо до фону
                    }

                    result.SetPixel(x, y, binaryColor); // записуємо чорно-білий піксель
                }
            }

            pictureBoxStemOtsu.Image = null;
            pictureBoxStemOpening.Image = null;

            stemOtsuImage?.Dispose();
            stemOpenedImage?.Dispose();

            stemOtsuImage = result;
            stemOpenedImage = null;

            pictureBoxStemOtsu.Image = stemOtsuImage;

            lblStemOtsuThreshold.Text = $"Поріг Оцу: {threshold}";
        }

        private void btnStemOpening_Click(object sender, EventArgs e) // морфологічне відкриття
        {
            if (stemOtsuImage == null)
            {
                MessageBox.Show("Спочатку застосуйте метод Оцу.");
                return;
            }

            using Bitmap eroded = ApplyErosion(stemOtsuImage); // спочатку виконуємо ерозію

            Bitmap opened = ApplyDilation(eroded); // потім виконуємо дилатацію

            pictureBoxStemOpening.Image = null;

            stemOpenedImage?.Dispose();

            stemOpenedImage = opened;

            pictureBoxStemOpening.Image = stemOpenedImage;

            MessageBox.Show("Морфологічне відкриття виконано.");
        }

        // Пейплайн 2

        private void btnStemSegmentHSV_Click(object sender, EventArgs e) // кольорова сегментація HSV
        {
            if (stemOriginalImage == null)
            {
                MessageBox.Show("Спочатку завантажте зображення.");
                return;
            }

            double hMin = (double)numStemHMin.Value;  // зчитуємо задані користувачем діапазони
            double hMax = (double)numStemHMax.Value;

            double sMin = (double)numStemSMin.Value;
            double sMax = (double)numStemSMax.Value;

            double vMin = (double)numStemVMin.Value;
            double vMax = (double)numStemVMax.Value;

            if (sMin > sMax || vMin > vMax) // перевіряємо правильність діапазонів S і V
            {
                MessageBox.Show("Перевірте діапазони S та V.");
                return;
            }

            Bitmap result = new Bitmap(stemOriginalImage.Width, stemOriginalImage.Height); // створюємо порожню бінарну маску

            for (int y = 0; y < stemOriginalImage.Height; y++)
            {
                for (int x = 0; x < stemOriginalImage.Width; x++)
                {
                    Color pixel = stemOriginalImage.GetPixel(x, y); // отримуємо колір RGB

                    ConvertRGBtoHSV(pixel, out double h, out double s, out double v); // перетворюємо RGB у HSV

                    bool hueMatches = hMin <= hMax ? h >= hMin && h <= hMax : h >= hMin || h <= hMax; // перевіряємо колірний тон і враховуємо можливий перехід через 0 градусів

                    bool saturationMatches = s >= sMin && s <= sMax; // перевіряємо насиченість і яскравість
                    bool valueMatches = v >= vMin && v <= vMax;

                    bool isSelected = hueMatches && saturationMatches && valueMatches; // піксель належить вибраному діапазону HSV

                    if (chkStemHSVInvert.Checked) // за потреби інвертуємо результат
                    {
                        isSelected = !isSelected;
                    }

                    Color newColor = isSelected ? Color.White : Color.Black; // створюємо білу або чорну точку маски

                    result.SetPixel(x, y, newColor);
                }
            }

            pictureBoxStemHSVMask.Image = null;
            pictureBoxStemHSVEroded.Image = null;

            stemHSVMaskImage?.Dispose();
            stemHSVErodedImage?.Dispose();

            stemHSVMaskImage = result;
            stemHSVErodedImage = null;

            pictureBoxStemHSVMask.Image = stemHSVMaskImage;
        }

        private void btnStemHSVErosion_Click(object sender, EventArgs e) // морфологічна ерозія після HSV-сегментації
        {
            if (stemHSVMaskImage == null)
            {
                MessageBox.Show("Спочатку виконайте HSV-сегментацію.");
                return;
            }

            Bitmap result = ApplyErosion(stemHSVMaskImage); // застосовуємо ерозію з пайплайна №3

            pictureBoxStemHSVEroded.Image = null;

            stemHSVErodedImage?.Dispose();

            stemHSVErodedImage = result;

            pictureBoxStemHSVEroded.Image = stemHSVErodedImage;
        }

        private void btnStemSaveAll_Click(object sender, EventArgs e) // збереження всіх результатів
        {
            if (stemOriginalImage == null || stemGrayImage == null || stemOtsuImage == null || stemOpenedImage == null) // перевіряємо завершення пайплайна №3
            {
                MessageBox.Show("Спочатку виконайте всі етапи пайплайна №3.");
                return;
            }

            if (stemHSVMaskImage == null || stemHSVErodedImage == null) // перевіряємо завершення пайплайна №4
            {
                MessageBox.Show("Спочатку виконайте всі етапи пайплайна №4.");
                return;
            }

            int imageIndex = cmbStemImage.SelectedIndex; // отримуємо номер фотографії

            if (imageIndex < 0 || imageIndex >= stemImageFiles.Length)
            {
                MessageBox.Show("Спочатку виберіть фотографію.");
                return;
            }

            using FolderBrowserDialog dialog = new FolderBrowserDialog(); // вибір папки збереження

            dialog.Description = "Виберіть папку для збереження результатів";

            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            string folderPath = dialog.SelectedPath; // шлях до вибраної папки

            string photoName = Path.GetFileNameWithoutExtension(stemImageFiles[imageIndex]); // назва початкової фотографії без розширення

            string fileOriginal = Path.Combine(folderPath, $"{photoName}_P3_1_Original.png");

            string fileGray = Path.Combine(folderPath, $"{photoName}_P3_2_Grayscale.png");

            string fileOtsu = Path.Combine(folderPath, $"{photoName}_P3_3_Otsu.png");

            string fileOpening = Path.Combine(folderPath, $"{photoName}_P3_4_Opening.png");

            string fileHSV = Path.Combine(folderPath, $"{photoName}_P4_1_HSVMask.png");

            string fileErosion = Path.Combine(folderPath, $"{photoName}_P4_2_Erosion.png");

            string[] outputFiles =
            {
            fileOriginal,
            fileGray,
            fileOtsu,
            fileOpening,
            fileHSV,
            fileErosion
            }; // усі шляхи для збереження

            foreach (string file in outputFiles) // перевіряємо наявність старих результатів
            {
                if (File.Exists(file))
                {
                    DialogResult answer = MessageBox.Show("Деякі результати вже існують. Перезаписати їх?", "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (answer != DialogResult.Yes)
                    {
                        return;
                    }

                    break;
                }
            }

            try
            {
                stemOriginalImage.Save(fileOriginal, ImageFormat.Png); // зберігаємо оригінальне зображення

                stemGrayImage.Save(fileGray, ImageFormat.Png); // зберігаємо відтінки сірого

                stemOtsuImage.Save(fileOtsu, ImageFormat.Png); // зберігаємо результат методу Оцу

                stemOpenedImage.Save(fileOpening, ImageFormat.Png); // зберігаємо результат морфологічного відкриття

                stemHSVMaskImage.Save(fileHSV, ImageFormat.Png); // зберігаємо маску HSV

                stemHSVErodedImage.Save(fileErosion, ImageFormat.Png); // зберігаємо маску після ерозії

                MessageBox.Show("Успішно збережено 6 зображень.\nПапка: " + folderPath, "Збереження завершено", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) // обробляємо помилки збереження
            {
                MessageBox.Show("Помилка збереження: " + ex.Message);
            }
        }

        // розрахування метриків

        private double CalculateMSE(Bitmap original, Bitmap processed) // обчислення середньоквадратичної похибки між зображеннями
        {
            if (original.Width != processed.Width || original.Height != processed.Height) // перевіряємо, чи однакові розміри оригінального та обробленого зображень
            {
                throw new ArgumentException("Розміри зображень повинні співпадати.");
            }

            double sum = 0; // сума квадратів різниць між пікселями

            int pixelCount = original.Width * original.Height; // загальна кількість пікселів

            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    Color originalPixel = original.GetPixel(x, y); // отримуємо піксель оригінального зображення

                    Color processedPixel = processed.GetPixel(x, y); // отримуємо піксель обробленого зображення

                    double originalGray = 0.299 * originalPixel.R + 0.587 * originalPixel.G + 0.114 * originalPixel.B; // обчислюємо яскравість пікселя оригінального зображення

                    double processedGray = 0.299 * processedPixel.R + 0.587 * processedPixel.G + 0.114 * processedPixel.B; // обчислюємо яскравість пікселя обробленого зображення

                    double difference = originalGray - processedGray; // знаходимо різницю між значеннями яскравості

                    sum += difference * difference; // додаємо квадрат різниці до загальної суми
                }
            }

            return sum / pixelCount; // повертаємо середнє значення квадратів різниць
        }

        private double CalculatePSNR(double mse) // обчислення пікового відношення сигналу до шуму
        {
            if (mse == 0) // якщо MSE дорівнює нулю, зображення повністю однакові
            {
                return double.PositiveInfinity; // повертаємо нескінченне значення PSNR
            }

            double maxPixelValue = 255.0; // максимальне значення яскравості одного пікселя

            double psnr = 10.0 * Math.Log10((maxPixelValue * maxPixelValue) / mse); // обчислюємо PSNR за формулою

            return psnr; // повертаємо значення PSNR у децибелах
        }

        private double CalculateSSIM(Bitmap original, Bitmap processed) // обчислення структурної схожості між зображеннями
        {
            if (original.Width != processed.Width || original.Height != processed.Height) // перевіряємо, чи однакові розміри оригінального та обробленого зображень
            {
                throw new ArgumentException("Розміри зображень повинні співпадати.");
            }

            int pixelCount = original.Width * original.Height; // загальна кількість пікселів

            double meanOriginal = 0; // середня яскравість оригінального зображення
            double meanProcessed = 0; // середня яскравість обробленого зображення

            for (int y = 0; y < original.Height; y++) // обчислюємо середню яскравість обох зображень
            {
                for (int x = 0; x < original.Width; x++)
                {
                    Color originalPixel = original.GetPixel(x, y); // отримуємо піксель оригінального зображення

                    Color processedPixel = processed.GetPixel(x, y); // отримуємо піксель обробленого зображення

                    double originalGray = 0.299 * originalPixel.R + 0.587 * originalPixel.G + 0.114 * originalPixel.B; // обчислюємо яскравість оригінального пікселя

                    double processedGray = 0.299 * processedPixel.R + 0.587 * processedPixel.G + 0.114 * processedPixel.B; // обчислюємо яскравість обробленого пікселя

                    meanOriginal += originalGray; // додаємо яскравість до загальної суми

                    meanProcessed += processedGray; // додаємо яскравість до загальної суми
                }
            }

            meanOriginal /= pixelCount; // знаходимо середню яскравість оригіналу

            meanProcessed /= pixelCount; // знаходимо середню яскравість обробленого зображення

            double varianceOriginal = 0; // дисперсія оригінального зображення

            double varianceProcessed = 0; // дисперсія обробленого зображення

            double covariance = 0; // коваріація між двома зображеннями

            for (int y = 0; y < original.Height; y++) // обчислюємо дисперсію та коваріацію
            {
                for (int x = 0; x < original.Width; x++)
                {
                    Color originalPixel = original.GetPixel(x, y);

                    Color processedPixel = processed.GetPixel(x, y);

                    double originalGray = 0.299 * originalPixel.R + 0.587 * originalPixel.G + 0.114 * originalPixel.B;

                    double processedGray = 0.299 * processedPixel.R + 0.587 * processedPixel.G + 0.114 * processedPixel.B;

                    double originalDifference = originalGray - meanOriginal; // відхилення від середнього значення

                    double processedDifference = processedGray - meanProcessed; // відхилення від середнього значення

                    varianceOriginal += originalDifference * originalDifference; // накопичуємо дисперсію оригіналу

                    varianceProcessed += processedDifference * processedDifference; // накопичуємо дисперсію результату

                    covariance += originalDifference * processedDifference; // накопичуємо коваріацію
                }
            }

            varianceOriginal /= pixelCount; // обчислюємо дисперсію оригіналу

            varianceProcessed /= pixelCount; // обчислюємо дисперсію результату

            covariance /= pixelCount; // обчислюємо коваріацію

            double c1 = Math.Pow(0.01 * 255, 2); // стабілізуюча константа для яскравості

            double c2 = Math.Pow(0.03 * 255, 2); // стабілізуюча константа для контрасту

            double numerator = (2 * meanOriginal * meanProcessed + c1) * (2 * covariance + c2); // обчислюємо чисельник формули SSIM

            double denominator = (meanOriginal * meanOriginal + meanProcessed * meanProcessed + c1) * (varianceOriginal + varianceProcessed + c2); // обчислюємо знаменник формули SSIM

            return numerator / denominator; // повертаємо значення SSIM
        }

        private Bitmap ProcessOtsuForMetrics(Bitmap original) // виконання пайплайна №3 для автоматичного аналізу метрик
        {
            using Bitmap grayImage = ConvertToGrayscale(original); // перетворюємо зображення у відтінки сірого

            int threshold = CalculateOtsuThreshold(grayImage); // автоматично визначаємо поріг методом Оцу

            using Bitmap otsuImage = new Bitmap(grayImage.Width, grayImage.Height); // створюємо бінарне зображення

            for (int y = 0; y < grayImage.Height; y++)
            {
                for (int x = 0; x < grayImage.Width; x++)
                {
                    Color pixel = grayImage.GetPixel(x, y); // отримуємо піксель сірого зображення

                    int brightness = pixel.R; // отримуємо значення яскравості

                    Color binaryColor;

                    if (brightness <= threshold) // якщо піксель темніший за знайдений поріг
                    {
                        binaryColor = Color.White; // позначаємо його білим кольором
                    }
                    else
                    {
                        binaryColor = Color.Black; // інші пікселі позначаємо чорним кольором
                    }

                    otsuImage.SetPixel(x, y, binaryColor); // записуємо бінарний піксель
                }
            }

            using Bitmap erodedImage = ApplyErosion(otsuImage); // виконуємо ерозію бінарного зображення

            Bitmap openedImage = ApplyDilation(erodedImage); // після ерозії виконуємо дилатацію

            return openedImage; // повертаємо результат морфологічного відкриття
        }

        private Bitmap ProcessHSVForMetrics(Bitmap original) // виконання пайплайна №4 для автоматичного аналізу метрик
        {
            double hMin = (double)numStemHMin.Value; // отримуємо мінімальний колірний тон

            double hMax = (double)numStemHMax.Value; // отримуємо максимальний колірний тон

            double sMin = (double)numStemSMin.Value; // отримуємо мінімальну насиченість

            double sMax = (double)numStemSMax.Value; // отримуємо максимальну насиченість

            double vMin = (double)numStemVMin.Value; // отримуємо мінімальну яскравість

            double vMax = (double)numStemVMax.Value; // отримуємо максимальну яскравість

            using Bitmap hsvMask = new Bitmap(original.Width, original.Height); // створюємо бінарну HSV-маску

            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    Color pixel = original.GetPixel(x, y); // отримуємо RGB-піксель

                    ConvertRGBtoHSV(pixel, out double h, out double s, out double v); // перетворюємо RGB у HSV

                    bool hueMatches = hMin <= hMax ? h >= hMin && h <= hMax : h >= hMin || h <= hMax; // перевіряємо відповідність колірного тону

                    bool saturationMatches = s >= sMin && s <= sMax; // перевіряємо відповідність насиченості

                    bool valueMatches = v >= vMin && v <= vMax; // перевіряємо відповідність яскравості

                    bool isSelected = hueMatches && saturationMatches && valueMatches; // визначаємо, чи належить піксель вибраному HSV-діапазону

                    if (chkStemHSVInvert.Checked) // перевіряємо необхідність інвертування маски
                    {
                        isSelected = !isSelected; // інвертуємо результат
                    }

                    Color newColor = isSelected ? Color.White : Color.Black; // створюємо білий або чорний піксель маски

                    hsvMask.SetPixel(x, y, newColor); // записуємо піксель у HSV-маску
                }
            }

            Bitmap erodedImage = ApplyErosion(hsvMask); // застосовуємо ерозію до HSV-маски

            return erodedImage; // повертаємо результат після ерозії
        }

        private void btnMetrics_Click(object sender, EventArgs e) // аналіз метрик для всіх завантажених фотографій
        {
            if (stemImageFiles.Length == 0) // перевіряємо, чи завантажена папка з фотографіями
            {
                MessageBox.Show("Спочатку завантажте папку з фотографіями.");
                return;
            }

            if (numStemSMin.Value > numStemSMax.Value || numStemVMin.Value > numStemVMax.Value) // перевіряємо правильність діапазонів S та V
            {
                MessageBox.Show("Перевірте діапазони S та V.");
                return;
            }

            dgvMetrics.Rows.Clear(); // очищаємо попередні результати з таблиці

            try
            {
                foreach (string filePath in stemImageFiles) // послідовно обробляємо всі фотографії із завантаженої папки
                {
                    string fileName = Path.GetFileName(filePath); // отримуємо назву поточного файла

                    using Bitmap source = new Bitmap(filePath); // відкриваємо поточну фотографію

                    using Bitmap original = new Bitmap(source); // створюємо копію оригінального зображення

                    // Пайплайн №3

                    using Bitmap otsuResult = ProcessOtsuForMetrics(original); // виконуємо метод Оцу та морфологічне відкриття

                    double mseP3 = CalculateMSE(original, otsuResult); // обчислюємо MSE для пайплайна №3

                    double psnrP3 = CalculatePSNR(mseP3); // обчислюємо PSNR для пайплайна №3

                    double ssimP3 = CalculateSSIM(original, otsuResult); // обчислюємо SSIM для пайплайна №3

                    dgvMetrics.Rows.Add(fileName, "Otsu + Opening", mseP3.ToString("F2"), double.IsPositiveInfinity(psnrP3) ? "∞" : psnrP3.ToString("F2"), ssimP3.ToString("F4")); // додаємо результати пайплайна №3 у таблицю

                    // Пайплайн №4

                    using Bitmap hsvResult = ProcessHSVForMetrics(original); // виконуємо HSV-сегментацію та ерозію

                    double mseP4 = CalculateMSE(original, hsvResult); // обчислюємо MSE для пайплайна №4

                    double psnrP4 = CalculatePSNR(mseP4); // обчислюємо PSNR для пайплайна №4

                    double ssimP4 = CalculateSSIM(original, hsvResult); // обчислюємо SSIM для пайплайна №4

                    dgvMetrics.Rows.Add(fileName, "HSV + Erosion", mseP4.ToString("F2"), double.IsPositiveInfinity(psnrP4) ? "∞" : psnrP4.ToString("F2"), ssimP4.ToString("F4")); // додаємо результати пайплайна №4 у таблицю
                }

                MessageBox.Show($"Аналіз завершено.\nОброблено фотографій: {stemImageFiles.Length}"); // повідомляємо кількість проаналізованих фотографій
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка під час аналізу метрик: " + ex.Message); // повідомляємо про помилку під час аналізу
            }
        }
    }
}