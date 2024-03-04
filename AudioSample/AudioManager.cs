using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AudioSample
{
    internal sealed class AudioManager
    {
        public static AudioManager Instance => _instance.Value;

        public bool IsRunning { get; private set; } = false;
        public int SampleRate => 44100;
        public int BitDepth => 16;
        public int Channels => 1;
        public int BufferMilliseconds => 10;
        public double NoiseRatio => 20 * Math.Log10(Math.Pow(2, BitDepth - 1) * Math.Pow(3 / 2.0, 1 / 2.0));
        public WaveFormat Format => WaveFormat.CreateIeeeFloatWaveFormat(SampleRate, 2);
        //public WaveFormat Format => new WaveFormat(SampleRate, BitDepth, Channels);

        public delegate void FftValueChangedEventHandler(double[] values, double period, int length);
        public event FftValueChangedEventHandler FftValueChanged;

        public string[] GetDevices()
        {
            string[] ret;
            lock (_deviceLock)
            {
                ret = RefreshDevices();
            }
            return ret;
        }

        private readonly object _deviceLock = new object();
        public void SelectDevice(string deviceName)
        {
            lock (_deviceLock)
            {
                foreach (var name in _deviceNames)
                {
                    if (name == deviceName)
                    {
                        _selectedDevice = _devices[_deviceNames.IndexOf(name)];
                        return;
                    }
                }
                Logging.Logger.Warn($"Device not found : {deviceName}");
            }
        }

        public void StartRecording(string deviceName, string filePath, int timeSeconds = 5)
        {
            if (IsRunning) return;
            if (!_deviceNames.Contains(deviceName))
            {
                Logging.Logger.Warn($"Device not found : {deviceName}");
                return;
            }

            IsRunning = true;
            _cts = new CancellationTokenSource();

            var finfo = new FileInfo(filePath);
            if (!finfo.Directory.Exists)
            {
                Logging.Logger.Warn($"Directory not found, will be create ({finfo.DirectoryName})");
                finfo.Directory.Create();
                finfo.Refresh();
            }
            if (finfo.Exists)
            {
                Logging.Logger.Warn($"Old file exist, will be delete ({finfo.FullName})");
                finfo.Delete();
                finfo.Refresh();
            }

            Task.Run(async () =>
            {
                try
                {
                    _rawDevice = new WasapiCapture(_selectedDevice, true, BufferMilliseconds);
                    _rawDevice.DataAvailable += WaveIn_DataAvailable;
                    var fmt = new WaveFormat(SampleRate, BitDepth, Channels);
                    _rawDevice.WaveFormat = fmt;
                    _waveWriter = new WaveFileWriter(finfo.FullName, fmt);
                    _rawDevice.StartRecording();
                    await Task.Delay(TimeSpan.FromSeconds(timeSeconds), _cts.Token);
                }
                catch (Exception ex)
                {
                    if (ex is OperationCanceledException || ex is TaskCanceledException)
                    {
                        Logging.Logger.Debug("Recording canceled");
                    }
                    else
                    {
                        Logging.Logger.Error("Unexpected error");
                        Logging.Logger.Error(ex.ToString());
                    }
                }
                finally
                {
                    _cts?.Dispose();
                    _cts = null;
                    StopRecording();
                }
            }, _cts.Token);
        }

        public void StopRecording()
        {
            if (!IsRunning) return;

            IsRunning = false;
            _cts?.Cancel();
            if (_rawDevice != null)
            {
                _rawDevice.DataAvailable -= WaveIn_DataAvailable;
                _rawDevice.Dispose();
                _rawDevice = null;
            }
            if (_waveWriter != null)
            {
                _waveWriter.Dispose();
                _waveWriter = null;
            }
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
        }

        public void StartMonitoring(string deviceName, ScottPlot.WinForms.FormsPlot plot)
        {
            if (IsRunning) return;
            if (!_deviceNames.Contains(deviceName))
            {
                Logging.Logger.Warn($"Device not found : {deviceName}");
                return;
            }

            IsRunning = true;

            Logging.Logger.Debug($"Noise Ratio (SNR) : -{NoiseRatio}");

            _rawDevice = new WasapiCapture(_selectedDevice, true, BufferMilliseconds);
            _rawDevice.DataAvailable += WaveIn_DataAvailable;
            //var fmt = new WaveFormat(SampleRate, BitDepth, Channels);
            //_rawDevice.WaveFormat = fmt;

            //var fmt = _rawDevice.WaveFormat;

            var fmt = this.Format;
            _rawDevice.WaveFormat = fmt;

            _audioValues = new double[fmt.SampleRate / 10];
            var window = new FftSharp.Windows.Hanning();
            var windowed = window.Apply(_audioValues);
            var paddedAudio = FftSharp.Pad.ZeroPad(windowed);
            var spectrum = FftSharp.FFT.Forward(paddedAudio);
            var fftPower = FftSharp.FFT.Power(spectrum);
            _fftValues = new double[fftPower.Length];
            var fftPeriod = FftSharp.FFT.FrequencyResolution(fftPower.Length, fmt.SampleRate);
            _formsPlot = plot;
            _formsPlot.Plot.Clear();
            //_formsPlot.Plot.Add.Signal(_fftValues, 1.0 / fftPeriod);
            //_formsPlot.Plot.Add.Signal(_fftValues, 2.0 * fftPower.Length / fmt.SampleRate);
            _formsPlot.Plot.Add.Signal(_fftValues, fftPeriod);
            _formsPlot.Plot.YLabel("Spectral Power");
            _formsPlot.Plot.XLabel("Frequency (kHz)");
            _formsPlot.Plot.Title($"{fmt.Encoding} ({fmt.BitsPerSample}-bit) {fmt.SampleRate} kHz");
            _formsPlot.Plot.Axes.SetLimits(
                -100, 
                fmt.SampleRate, 
                -200, 
                0);
            _formsPlot.Refresh();

            _timer = new System.Timers.Timer
            {
                Interval = BufferMilliseconds,
                AutoReset = true,
            };
            _timer.Elapsed += Timer_Elapsed;

            _rawDevice.StartRecording();
            _timer.Start();
        }

        public void StopMonitoring()
        {
            if (!IsRunning) return;

            IsRunning = false;
            _rawDevice.StopRecording();
            _rawDevice.DataAvailable -= WaveIn_DataAvailable;
            _rawDevice.Dispose();
            _rawDevice = null;

            _timer.Stop();
            _timer.Elapsed -= Timer_Elapsed;
            _timer.Dispose();
            _timer = null;

            _formsPlot = null;
        }

        #region Private
        private static readonly Lazy<AudioManager> _instance = new Lazy<AudioManager>(() => new AudioManager());
        private AudioManager() { }

        private List<MMDevice> _devices = new List<MMDevice>();
        private List<string> _deviceNames = new List<string>();
        private MMDevice _selectedDevice;
        private WasapiCapture _rawDevice;
        private System.Timers.Timer _timer;
        private CancellationTokenSource _cts;
        private WaveFileWriter _waveWriter;
        private double[] _audioValues;
        private double[] _fftValues;

        private ScottPlot.WinForms.FormsPlot _formsPlot;

        private string[] RefreshDevices()
        {
            var devices = new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
            _devices.Clear();
            _devices.AddRange(devices);
            if (_selectedDevice != null || !_devices.Contains(_selectedDevice))
            {
                _selectedDevice = _devices.First();
            }
            _deviceNames.Clear();
            _deviceNames.AddRange((from device in _devices select device.FriendlyName).ToArray());
            return _deviceNames.ToArray();
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (_waveWriter != null)
            {
                // Recording
                _waveWriter.Write(e.Buffer, 0, e.BytesRecorded);
                _waveWriter.Flush();
            }
            else
            {
                // Monitoring
                var fmt = _rawDevice.WaveFormat;
                var bpsPerChannel = fmt.BitsPerSample / 8;
                var bytesPerSample = bpsPerChannel * fmt.Channels;
                var bufferCount = e.Buffer.Length / bytesPerSample;
                if (bufferCount >= _audioValues.Length)
                    bufferCount = _audioValues.Length;

                if (bpsPerChannel == 2 && fmt.Encoding == WaveFormatEncoding.Pcm)
                {
                    for (int i = 0; i < bufferCount; i++)
                        _audioValues[i] = BitConverter.ToInt16(e.Buffer, i * bytesPerSample);
                }
                else if (bpsPerChannel == 4 && fmt.Encoding == WaveFormatEncoding.Pcm)
                {
                    for (int i = 0; i < bufferCount; i++)
                        _audioValues[i] = BitConverter.ToInt32(e.Buffer, i * bytesPerSample);
                }
                else if (bpsPerChannel == 4 && fmt.Encoding == WaveFormatEncoding.IeeeFloat)
                {
                    for (int i = 0; i < bufferCount; i++)
                        _audioValues[i] = BitConverter.ToSingle(e.Buffer, i * bytesPerSample);
                }
                else
                {
                    throw new NotSupportedException(fmt.Encoding.ToString());
                }
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var fmt = _rawDevice.WaveFormat;
            var window = new FftSharp.Windows.Hanning();
            var windowed = window.Apply(_audioValues);
            var paddedAudio = FftSharp.Pad.ZeroPad(windowed);
            var spectrum = FftSharp.FFT.Forward(paddedAudio);
            var fftPower = FftSharp.FFT.Power(spectrum);
            Array.Copy(fftPower, _fftValues, fftPower.Length);
            var fftFreq = FftSharp.FFT.FrequencyScale(fftPower.Length, fmt.SampleRate);
            int peakIndex = 0;
            for (int i = 0; i < fftPower.Length; i++)
            {
                if (fftPower[i] > fftPower[peakIndex])
                    peakIndex = i;
            }
            //var fftPeriod = FftSharp.FFT.FrequencyResolution(fftPower.Length, fmt.SampleRate);
            //var peakFreq = fftPeriod * peakIndex;
            var peakFreq = fftFreq[peakIndex];
            var peakPower = fftPower[peakIndex];
            Logging.Logger.Debug($"Peak Frequency : {peakFreq:N0} Hz");
            Logging.Logger.Debug($"Peak Power : {peakPower:N0}");

            if (_formsPlot != null)
            {
                //var plotLimits = _formsPlot.Plot.Axes.GetLimits();
                //_formsPlot.Plot.Axes.SetLimits(
                //    -100, 
                //    Math.Max(fmt.SampleRate / 2, plotLimits.Right),
                //    Math.Min(fftPower.Min(), plotLimits.Bottom),
                //    Math.Max(fftPower.Max(), plotLimits.Top));
                _formsPlot?.BeginInvoke(new Action(() => _formsPlot?.Refresh()));
            }
        }
        #endregion
    }
}
