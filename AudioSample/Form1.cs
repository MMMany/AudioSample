using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AudioSample
{
    public partial class Form1 : Form
    {
        readonly ScottPlot.WinForms.FormsPlot formsPlot1;

        public Form1()
        {
            InitializeComponent();

            formsPlot1 = new ScottPlot.WinForms.FormsPlot
            {
                Dock = DockStyle.Fill,
            };
            PlotPanel.Controls.Add(formsPlot1);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var devices = AudioManager.Instance.GetDevices();
            DeviceComboBox.Items.Clear();
            DeviceComboBox.Items.AddRange(devices);
            DeviceComboBox.SelectedIndex = 0;
        }

        private bool IsRunning => (IsRecording || IsMonitoring);
        private bool IsRecording = false;
        private bool IsMonitoring = false;
        private readonly object _controlLock = new object();

        private void UpdateContols()
        {
            if (!IsRunning)
            {
                RecordButton.Text = "Record";
                MonitorButton.Text = "Monitor";
                RecordButton.Enabled = MonitorButton.Enabled = DeviceComboBox.Enabled = true;
            }
            else
            {
                RecordButton.Text = IsRecording ? "Stop" : RecordButton.Text;
                RecordButton.Enabled = IsRecording;
                MonitorButton.Text = IsMonitoring ? "Stop" : MonitorButton.Text;
                MonitorButton.Enabled = IsMonitoring;
                DeviceComboBox.Enabled = false;
            }
        }

        private void RecordButton_Click(object sender, EventArgs e)
        {
            if (DeviceComboBox.Items.Count == 0) return;
            if (Monitor.IsEntered(_controlLock)) return;
            Monitor.Enter(_controlLock);
            if (!IsRunning)
            {
                // start
                IsRecording = true;
            }
            else
            {
                // stop
                IsRecording = false;
            }
            UpdateContols();
            Monitor.Exit(_controlLock);
        }

        private void MonitorButton_Click(object sender, EventArgs e)
        {
            if (DeviceComboBox.Items.Count == 0) return;
            if (Monitor.IsEntered(_controlLock)) return;
            Monitor.Enter(_controlLock);
            if (!IsRunning)
            {
                // start
                IsMonitoring = true;
                AudioManager.Instance.StartMonitoring(DeviceComboBox.SelectedItem as string, formsPlot1);
            }
            else
            {
                // stop
                IsMonitoring = false;
                AudioManager.Instance.StopMonitoring();
            }
            UpdateContols();
            Monitor.Exit(_controlLock);
        }

        private void DeviceComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (DeviceComboBox.Items.Count > 0)
            {
                var deviceName = DeviceComboBox.SelectedItem as string;
                Logging.Logger.Debug($"Device changed : {deviceName}");
                AudioManager.Instance.SelectDevice(deviceName);
            }
        }
    }
}
