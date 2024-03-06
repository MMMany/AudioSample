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
    public partial class MainForm : Form
    {
        private readonly ScottPlot.WinForms.FormsPlot SignalPlot;

        public MainForm()
        {
            InitializeComponent();

            SignalPlot = new ScottPlot.WinForms.FormsPlot
            {
                Dock = DockStyle.Fill,
            };
            PlotPanel.Controls.Add(SignalPlot);

            foreach (var isFloat in new[] { true, false })
            {
                foreach (var bit in new[] { 16, 32 })
                {
                    foreach (var ch in new[] { 1, 2 })
                    {
                        foreach (var rate in new[] { 32000, 44100, 48000 })
                        {
                            if (isFloat && bit == 16) continue;
                            FormatComboBox.Items.Add(new CaptureFormat(rate, bit, ch, isFloat));
                        }
                    }
                }
            }
            FormatComboBox.SelectedIndex = 0;
        }

        private void Log(string message)
        {
            Logging.Logger.Debug(message);
            var time = DateTime.Now.ToString(@"yyyy.MM.dd HH\:mm\:ss.fff");
            LogTextBox.AddText($"[{time}] {message}");
        }

        private void PeakValueChanged(double freq, double power)
        {
            BeginInvoke(new Action(() =>
            {
                PeakFreqLabel.Text = $"{freq:N0} Hz";
                PeakLevelLabel.Text = $"{power:N0}";
            }));
        }

        private void FftValueChanged(double[] values, double period)
        {
            BeginInvoke(new Action(() =>
            {
                AverageLevelLabel.Text = $"{values.Average():N0}";
            }));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var devices = AudioManager.Instance.GetDevices();
            DeviceComboBox.Items.Clear();
            DeviceComboBox.Items.AddRange(devices);
            DeviceComboBox.SelectedIndex = 0;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            AudioManager.Instance.PeakValueChanged -= PeakValueChanged;
            AudioManager.Instance.FftValueChanged -= FftValueChanged;
            AudioManager.Instance.Stop();
            base.OnFormClosing(e);
        }

        private bool IsRunning = false;
        private readonly object _controlLock = new object();

        private void UpdateContols()
        {
            StartButton.Text = IsRunning ? "Stop" : "Start";
            RefreshButton.Enabled = !IsRunning;
            DeviceComboBox.Enabled = !IsRunning;
            FormatComboBox.Enabled = !IsRunning;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (DeviceComboBox.Items.Count == 0) return;
            if (Monitor.IsEntered(_controlLock)) return;
            Monitor.Enter(_controlLock);
            if (!IsRunning)
            {
                // start
                IsRunning = true;
                Log("Start capturing");
                var fmt = FormatComboBox.SelectedItem as CaptureFormat;
                AudioManager.Instance.PeakValueChanged += PeakValueChanged;
                AudioManager.Instance.FftValueChanged += FftValueChanged;
                AudioManager.Instance.Start(
                    deviceName: DeviceComboBox.SelectedItem as string,
                    sampleRate: fmt.SampleRate,
                    bitDepth: fmt.BitDepth,
                    channels: fmt.Channels,
                    isFloat32: fmt.IsFloat32,
                    plot: SignalPlot);
                Log($"SNR = {AudioManager.Instance.SNR:N2}");
            }
            else
            {
                // stop
                IsRunning = false;
                Log("Stop capturing");
                AudioManager.Instance.PeakValueChanged -= PeakValueChanged;
                AudioManager.Instance.FftValueChanged -= FftValueChanged;
                AudioManager.Instance.Stop();
            }
            UpdateContols();
            Monitor.Exit(_controlLock);
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            if (Monitor.IsEntered(_controlLock)) return;
            Monitor.Enter(_controlLock);
            Log("Refresh devices");
            var devices = AudioManager.Instance.GetDevices();
            DeviceComboBox.Items.Clear();
            DeviceComboBox.Items.AddRange(devices);
            DeviceComboBox.SelectedIndex = 0;
            Monitor.Exit(_controlLock);
        }

        private void DeviceComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (DeviceComboBox.Items.Count > 0)
            {
                var deviceName = DeviceComboBox.SelectedItem as string;
                if (AudioManager.Instance.SelectDevice(deviceName))
                {
                    Log($"Device changed - {deviceName}");
                }
            }
        }

        private void FormatComboBox_SelectedValueChanged(object sender, EventArgs e)
        {

        }
    }
}
