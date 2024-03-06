using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioSample
{
    internal sealed class CaptureFormat
    {
        public int SampleRate { get; set; }
        public int BitDepth { get; set; }
        public int Channels { get; set; }
        public bool IsFloat32 { get; set; }

        public CaptureFormat(int sampleRate, int bitDepth, int channels, bool isFloat)
        {
            SampleRate = sampleRate;
            BitDepth = bitDepth;
            Channels = channels;
            IsFloat32 = isFloat;
        }

        public override string ToString()
        {
            return string.Join(", ", new[]
            {
                IsFloat32 ? "IeeeFloat" : "PCM",
                $"{BitDepth}bit",
                $"{Channels}ch",
                $"{SampleRate}Hz",
            });
        }
    }
}
