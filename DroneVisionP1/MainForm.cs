using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text;

namespace DroneVisionP1;

public sealed class MainForm : Form
{
    private readonly TextBox input = new() { Width = 480, ReadOnly = true };
    private readonly TextBox output = new() { Width = 480, ReadOnly = true };
    private readonly ComboBox pipeline = new() { Width = 230, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly NumericUpDown threshold = new() { Minimum = 0, Maximum = 255, Value = 80, Width = 75 };
    private readonly NumericUpDown hMin = new() { Minimum = 0, Maximum = 360, Value = 0, Width = 55 };
    private readonly NumericUpDown hMax = new() { Minimum = 0, Maximum = 360, Value = 360, Width = 55 };
    private readonly NumericUpDown sMin = new() { Minimum = 0, Maximum = 100, Value = 0, Width = 55 };
    private readonly NumericUpDown sMax = new() { Minimum = 0, Maximum = 100, Value = 100, Width = 55 };
    private readonly NumericUpDown vMin = new() { Minimum = 0, Maximum = 100, Value = 0, Width = 55 };
    private readonly NumericUpDown vMax = new() { Minimum = 0, Maximum = 100, Value = 100, Width = 55 };
    private readonly CheckBox invertHsv = new() { Text = "Інвертувати HSV", AutoSize = true };
    private readonly Label status = new() { AutoSize = true, Text = "Оберіть зображення або папку." };
    private readonly PictureBox[] previews = new PictureBox[4];
    private readonly Label[] names = new Label[4];
    private readonly DataGridView metricsTable = new()
    {
        Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        RowHeadersVisible = false
    };
    private bool isFolder;

    public MainForm()
    {
        Text = "STEM | Дрони — 4 пайплайни";
        Width = 1300; Height = 850; MinimumSize = new Size(900, 650);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 10);

        pipeline.Items.AddRange(new object[] { "Усі 4 пайплайни", "№1 — Прюітт", "№2 — Собель", "№3 — Оцу + відкриття", "№4 — HSV + ерозія" });
        pipeline.SelectedIndex = 0;
        output.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DroneResults");

        var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, Padding = new Padding(12) };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 165));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 64));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 36));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        Controls.Add(root);

        var controls = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, WrapContents = true };
        root.Controls.Add(controls, 0, 0);
        controls.Controls.Add(MakeButton("Файл", () => ChooseFile()));
        controls.Controls.Add(MakeButton("Папка", () => ChooseFolder()));
        controls.Controls.Add(input);
        controls.Controls.Add(MakeButton("Куди зберігати", () => ChooseOutput()));
        controls.Controls.Add(output);
        controls.Controls.Add(new Label { Text = "Пайплайн:", AutoSize = true, Margin = new Padding(8, 9, 2, 0) });
        controls.Controls.Add(pipeline);
        controls.Controls.Add(new Label { Text = "Поріг:", AutoSize = true, Margin = new Padding(8, 9, 2, 0) });
        controls.Controls.Add(threshold);
        controls.Controls.Add(new Label { Text = "HSV H від/до:", AutoSize = true, Margin = new Padding(8, 9, 2, 0) });
        controls.Controls.Add(hMin); controls.Controls.Add(hMax);
        controls.Controls.Add(new Label { Text = "S від/до:", AutoSize = true, Margin = new Padding(8, 9, 2, 0) });
        controls.Controls.Add(sMin); controls.Controls.Add(sMax);
        controls.Controls.Add(new Label { Text = "V від/до:", AutoSize = true, Margin = new Padding(8, 9, 2, 0) });
        controls.Controls.Add(vMin); controls.Controls.Add(vMax);
        controls.Controls.Add(invertHsv);
        controls.Controls.Add(MakeButton("Запустити обробку", async () => await ProcessAsync()));

        var pictures = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 1 };
        for (int i = 0; i < 4; i++) pictures.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        root.Controls.Add(pictures, 0, 1);
        for (int i = 0; i < 4; i++)
        {
            var group = new GroupBox { Dock = DockStyle.Fill, Text = i == 0 ? "Оригінал" : $"Етап {i}", Padding = new Padding(8) };
            var box = new PictureBox { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.FromArgb(238, 241, 245) };
            previews[i] = box;
            names[i] = new Label { Dock = DockStyle.Bottom, Height = 30, TextAlign = ContentAlignment.MiddleCenter };
            group.Controls.Add(box);
            group.Controls.Add(names[i]);
            pictures.Controls.Add(group, i, 0);
        }
        metricsTable.Columns.Add("file", "Файл");
        metricsTable.Columns.Add("pipe", "Пайплайн");
        metricsTable.Columns.Add("mse", "MSE");
        metricsTable.Columns.Add("psnr", "PSNR, дБ");
        metricsTable.Columns.Add("ssim", "SSIM (глобальний)");
        root.Controls.Add(metricsTable, 0, 2);
        root.Controls.Add(status, 0, 3);
    }

    private static Button MakeButton(string title, Action action)
    {
        var button = new Button { Text = title, AutoSize = true, Height = 33, Margin = new Padding(3) };
        button.Click += (_, _) => action();
        return button;
    }

    private void ChooseFile()
    {
        using var dialog = new OpenFileDialog { Filter = "Зображення|*.jpg;*.jpeg;*.png;*.bmp" };
        if (dialog.ShowDialog() == DialogResult.OK) { input.Text = dialog.FileName; isFolder = false; }
    }
    private void ChooseFolder()
    {
        using var dialog = new FolderBrowserDialog();
        if (dialog.ShowDialog() == DialogResult.OK) { input.Text = dialog.SelectedPath; isFolder = true; }
    }
    private void ChooseOutput()
    {
        using var dialog = new FolderBrowserDialog();
        if (dialog.ShowDialog() == DialogResult.OK) output.Text = dialog.SelectedPath;
    }

    private async Task ProcessAsync()
    {
        string path = input.Text, destination = output.Text;
        if (string.IsNullOrWhiteSpace(path) || (!File.Exists(path) && !Directory.Exists(path)))
        { MessageBox.Show("Оберіть існуючий файл або папку."); return; }
        if (string.IsNullOrWhiteSpace(destination)) { MessageBox.Show("Оберіть папку результатів."); return; }
        int level = (int)threshold.Value;
        int[] selected = pipeline.SelectedIndex switch { 1 => new[] { 1 }, 2 => new[] { 2 }, 3 => new[] { 3 }, 4 => new[] { 4 }, _ => new[] { 1, 2, 3, 4 } };
        if (sMin.Value > sMax.Value || vMin.Value > vMax.Value)
        { MessageBox.Show("Перевірте діапазони HSV S та V."); return; }
        var hsv = new HsvOptions((double)hMin.Value, (double)hMax.Value, (double)sMin.Value, (double)sMax.Value,
            (double)vMin.Value, (double)vMax.Value, invertHsv.Checked);
        string[] paths = isFolder
            ? Directory.GetFiles(path).Where(p => new[] { ".jpg", ".jpeg", ".png", ".bmp" }.Contains(Path.GetExtension(p).ToLowerInvariant())).OrderBy(p => p).ToArray()
            : new[] { path };
        if (paths.Length == 0) { MessageBox.Show("У папці немає зображень."); return; }
        Enabled = false;
        metricsTable.Rows.Clear();
        status.Text = $"Обробка {paths.Length} зображень...";
        try
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var progress = new Progress<string>(message => status.Text = message);
            var results = await Task.Run(() => ProcessFiles(paths, destination, selected, level, hsv, progress));
            watch.Stop();
            foreach (var r in results.Rows)
                metricsTable.Rows.Add(r.File, r.Pipeline, r.Mse, r.Psnr, r.Ssim);
            if (results.PreviewFile is not null)
            {
                SetPreview(0, results.PreviewFile, "Вхідне зображення");
                for (int i = 0; i < 3; i++)
                    SetPreview(i + 1, results.PreviewStages![i], results.PreviewNames![i]);
            }
            status.Text = $"Готово за {watch.Elapsed:mm\\:ss}: {results.Rows.Count} результатів. CSV: {Path.Combine(destination, "metrics.csv")}. Помилок: {results.Errors.Count}.";
            if (results.Errors.Count > 0)
                MessageBox.Show("Частину файлів пропущено:\n" + string.Join("\n", results.Errors.Take(8)), "Помилки");
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        finally { Enabled = true; }
    }

    private static BatchResult ProcessFiles(string[] paths, string destination, int[] selected, int level, HsvOptions hsv, IProgress<string> progress)
    {
        Directory.CreateDirectory(destination);
        var result = new BatchResult();
        var csv = new StringBuilder("image,pipeline,threshold,MSE,PSNR_dB,SSIM_global\r\n");
        var batchTimer = System.Diagnostics.Stopwatch.StartNew();
        for (int fileIndex = 0; fileIndex < paths.Length; fileIndex++)
        {
            string path = paths[fileIndex];
            progress.Report($"Обробка {fileIndex + 1}/{paths.Length} | Минуло {batchTimer.Elapsed:mm\\:ss} | {Path.GetFileName(path)}");
            try
            {
                using var original = new Bitmap(path);
                byte[,] reference = ImageAlgorithms.Grayscale(original);
                foreach (int id in selected)
                {
                    progress.Report($"Обробка {fileIndex + 1}/{paths.Length}, пайплайн №{id} | Минуло {batchTimer.Elapsed:mm\\:ss}");
                    var stages = id == 4
                        ? TeammateAlgorithms.RunHsv(original, hsv.HMin, hsv.HMax, hsv.SMin, hsv.SMax, hsv.VMin, hsv.VMax, hsv.Invert)
                        : ImageAlgorithms.Run(reference, id, level);
                    // Додаємо номер файлу, щоб однакові назви не перезаписували результати.
                    string stem = Path.GetFileNameWithoutExtension(path);
                    string folder = Path.Combine(destination, stem, $"pipeline_{id}");
                    Directory.CreateDirectory(folder);
                    var saved = new string[3];
                    for (int i = 0; i < stages.Count; i++)
                    {
                        saved[i] = Path.Combine(folder, stages[i].Name + ".png");
                        using var bitmap = ImageAlgorithms.ToBitmap(stages[i].Data);
                        bitmap.Save(saved[i], ImageFormat.Png);
                    }
                    var m = ImageAlgorithms.Metrics(reference, stages[^1].Data);
                    string psnr = double.IsPositiveInfinity(m.Psnr) ? "Infinity" : m.Psnr.ToString("F5", CultureInfo.InvariantCulture);
                    result.Rows.Add((Path.GetFileName(path), $"№{id}", m.Mse.ToString("F5"), psnr, m.Ssim.ToString("F5")));
                    csv.AppendLine($"\"{Path.GetFileName(path).Replace("\"", "\"\"")}\",{id},{level},{m.Mse.ToString("F5", CultureInfo.InvariantCulture)},{psnr},{m.Ssim.ToString("F5", CultureInfo.InvariantCulture)}");
                    if (result.PreviewFile is null)
                    {
                        result.PreviewFile = path;
                        result.PreviewStages = saved;
                        result.PreviewNames = stages.Select(s => s.Name).ToArray();
                    }
                }
            }
            catch (Exception ex) { result.Errors.Add($"{Path.GetFileName(path)}: {ex.Message}"); }
        }
        File.WriteAllText(Path.Combine(destination, "metrics.csv"), csv.ToString(), new UTF8Encoding(true));
        return result;
    }

    private void SetPreview(int index, string path, string label)
    {
        using var loaded = new Bitmap(path);
        var previous = previews[index].Image;
        previews[index].Image = new Bitmap(loaded);
        previous?.Dispose();
        names[index].Text = label;
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        foreach (var preview in previews) preview.Image?.Dispose();
        base.OnFormClosed(e);
    }

    private sealed class BatchResult
    {
        public List<(string File, string Pipeline, string Mse, string Psnr, string Ssim)> Rows { get; } = new();
        public List<string> Errors { get; } = new();
        public string? PreviewFile { get; set; }
        public string[]? PreviewStages { get; set; }
        public string[]? PreviewNames { get; set; }
    }

    private sealed record HsvOptions(double HMin, double HMax, double SMin, double SMax,
        double VMin, double VMax, bool Invert);
}
