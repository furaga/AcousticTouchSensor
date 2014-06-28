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
    public partial class Form1 : Form
    {
        AcousticSensor sensor;


        PointF[] leftGraph = new PointF[8000];
        PointF[] rightGraph = new PointF[8000];
        double[] leftSpec = new double[sample / 2];
        double[] rightSpec = new double[sample / 2];

        List<double[]> leftSpecs = new List<double[]>();


        public Form1()
        {
            InitializeComponent();
            sensor = new AcousticSensor(8000, 16, 2);
        }

        
        WaveIn waveIn;
        WaveFileWriter writer;
        List<short> left = new List<short>();
        List<short> right = new List<short>();
        int state = 0;
        int cur = 0;
        int to = 0;
        const int sample = 8000 * 30 / 1000;
        ILArray<double> fftIn = new double[sample];
        int shift = 800 * 20 / 1000;
        int offset = 0;
        void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new EventHandler<WaveInEventArgs>(OnDataAvailable), sender, e);
            }
            else
            {
                int prev = to;
                for (int i = 0; i < e.BytesRecorded; i++)
                {
                    switch (state)
                    {
                        case 0:
                            left.Add(e.Buffer[i]);
                            break;
                        case 1:
                            left[left.Count - 1] = (short)(left[left.Count - 1] | (e.Buffer[i] << 8));
                            break;
                        case 2:
                            right.Add(e.Buffer[i]);
                            break;
                        case 3:
                            right[right.Count - 1] = (short)(right[right.Count - 1] | (e.Buffer[i] << 8));
                            to++;
                            break;
                    }
                    while (left.Count >= leftGraph.Length)
                        left.RemoveAt(0);
                    while (right.Count >= leftGraph.Length)
                        right.RemoveAt(0);

                    state = ((state + 1) & 0x3);
                }

                if (prev != to)
                {
                    if (to % shift == 0 && left.Count >= leftGraph.Length - 1)
                    {
                        for (int i = 0; i < fftIn.Length; i++)
                            fftIn[i] = left[i - sample + left.Count - 1];
                        ILArray<complex> fft = ILMath.fft(fftIn);
                        int idx = 0;
                        foreach (var z in fft)
                        {
                            if (leftSpec.Length <= idx)
                                break;
                            leftSpec[idx] = z.Abs();
                            idx++;
                        }
                        double max = Math.Max(0.0001, leftSpec.Max());
                        for (int i = 0; i < leftSpec.Length; i++)
                            leftSpec[i] /= max;

                        leftSpecs.Add(leftSpec);
                        while (leftSpecs.Count >= 34)
                            leftSpecs.RemoveAt(0);
                        leftSpec = new double[leftSpec.Length];
                    }
                }

                writer.Write(e.Buffer, 0, e.BytesRecorded);
                //                int secondsRecorded = (int)(writer.Length / writer.WaveFormat.AverageBytesPerSecond);
                pictureBox1.Invalidate();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (waveIn == null)
            {
                waveIn = new WaveIn();
                waveIn.WaveFormat = new WaveFormat(8000, 16, 2);

                writer = new WaveFileWriter("test.wav", waveIn.WaveFormat);

                waveIn.DataAvailable += OnDataAvailable;
                waveIn.RecordingStopped += OnRecordingStopped;
                waveIn.StartRecording();
            }
        }
        void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new EventHandler<StoppedEventArgs>(OnRecordingStopped), sender, e);
            }
            else
            {
                Cleanup();
                if (e.Exception != null)
                {
                    MessageBox.Show(String.Format("A problem was encountered during recording {0}",
                                                  e.Exception.Message));
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (waveIn != null)
                waveIn.StopRecording();
        }

        private void Cleanup()
        {
            if (waveIn != null) // working around problem with double raising of RecordingStopped
            {
                waveIn.Dispose();
                waveIn = null;
            }
            if (writer != null)
            {
                writer.Close();
                writer = null;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cleanup();
        }
        unsafe private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < rightGraph.Length; i++)
            {
                rightGraph[i].X = (float)i * pictureBox1.Width / rightGraph.Length;
                if (right.Count > i)
                    rightGraph[i].Y = (float)right[i] / short.MaxValue * pictureBox1.Height * 0.25f;
                else
                    rightGraph[i].Y = 0;
                rightGraph[i].Y += (float)pictureBox1.Height * 0.75f;
            }

            e.Graphics.Clear(Color.White);

            float width = (float)pictureBox1.Width / leftSpecs.Count;
            for (int i = 0; i < leftSpecs.Count; i++)
            {
                float height = (float)pictureBox1.Height / leftSpecs[0].Length * 0.5f;
                float x = i * width;
                for (int j = 0; j < leftSpecs[i].Length; j++)
                {
                    float y = j * height;
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb((int)(255 * leftSpecs[i][j]), 0, 0)), x, y, width, height);
                }
            }

            e.Graphics.DrawLines(new Pen(Brushes.Red), leftGraph);
            e.Graphics.DrawLines(new Pen(Brushes.Blue), rightGraph);
        }

    }
}