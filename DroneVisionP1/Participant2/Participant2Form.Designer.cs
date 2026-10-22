namespace DroneVisionP1
{
    partial class Participant2Form
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pictureBoxStemOriginal = new PictureBox();
            pictureBoxStemOpening = new PictureBox();
            pictureBoxStemGray = new PictureBox();
            pictureBoxStemOtsu = new PictureBox();
            btnStemLoad = new Button();
            btnStemGrayscale = new Button();
            cmbStemImage = new ComboBox();
            btnStemOtsu = new Button();
            lblStemOtsuThreshold = new Label();
            btnStemOpening = new Button();
            groupBox1 = new GroupBox();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            groupBox2 = new GroupBox();
            btnStemSaveAll = new Button();
            btnStemHSVErosion = new Button();
            btnStemSegmentHSV = new Button();
            chkStemHSVInvert = new CheckBox();
            label13 = new Label();
            label12 = new Label();
            label11 = new Label();
            label10 = new Label();
            label9 = new Label();
            label8 = new Label();
            label7 = new Label();
            numStemVMax = new NumericUpDown();
            numStemVMin = new NumericUpDown();
            numStemSMax = new NumericUpDown();
            numStemSMin = new NumericUpDown();
            numStemHMax = new NumericUpDown();
            numStemHMin = new NumericUpDown();
            label6 = new Label();
            label5 = new Label();
            pictureBoxStemHSVEroded = new PictureBox();
            pictureBoxStemHSVMask = new PictureBox();
            dgvMetrics = new DataGridView();
            colFile = new DataGridViewTextBoxColumn();
            colPipeline = new DataGridViewTextBoxColumn();
            colMSE = new DataGridViewTextBoxColumn();
            colPSNR = new DataGridViewTextBoxColumn();
            colSSIM = new DataGridViewTextBoxColumn();
            btnMetrics = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStemOriginal).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStemOpening).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStemGray).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStemOtsu).BeginInit();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numStemVMax).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numStemVMin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numStemSMax).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numStemSMin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numStemHMax).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numStemHMin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStemHSVEroded).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStemHSVMask).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvMetrics).BeginInit();
            SuspendLayout();
            // 
            // pictureBoxStemOriginal
            // 
            pictureBoxStemOriginal.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxStemOriginal.Location = new Point(7, 35);
            pictureBoxStemOriginal.Name = "pictureBoxStemOriginal";
            pictureBoxStemOriginal.Size = new Size(250, 250);
            pictureBoxStemOriginal.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxStemOriginal.TabIndex = 0;
            pictureBoxStemOriginal.TabStop = false;
            // 
            // pictureBoxStemOpening
            // 
            pictureBoxStemOpening.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxStemOpening.Location = new Point(775, 35);
            pictureBoxStemOpening.Name = "pictureBoxStemOpening";
            pictureBoxStemOpening.Size = new Size(250, 250);
            pictureBoxStemOpening.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxStemOpening.TabIndex = 1;
            pictureBoxStemOpening.TabStop = false;
            // 
            // pictureBoxStemGray
            // 
            pictureBoxStemGray.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxStemGray.Location = new Point(263, 35);
            pictureBoxStemGray.Name = "pictureBoxStemGray";
            pictureBoxStemGray.Size = new Size(250, 250);
            pictureBoxStemGray.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxStemGray.TabIndex = 2;
            pictureBoxStemGray.TabStop = false;
            // 
            // pictureBoxStemOtsu
            // 
            pictureBoxStemOtsu.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxStemOtsu.Location = new Point(519, 35);
            pictureBoxStemOtsu.Name = "pictureBoxStemOtsu";
            pictureBoxStemOtsu.Size = new Size(250, 250);
            pictureBoxStemOtsu.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxStemOtsu.TabIndex = 3;
            pictureBoxStemOtsu.TabStop = false;
            // 
            // btnStemLoad
            // 
            btnStemLoad.Location = new Point(39, 291);
            btnStemLoad.Name = "btnStemLoad";
            btnStemLoad.Size = new Size(186, 23);
            btnStemLoad.TabIndex = 4;
            btnStemLoad.Text = "Завантажити папку з фото";
            btnStemLoad.UseVisualStyleBackColor = true;
            btnStemLoad.Click += btnStemLoad_Click;
            // 
            // btnStemGrayscale
            // 
            btnStemGrayscale.Location = new Point(297, 291);
            btnStemGrayscale.Name = "btnStemGrayscale";
            btnStemGrayscale.Size = new Size(188, 23);
            btnStemGrayscale.TabIndex = 5;
            btnStemGrayscale.Text = "Перетворити у відтінки сірого";
            btnStemGrayscale.UseVisualStyleBackColor = true;
            btnStemGrayscale.Click += btnStemGrayscale_Click;
            // 
            // cmbStemImage
            // 
            cmbStemImage.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStemImage.FormattingEnabled = true;
            cmbStemImage.Location = new Point(68, 320);
            cmbStemImage.MaximumSize = new Size(230, 0);
            cmbStemImage.Name = "cmbStemImage";
            cmbStemImage.Size = new Size(121, 23);
            cmbStemImage.TabIndex = 6;
            cmbStemImage.SelectedIndexChanged += cmbStemImage_SelectedIndexChanged;
            // 
            // btnStemOtsu
            // 
            btnStemOtsu.Location = new Point(563, 291);
            btnStemOtsu.Name = "btnStemOtsu";
            btnStemOtsu.Size = new Size(161, 23);
            btnStemOtsu.TabIndex = 7;
            btnStemOtsu.Text = "Застосувати метод Оцу";
            btnStemOtsu.UseVisualStyleBackColor = true;
            btnStemOtsu.Click += btnStemOtsu_Click;
            // 
            // lblStemOtsuThreshold
            // 
            lblStemOtsuThreshold.AutoSize = true;
            lblStemOtsuThreshold.Location = new Point(608, 320);
            lblStemOtsuThreshold.Name = "lblStemOtsuThreshold";
            lblStemOtsuThreshold.Size = new Size(74, 15);
            lblStemOtsuThreshold.TabIndex = 8;
            lblStemOtsuThreshold.Text = "Поріг Оцу: -";
            // 
            // btnStemOpening
            // 
            btnStemOpening.Location = new Point(831, 291);
            btnStemOpening.Name = "btnStemOpening";
            btnStemOpening.Size = new Size(157, 23);
            btnStemOpening.TabIndex = 9;
            btnStemOpening.Text = "Морфологічне відкриття";
            btnStemOpening.UseVisualStyleBackColor = true;
            btnStemOpening.Click += btnStemOpening_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(pictureBoxStemOtsu);
            groupBox1.Controls.Add(btnStemOpening);
            groupBox1.Controls.Add(pictureBoxStemOriginal);
            groupBox1.Controls.Add(lblStemOtsuThreshold);
            groupBox1.Controls.Add(pictureBoxStemOpening);
            groupBox1.Controls.Add(btnStemOtsu);
            groupBox1.Controls.Add(pictureBoxStemGray);
            groupBox1.Controls.Add(cmbStemImage);
            groupBox1.Controls.Add(btnStemLoad);
            groupBox1.Controls.Add(btnStemGrayscale);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(1037, 354);
            groupBox1.TabIndex = 10;
            groupBox1.TabStop = false;
            groupBox1.Text = "Пайплайн №3";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(831, 17);
            label4.Name = "label4";
            label4.Size = new Size(144, 15);
            label4.TabIndex = 15;
            label4.Text = "Морфологічне відкриття";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(615, 17);
            label3.Name = "label3";
            label3.Size = new Size(67, 15);
            label3.TabIndex = 14;
            label3.Text = "Метод Оцу";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(340, 17);
            label2.Name = "label2";
            label2.Size = new Size(89, 15);
            label2.TabIndex = 13;
            label2.Text = "Відтінки сірого";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(56, 17);
            label1.Name = "label1";
            label1.Size = new Size(147, 15);
            label1.TabIndex = 12;
            label1.Text = "Оригінальне зображення";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(btnStemSaveAll);
            groupBox2.Controls.Add(btnStemHSVErosion);
            groupBox2.Controls.Add(btnStemSegmentHSV);
            groupBox2.Controls.Add(chkStemHSVInvert);
            groupBox2.Controls.Add(label13);
            groupBox2.Controls.Add(label12);
            groupBox2.Controls.Add(label11);
            groupBox2.Controls.Add(label10);
            groupBox2.Controls.Add(label9);
            groupBox2.Controls.Add(label8);
            groupBox2.Controls.Add(label7);
            groupBox2.Controls.Add(numStemVMax);
            groupBox2.Controls.Add(numStemVMin);
            groupBox2.Controls.Add(numStemSMax);
            groupBox2.Controls.Add(numStemSMin);
            groupBox2.Controls.Add(numStemHMax);
            groupBox2.Controls.Add(numStemHMin);
            groupBox2.Controls.Add(label6);
            groupBox2.Controls.Add(label5);
            groupBox2.Controls.Add(pictureBoxStemHSVEroded);
            groupBox2.Controls.Add(pictureBoxStemHSVMask);
            groupBox2.Location = new Point(12, 372);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(1037, 322);
            groupBox2.TabIndex = 11;
            groupBox2.TabStop = false;
            groupBox2.Text = "Пайплайн №4";
            // 
            // btnStemSaveAll
            // 
            btnStemSaveAll.Location = new Point(851, 27);
            btnStemSaveAll.Name = "btnStemSaveAll";
            btnStemSaveAll.Size = new Size(137, 23);
            btnStemSaveAll.TabIndex = 20;
            btnStemSaveAll.Text = "Зберегти результати";
            btnStemSaveAll.UseVisualStyleBackColor = true;
            btnStemSaveAll.Click += btnStemSaveAll_Click;
            // 
            // btnStemHSVErosion
            // 
            btnStemHSVErosion.Location = new Point(597, 285);
            btnStemHSVErosion.Name = "btnStemHSVErosion";
            btnStemHSVErosion.Size = new Size(137, 23);
            btnStemHSVErosion.TabIndex = 19;
            btnStemHSVErosion.Text = "Застосувати ерозію";
            btnStemHSVErosion.UseVisualStyleBackColor = true;
            btnStemHSVErosion.Click += btnStemHSVErosion_Click;
            // 
            // btnStemSegmentHSV
            // 
            btnStemSegmentHSV.Location = new Point(283, 260);
            btnStemSegmentHSV.Name = "btnStemSegmentHSV";
            btnStemSegmentHSV.Size = new Size(171, 23);
            btnStemSegmentHSV.TabIndex = 18;
            btnStemSegmentHSV.Text = "Виконати HSV-сегментацію";
            btnStemSegmentHSV.UseVisualStyleBackColor = true;
            btnStemSegmentHSV.Click += btnStemSegmentHSV_Click;
            // 
            // chkStemHSVInvert
            // 
            chkStemHSVInvert.AutoSize = true;
            chkStemHSVInvert.Location = new Point(305, 225);
            chkStemHSVInvert.Name = "chkStemHSVInvert";
            chkStemHSVInvert.Size = new Size(126, 19);
            chkStemHSVInvert.TabIndex = 17;
            chkStemHSVInvert.Text = "Інвертувати маску";
            chkStemHSVInvert.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(262, 182);
            label13.Name = "label13";
            label13.Size = new Size(135, 15);
            label13.TabIndex = 16;
            label13.Text = "Яскравість максимум : ";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(263, 151);
            label12.Name = "label12";
            label12.Size = new Size(120, 15);
            label12.TabIndex = 15;
            label12.Text = "Яскравість мінімум :";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(261, 124);
            label11.Name = "label11";
            label11.Size = new Size(145, 15);
            label11.TabIndex = 14;
            label11.Text = "Насиченість максимум : ";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(261, 95);
            label10.Name = "label10";
            label10.Size = new Size(130, 15);
            label10.TabIndex = 13;
            label10.Text = "Насиченість мінімум :";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(261, 64);
            label9.Name = "label9";
            label9.Size = new Size(151, 15);
            label9.TabIndex = 12;
            label9.Text = "Колірний тон максимум : ";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(261, 37);
            label8.Name = "label8";
            label8.Size = new Size(136, 15);
            label8.TabIndex = 11;
            label8.Text = "Колірний тон мінімум :";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(421, 11);
            label7.Name = "label7";
            label7.Size = new Size(97, 15);
            label7.TabIndex = 10;
            label7.Text = "Вибір діапазонів";
            // 
            // numStemVMax
            // 
            numStemVMax.Location = new Point(412, 174);
            numStemVMax.Name = "numStemVMax";
            numStemVMax.Size = new Size(120, 23);
            numStemVMax.TabIndex = 9;
            numStemVMax.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // numStemVMin
            // 
            numStemVMin.Location = new Point(412, 145);
            numStemVMin.Name = "numStemVMin";
            numStemVMin.Size = new Size(120, 23);
            numStemVMin.TabIndex = 8;
            // 
            // numStemSMax
            // 
            numStemSMax.Location = new Point(412, 116);
            numStemSMax.Name = "numStemSMax";
            numStemSMax.Size = new Size(120, 23);
            numStemSMax.TabIndex = 7;
            numStemSMax.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // numStemSMin
            // 
            numStemSMin.Location = new Point(412, 87);
            numStemSMin.Name = "numStemSMin";
            numStemSMin.Size = new Size(120, 23);
            numStemSMin.TabIndex = 6;
            // 
            // numStemHMax
            // 
            numStemHMax.Location = new Point(412, 58);
            numStemHMax.Maximum = new decimal(new int[] { 360, 0, 0, 0 });
            numStemHMax.Name = "numStemHMax";
            numStemHMax.Size = new Size(120, 23);
            numStemHMax.TabIndex = 5;
            numStemHMax.Value = new decimal(new int[] { 360, 0, 0, 0 });
            // 
            // numStemHMin
            // 
            numStemHMin.Location = new Point(412, 29);
            numStemHMin.Maximum = new decimal(new int[] { 360, 0, 0, 0 });
            numStemHMin.Name = "numStemHMin";
            numStemHMin.Size = new Size(120, 23);
            numStemHMin.TabIndex = 4;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(608, 13);
            label6.Name = "label6";
            label6.Size = new Size(126, 15);
            label6.TabIndex = 3;
            label6.Text = "Результат після ерозії";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(100, 17);
            label5.Name = "label5";
            label5.Size = new Size(67, 15);
            label5.TabIndex = 2;
            label5.Text = "Маска HSV";
            // 
            // pictureBoxStemHSVEroded
            // 
            pictureBoxStemHSVEroded.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxStemHSVEroded.Location = new Point(538, 29);
            pictureBoxStemHSVEroded.Name = "pictureBoxStemHSVEroded";
            pictureBoxStemHSVEroded.Size = new Size(250, 250);
            pictureBoxStemHSVEroded.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxStemHSVEroded.TabIndex = 1;
            pictureBoxStemHSVEroded.TabStop = false;
            // 
            // pictureBoxStemHSVMask
            // 
            pictureBoxStemHSVMask.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxStemHSVMask.Location = new Point(7, 35);
            pictureBoxStemHSVMask.Name = "pictureBoxStemHSVMask";
            pictureBoxStemHSVMask.Size = new Size(250, 250);
            pictureBoxStemHSVMask.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxStemHSVMask.TabIndex = 0;
            pictureBoxStemHSVMask.TabStop = false;
            // 
            // dgvMetrics
            // 
            dgvMetrics.AllowUserToAddRows = false;
            dgvMetrics.AllowUserToDeleteRows = false;
            dgvMetrics.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMetrics.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMetrics.Columns.AddRange(new DataGridViewColumn[] { colFile, colPipeline, colMSE, colPSNR, colSSIM });
            dgvMetrics.Location = new Point(12, 726);
            dgvMetrics.Name = "dgvMetrics";
            dgvMetrics.ReadOnly = true;
            dgvMetrics.RowHeadersVisible = false;
            dgvMetrics.Size = new Size(1037, 262);
            dgvMetrics.TabIndex = 12;
            // 
            // colFile
            // 
            colFile.HeaderText = "Файл";
            colFile.Name = "colFile";
            colFile.ReadOnly = true;
            // 
            // colPipeline
            // 
            colPipeline.HeaderText = "Пайплайн";
            colPipeline.Name = "colPipeline";
            colPipeline.ReadOnly = true;
            // 
            // colMSE
            // 
            colMSE.HeaderText = "MSE";
            colMSE.Name = "colMSE";
            colMSE.ReadOnly = true;
            // 
            // colPSNR
            // 
            colPSNR.HeaderText = "PSNR, дБ";
            colPSNR.Name = "colPSNR";
            colPSNR.ReadOnly = true;
            // 
            // colSSIM
            // 
            colSSIM.HeaderText = "SSIM (глобальний)";
            colSSIM.Name = "colSSIM";
            colSSIM.ReadOnly = true;
            // 
            // btnMetrics
            // 
            btnMetrics.Location = new Point(473, 694);
            btnMetrics.Name = "btnMetrics";
            btnMetrics.Size = new Size(137, 26);
            btnMetrics.TabIndex = 14;
            btnMetrics.Text = "Аналіз метриків";
            btnMetrics.UseVisualStyleBackColor = true;
            btnMetrics.Click += btnMetrics_Click;
            // 
            // Participant2Form
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1061, 1000);
            Controls.Add(btnMetrics);
            Controls.Add(dgvMetrics);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Name = "Participant2Form";
            Text = "STEM - Пайплайни №3 та №4";
            ((System.ComponentModel.ISupportInitialize)pictureBoxStemOriginal).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStemOpening).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStemGray).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStemOtsu).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numStemVMax).EndInit();
            ((System.ComponentModel.ISupportInitialize)numStemVMin).EndInit();
            ((System.ComponentModel.ISupportInitialize)numStemSMax).EndInit();
            ((System.ComponentModel.ISupportInitialize)numStemSMin).EndInit();
            ((System.ComponentModel.ISupportInitialize)numStemHMax).EndInit();
            ((System.ComponentModel.ISupportInitialize)numStemHMin).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStemHSVEroded).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStemHSVMask).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvMetrics).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBoxStemOriginal;
        private PictureBox pictureBoxStemOpening;
        private PictureBox pictureBoxStemGray;
        private PictureBox pictureBoxStemOtsu;
        private Button btnStemLoad;
        private Button btnStemGrayscale;
        private ComboBox cmbStemImage;
        private Button btnStemOtsu;
        private Label lblStemOtsuThreshold;
        private Button btnStemOpening;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private Label label6;
        private Label label5;
        private PictureBox pictureBoxStemHSVEroded;
        private PictureBox pictureBoxStemHSVMask;
        private CheckBox chkStemHSVInvert;
        private Label label13;
        private Label label12;
        private Label label11;
        private Label label10;
        private Label label9;
        private Label label8;
        private Label label7;
        private NumericUpDown numStemVMax;
        private NumericUpDown numStemVMin;
        private NumericUpDown numStemSMax;
        private NumericUpDown numStemSMin;
        private NumericUpDown numStemHMax;
        private NumericUpDown numStemHMin;
        private Button btnStemSegmentHSV;
        private Button btnStemHSVErosion;
        private Button btnStemSaveAll;
        private DataGridView dgvMetrics;
        private DataGridViewTextBoxColumn colFile;
        private DataGridViewTextBoxColumn colPipeline;
        private DataGridViewTextBoxColumn colMSE;
        private DataGridViewTextBoxColumn colPSNR;
        private DataGridViewTextBoxColumn colSSIM;
        private Button btnMetrics;
    }
}
