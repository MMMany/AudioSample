using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using Accord.Audio;
using Accord.Audio.Filters;
using Accord.Math;
using NAudio.Dsp;
using Accord.Math.Transforms;

namespace AudioSample
{
    internal sealed class AudioManager2
    {
        public static AudioManager2 Instance => _instance.Value;

        public void Test()
        {
            // sample using Accord.NET
            return;

#if false
            const int sampleRate = 48000;
            const int fftLength = 1024; // FFT 크기
            const int bufferLength = fftLength * 2; // 버퍼 크기 (2배 FFT 크기)
            
            // 오디오 입력 장치 설정
            WaveInEvent waveIn = new WaveInEvent();
            waveIn.DeviceNumber = 0; // 오디오 장치 번호
            waveIn.WaveFormat = new WaveFormat(sampleRate, 16, 1); // 16비트, 단일 채널

            // FFT 준비
            var fft = new FastFourierTransform(fftLength);
            
            var spectrum = new double[fftLength / 2];

            // 버퍼 설정
            byte[] buffer = new byte[bufferLength];

            // 오디오 입력 이벤트 핸들러
            waveIn.DataAvailable += (sender, e) =>
            {
                // 16비트를 복소수 형태의 배열로 변환
                var signal = Signal.FromArray(BitConverter.ToInt16(e.Buffer, 0), bufferLength);

                // FFT 수행
                System.Numerics.Complex[] fftResult = fft.Compute(signal);

                // 스펙트럼 계산
                for (int i = 0; i < spectrum.Length; i++)
                {
                    spectrum[i] = 20 * Math.Log10(fftResult[i].Magnitude);
                }

                // dB 값 출력
                foreach (var value in spectrum)
                {
                    Console.WriteLine($"dB: {value}");
                }
            };

            // 오디오 입력 시작
            waveIn.StartRecording();

            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();

            // 오디오 입력 중지
            waveIn.StopRecording();
#endif
        }

        #region Private
        private readonly static Lazy<AudioManager2> _instance = new Lazy<AudioManager2>(() => new AudioManager2());
        private AudioManager2() { }
        #endregion
    }
}
