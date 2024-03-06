using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WinForms;
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
        public int SampleRate { get; private set; } = 48000;
        public int BitDepth { get; private set; } = 32;
        public int Channels { get; private set; } = 2;
        public int BufferMilliseconds { get; private set; } = 20;
        public bool IsFloat32 { get; private set; } = true;
        public double SNR => 6.02 * Format.BitsPerSample + 1.761;
        //public double NoiseLevel => 

        public delegate void PeakValueChangedEventHandler(double freq, double power);
        public event PeakValueChangedEventHandler PeakValueChanged;

        public delegate void FftValueChangedEventHandler(double[] values, double period);
        public event FftValueChangedEventHandler FftValueChanged;

        public WaveFormat Format 
        {
            get
            {
                return IsFloat32 ? WaveFormat.CreateIeeeFloatWaveFormat(SampleRate, Channels)
                    : new WaveFormat(SampleRate, BitDepth, Channels);
            }
        }

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
        public bool SelectDevice(string deviceName)
        {
            lock (_deviceLock)
            {
                foreach (var name in _deviceNames)
                {
                    if (name == deviceName)
                    {
                        _device = _devices[_deviceNames.IndexOf(name)];
                        return true;
                    }
                }
                Logging.Logger.Warn($"Device not found : {deviceName}");
                return false;
            }
        }

        public void Start(string deviceName, int sampleRate, int bitDepth, int channels, bool isFloat32, FormsPlot plot = null)
        {
            SampleRate = sampleRate;
            BitDepth = bitDepth;
            Channels = channels;
            IsFloat32 = isFloat32;
            this.Start(deviceName, plot);
        }

        public void Start(string deviceName, FormsPlot plot = null)
        {
            if (IsRunning)
            {
                Logging.Logger.Warn($"Is running!! - {_device.FriendlyName}");
                return;
            }
            if (!_deviceNames.Contains(deviceName))
            {
                Logging.Logger.Warn($"Device not found - {deviceName}");
                return;
            }

            IsRunning = true;

            Logging.Logger.Debug($"Setup device - {deviceName}");

            Logging.Logger.Debug($"SNR = {SNR}");

            _capture = _device.DataFlow == DataFlow.Capture
                ? new WasapiCapture(_device, true, BufferMilliseconds)
                : new WasapiLoopbackCapture(_device);
            _capture.DataAvailable += WaveIn_DataAvailable;

            Logging.Logger.Debug($"Old format - {_capture.WaveFormat}");
            var fmt = this.Format;
            Logging.Logger.Debug($"Load format - {fmt}");
            _capture.WaveFormat = fmt;
            Logging.Logger.Debug($"New format - {_capture.WaveFormat}");

            _audioValues = new double[fmt.SampleRate / 10];
            var windowed = new FftSharp.Windows.Hanning().Apply(_audioValues);
            var padded = FftSharp.Pad.ZeroPad(windowed);
            var spectrum = FftSharp.FFT.Forward(padded);
            var fftValues = FftSharp.FFT.Power(spectrum);
            _fftValues = new double[fftValues.Length];

            var fftPeriod = FftSharp.FFT.FrequencyResolution(fftValues.Length, fmt.SampleRate);
            
            if (plot != null)
            {
                Logging.Logger.Debug("Setup signal plot");

                _signalPlot = plot;
                _signalPlot.Plot.Clear();
                _signalPlot.Plot.Add.Signal(_fftValues, fftPeriod);
                _signalPlot.Plot.YLabel("Spectral Power");
                _signalPlot.Plot.XLabel("Frequency (Hz)");
                _signalPlot.Plot.Title($"{fmt.Encoding} ({fmt.BitsPerSample}-bit) {fmt.SampleRate} Hz");
                _signalPlot.Plot.Axes.SetLimits(
                    left: 0,
                    right: fmt.SampleRate / 2,
                    //right: 8000,
                    bottom: SNR,
                    top: 0);
                _signalPlot.Plot.Add.VerticalLine(_targetFreq - _targetMargin, color: Colors.Red);
                _signalPlot.Plot.Add.VerticalLine(_targetFreq + _targetMargin, color: Colors.Red);
                _signalPlot.Plot.Add.HorizontalLine(0, color: Colors.Green, pattern: LinePattern.Dashed);
                _signalPlot.Refresh();
            }

            _timer = new System.Timers.Timer
            {
                Interval = BufferMilliseconds,
                AutoReset = true
            };
            _timer.Elapsed += Timer_Elapsed;

            Logging.Logger.Debug("Start capturing");
            _capture.StartRecording();
            _timer.Start();
        }

        public void Stop()
        {
            if (!IsRunning)
            {
                Logging.Logger.Warn("Already start");
                return;
            }

            Logging.Logger.Debug("Stop capturing");
            IsRunning = false;
            _signalPlot = null;

            if (_capture != null)
            {
                _capture.DataAvailable -= WaveIn_DataAvailable;
                _capture.StopRecording();
                _capture.Dispose();
            }
            if (_timer != null)
            {
                _timer.Elapsed -= Timer_Elapsed;
                _timer.Stop();
                _timer.Dispose();
            }
        }

        #region Private
        private static readonly Lazy<AudioManager> _instance = new Lazy<AudioManager>(() => new AudioManager());
        private AudioManager() { }

        private readonly List<MMDevice> _devices = new List<MMDevice>();
        private readonly List<string> _deviceNames = new List<string>();
        private MMDevice _device;
        private WasapiCapture _capture;
        private System.Timers.Timer _timer;
        private double[] _audioValues;
        private double[] _fftValues;

        private int _targetMargin = 100;
        private int _targetFreq = 1000;

        private int _dropFreq = 200;

        private FormsPlot _signalPlot;

        private double _averagePower;

        private string[] RefreshDevices()
        {
            var devices = new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
            _devices.Clear();
            _devices.AddRange(devices);
            _deviceNames.Clear();
            _deviceNames.AddRange((from device in _devices select device.FriendlyName).ToArray());
            return _deviceNames.ToArray();
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            var fmt = _capture.WaveFormat;
            var bytesPerSample = fmt.BitsPerSample / 8 * fmt.Channels;
            var bufferCount = e.Buffer.Length / bytesPerSample;

            if (bufferCount > _audioValues.Length)
                bufferCount = _audioValues.Length;
            
            if (fmt.BitsPerSample == 16 && fmt.Encoding == WaveFormatEncoding.Pcm)
            {
                for (int i = 0; i < bufferCount; i++)
                {
                    _audioValues[i] = BitConverter.ToInt16(e.Buffer, i * bytesPerSample) / Math.Pow(2, fmt.BitsPerSample - 1);
                    //_audioValues[i] = BitConverter.ToInt16(e.Buffer, i * bytesPerSample);
                }
            }
            else if (fmt.BitsPerSample == 32 && fmt.Encoding == WaveFormatEncoding.Pcm)
            {
                for (int i = 0; i < bufferCount; i++)
                {
                    _audioValues[i] = BitConverter.ToInt32(e.Buffer, i * bytesPerSample) / Math.Pow(2, fmt.BitsPerSample - 1);
                    //_audioValues[i] = BitConverter.ToInt32(e.Buffer, i * bytesPerSample);
                }
            }
            else if (fmt.BitsPerSample == 32 && fmt.Encoding == WaveFormatEncoding.IeeeFloat)
            {
                for (int i = 0; i < bufferCount; i++)
                {
                    _audioValues[i] = BitConverter.ToSingle(e.Buffer, i * bytesPerSample);
                }
            }
            else
            {
                throw new NotSupportedException(fmt.Encoding.ToString());
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var fmt = _capture.WaveFormat;

            var windowed = new FftSharp.Windows.Hanning().Apply(_audioValues);
            var padded = FftSharp.Pad.ZeroPad(windowed);
            var spectrum = FftSharp.FFT.Forward(padded);
            var fftValues = FftSharp.FFT.Power(spectrum);

            var fftPeriod = FftSharp.FFT.FrequencyResolution(fftValues.Length, fmt.SampleRate);
            var fftFreq = FftSharp.FFT.FrequencyScale(fftValues.Length, fmt.SampleRate);

            // filter
            //var dropRange = (int)Math.Floor(_dropFreq / fftPeriod);
            //for (int i = 0; i < dropRange; i++)
            //{
            //    fftValues[i] = SNR;
            //}
            //if (fmt.Encoding == WaveFormatEncoding.IeeeFloat && _audioValues.Max() > 1)

            //if (windowed.Max() > 1)
            //{
            //    Logging.Logger.Debug($"Over : {windowed.Max()} / {windowed.Min()}");
            //}

            Array.Copy(fftValues, _fftValues, fftValues.Length);

            var peakPower = fftValues.Max();
            var peakIndex = fftValues.ToList().IndexOf(peakPower);
            var peakFreq = peakIndex * fftPeriod;

            _averagePower = fftValues.Average();

            PeakValueChanged?.Invoke(peakFreq, peakPower);
            FftValueChanged?.Invoke(_fftValues, fftPeriod);

            _signalPlot?.BeginInvoke(new Action(() =>
            {
                if (_signalPlot != null)
                {
                    var limits = _signalPlot.Plot.Axes.GetLimits();
                    _signalPlot.Plot.Axes.SetLimits(
                        bottom: Math.Min(limits.Bottom, _fftValues.Min() == double.NegativeInfinity ? SNR : _fftValues.Min()),
                        top: Math.Max(limits.Top, _fftValues.Max()));
                    foreach (var line in _signalPlot.Plot.GetPlottables<HorizontalLine>())
                    {
                        line.Y = _averagePower;
                    }
                }
                _signalPlot?.Refresh();
            }));
        }
        #endregion
    }
}
