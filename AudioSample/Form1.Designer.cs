namespace AudioSample
{
    partial class Form1
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
            this.RecordButton = new System.Windows.Forms.Button();
            this.PlotPanel = new System.Windows.Forms.Panel();
            this.DeviceComboBox = new System.Windows.Forms.ComboBox();
            this.MonitorButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // RecordButton
            // 
            this.RecordButton.Location = new System.Drawing.Point(412, 10);
            this.RecordButton.Name = "RecordButton";
            this.RecordButton.Size = new System.Drawing.Size(75, 23);
            this.RecordButton.TabIndex = 0;
            this.RecordButton.Text = "Record";
            this.RecordButton.UseVisualStyleBackColor = true;
            this.RecordButton.Click += new System.EventHandler(this.RecordButton_Click);
            // 
            // PlotPanel
            // 
            this.PlotPanel.BackColor = System.Drawing.SystemColors.ControlDark;
            this.PlotPanel.Location = new System.Drawing.Point(12, 41);
            this.PlotPanel.Name = "PlotPanel";
            this.PlotPanel.Size = new System.Drawing.Size(776, 397);
            this.PlotPanel.TabIndex = 1;
            // 
            // DeviceComboBox
            // 
            this.DeviceComboBox.FormattingEnabled = true;
            this.DeviceComboBox.Location = new System.Drawing.Point(12, 12);
            this.DeviceComboBox.Name = "DeviceComboBox";
            this.DeviceComboBox.Size = new System.Drawing.Size(394, 20);
            this.DeviceComboBox.TabIndex = 2;
            this.DeviceComboBox.SelectedValueChanged += new System.EventHandler(this.DeviceComboBox_SelectedValueChanged);
            // 
            // MonitorButton
            // 
            this.MonitorButton.Location = new System.Drawing.Point(493, 9);
            this.MonitorButton.Name = "MonitorButton";
            this.MonitorButton.Size = new System.Drawing.Size(75, 23);
            this.MonitorButton.TabIndex = 3;
            this.MonitorButton.Text = "Monitor";
            this.MonitorButton.UseVisualStyleBackColor = true;
            this.MonitorButton.Click += new System.EventHandler(this.MonitorButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.MonitorButton);
            this.Controls.Add(this.DeviceComboBox);
            this.Controls.Add(this.PlotPanel);
            this.Controls.Add(this.RecordButton);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button RecordButton;
        private System.Windows.Forms.Panel PlotPanel;
        private System.Windows.Forms.ComboBox DeviceComboBox;
        private System.Windows.Forms.Button MonitorButton;
    }
}

