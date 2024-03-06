namespace AudioSample
{
    partial class MainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.StartButton = new System.Windows.Forms.Button();
            this.PlotPanel = new System.Windows.Forms.Panel();
            this.DeviceComboBox = new System.Windows.Forms.ComboBox();
            this.RunningProgress = new System.Windows.Forms.ProgressBar();
            this.LogTextBox = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.PeakFreqLabel = new System.Windows.Forms.Label();
            this.PeakLevelLabel = new System.Windows.Forms.Label();
            this.RefreshButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.AverageFreqLabel = new System.Windows.Forms.Label();
            this.AverageLevelLabel = new System.Windows.Forms.Label();
            this.FormatComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // StartButton
            // 
            this.StartButton.Location = new System.Drawing.Point(613, 10);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(75, 23);
            this.StartButton.TabIndex = 0;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // PlotPanel
            // 
            this.PlotPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PlotPanel.BackColor = System.Drawing.SystemColors.ControlDark;
            this.PlotPanel.Location = new System.Drawing.Point(12, 94);
            this.PlotPanel.Name = "PlotPanel";
            this.PlotPanel.Size = new System.Drawing.Size(776, 292);
            this.PlotPanel.TabIndex = 1;
            // 
            // DeviceComboBox
            // 
            this.DeviceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DeviceComboBox.FormattingEnabled = true;
            this.DeviceComboBox.Location = new System.Drawing.Point(12, 12);
            this.DeviceComboBox.Name = "DeviceComboBox";
            this.DeviceComboBox.Size = new System.Drawing.Size(360, 20);
            this.DeviceComboBox.TabIndex = 2;
            this.DeviceComboBox.SelectedValueChanged += new System.EventHandler(this.DeviceComboBox_SelectedValueChanged);
            // 
            // RunningProgress
            // 
            this.RunningProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RunningProgress.Location = new System.Drawing.Point(12, 530);
            this.RunningProgress.Name = "RunningProgress";
            this.RunningProgress.Size = new System.Drawing.Size(776, 23);
            this.RunningProgress.TabIndex = 4;
            // 
            // LogTextBox
            // 
            this.LogTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LogTextBox.Location = new System.Drawing.Point(12, 392);
            this.LogTextBox.Name = "LogTextBox";
            this.LogTextBox.ReadOnly = true;
            this.LogTextBox.Size = new System.Drawing.Size(776, 132);
            this.LogTextBox.TabIndex = 5;
            this.LogTextBox.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "Peak Frequency :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(42, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "Peak Level :";
            // 
            // PeakFreqLabel
            // 
            this.PeakFreqLabel.AutoSize = true;
            this.PeakFreqLabel.Location = new System.Drawing.Point(123, 45);
            this.PeakFreqLabel.Name = "PeakFreqLabel";
            this.PeakFreqLabel.Size = new System.Drawing.Size(46, 12);
            this.PeakFreqLabel.TabIndex = 8;
            this.PeakFreqLabel.Text = "0.00 Hz";
            // 
            // PeakLevelLabel
            // 
            this.PeakLevelLabel.AutoSize = true;
            this.PeakLevelLabel.Location = new System.Drawing.Point(123, 68);
            this.PeakLevelLabel.Name = "PeakLevelLabel";
            this.PeakLevelLabel.Size = new System.Drawing.Size(27, 12);
            this.PeakLevelLabel.TabIndex = 9;
            this.PeakLevelLabel.Text = "0.00";
            // 
            // RefreshButton
            // 
            this.RefreshButton.Location = new System.Drawing.Point(694, 10);
            this.RefreshButton.Name = "RefreshButton";
            this.RefreshButton.Size = new System.Drawing.Size(75, 23);
            this.RefreshButton.TabIndex = 10;
            this.RefreshButton.Text = "Refresh";
            this.RefreshButton.UseVisualStyleBackColor = true;
            this.RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(227, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 12);
            this.label3.TabIndex = 11;
            this.label3.Text = "Average Frequency :";
            this.label3.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(257, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 12);
            this.label4.TabIndex = 12;
            this.label4.Text = "Average Level :";
            // 
            // AverageFreqLabel
            // 
            this.AverageFreqLabel.AutoSize = true;
            this.AverageFreqLabel.Location = new System.Drawing.Point(356, 45);
            this.AverageFreqLabel.Name = "AverageFreqLabel";
            this.AverageFreqLabel.Size = new System.Drawing.Size(46, 12);
            this.AverageFreqLabel.TabIndex = 13;
            this.AverageFreqLabel.Text = "0.00 Hz";
            this.AverageFreqLabel.Visible = false;
            // 
            // AverageLevelLabel
            // 
            this.AverageLevelLabel.AutoSize = true;
            this.AverageLevelLabel.Location = new System.Drawing.Point(356, 68);
            this.AverageLevelLabel.Name = "AverageLevelLabel";
            this.AverageLevelLabel.Size = new System.Drawing.Size(27, 12);
            this.AverageLevelLabel.TabIndex = 14;
            this.AverageLevelLabel.Text = "0.00";
            // 
            // FormatComboBox
            // 
            this.FormatComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FormatComboBox.FormattingEnabled = true;
            this.FormatComboBox.Location = new System.Drawing.Point(378, 12);
            this.FormatComboBox.Name = "FormatComboBox";
            this.FormatComboBox.Size = new System.Drawing.Size(229, 20);
            this.FormatComboBox.TabIndex = 15;
            this.FormatComboBox.SelectedValueChanged += new System.EventHandler(this.FormatComboBox_SelectedValueChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 565);
            this.Controls.Add(this.FormatComboBox);
            this.Controls.Add(this.AverageLevelLabel);
            this.Controls.Add(this.AverageFreqLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.RefreshButton);
            this.Controls.Add(this.PeakLevelLabel);
            this.Controls.Add(this.PeakFreqLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LogTextBox);
            this.Controls.Add(this.RunningProgress);
            this.Controls.Add(this.DeviceComboBox);
            this.Controls.Add(this.PlotPanel);
            this.Controls.Add(this.StartButton);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.Panel PlotPanel;
        private System.Windows.Forms.ComboBox DeviceComboBox;
        private System.Windows.Forms.ProgressBar RunningProgress;
        private System.Windows.Forms.RichTextBox LogTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label PeakFreqLabel;
        private System.Windows.Forms.Label PeakLevelLabel;
        private System.Windows.Forms.Button RefreshButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label AverageFreqLabel;
        private System.Windows.Forms.Label AverageLevelLabel;
        private System.Windows.Forms.ComboBox FormatComboBox;
    }
}

