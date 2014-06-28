using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using FLib;
using NAudio;
using NAudio.Wave;
using ILNumerics;

namespace AcousticTouchSensor
{
    public class AcousticSensor
    {
        AcousticSensorData data;
        int rate, bits;

        WaveIn waveIn = null;
        int ch = 0;
        bool lowBit = true;

        public AcousticSensor(int rate, int bits, int channels)
        {
            this.rate = rate;
            this.bits = bits;
            this.data = new AcousticSensorData(channels);
        }

        public void Start()
        {
            if (waveIn != null)
                waveIn.StopRecording();
            Cleanup();

            waveIn = new WaveIn();
            waveIn.WaveFormat = new WaveFormat(rate, bits, data.Channels);
            waveIn.DataAvailable += OnDataAvailable;
            waveIn.RecordingStopped += OnRecordingStopped;
            waveIn.StartRecording();
        }

        public void Stop()
        {
            if (waveIn != null)
                waveIn.StopRecording();
            Cleanup();
        }

        private void Cleanup()
        {
            if (waveIn != null)
            {
                waveIn.Dispose();
                waveIn = null;
            }
        }

        //--------------------------------------------------------

        void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            AddBytes(e.Buffer, e.BytesRecorded);
        }

        void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            if (e.Exception != null)
                MessageBox.Show(e.Exception.ToString() + e.Exception.StackTrace);
        }

        void AddBytes(byte[] bytes, int count)
        {
            if (bytes == null || bytes.Length <= count)
                return;
            
            if (data.Buffer.Count <= ch)
                return;

            for (int i = 0; i < count; i++)
            {
                byte n = bytes[i];
                if (lowBit)
                {
                    data.Buffer[ch].Add(n);
                    lowBit = false;
                }
                else
                {
                    short high = (short)(n << 8);
                    if (data.Buffer[ch].Count <= 1)
                        continue;
                    short low = (short)data.Buffer[ch][data.Buffer.Count - 1];
                    data.Buffer[ch].Add((short)(high | low));
                    ch = (ch + 1) % data.Buffer.Count;
                    lowBit = true;
                }
            }
        }
    }

    public class AcousticSensorData
    {
        List<List<short>> buffer = new List<List<short>>();
        public List<List<short>> Buffer { get { return buffer; } }
        public int Channels { get { return buffer == null ? 0 : buffer.Count; } }

        public AcousticSensorData(int channels)
        {
            for (int i = 0; i < channels; i++)
                buffer.Add(new List<short>());
        }

    }
}
