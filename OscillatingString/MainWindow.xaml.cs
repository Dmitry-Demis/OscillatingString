using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Threading;

namespace OscillatingString
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int
            N = 51,
            mtt = 3,
            h = 1,
            c = 2;

        private const double
            tau = 0.1,
            dH = 0.01,
            dG = 0.01;

        private readonly double[] Ut = new double[N];
        private  readonly double[] U = new double[N];
        private  readonly double[] Un = new double[N];
        private  readonly double[] Us = new double[N];
        private  readonly double[] f = new double[N];
        private  readonly double[] G = new double[4];
        private  readonly int Nt;
        private  readonly double r;
        private readonly double[] X;

        private long step,
            err;        
        private int CanvasHeight;
        private int CanvasWidth;
        private volatile bool pause = false;


        public MainWindow()
        {
            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            Task.Run(Go);
        }

        private void VarInit()
        {
            r = Math.Pow(c * tau / h, 2);
            for (var index = 0; index < G.Length; index++)
            {
                G[index] = 0;
            }

            for (var i = 0; i < N; i++)
            {
                Ut[i] = U[i] = Un[i] = Us[i] = 0;
            }

            Us[5] = U[5] - tau * 100;
            Nt = N / 2 + 1;
            step = 1;
            err = 0;
        }


        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            pause = true;
            StartButton.Visibility = Visibility.Visible;
            PauseButton.Visibility = Visibility.Collapsed;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            pause = false;
            StartButton.Visibility = Visibility.Collapsed;
            PauseButton.Visibility = Visibility.Visible;
        }

        private void Teach(double e)
        {
            var deltaG = new double[4];
            int sn;
            for (var i = -1; i < 2; i++)
            {
                sn = U[Nt + i] < 0 ? -1 : 1;
                deltaG[1 + i] = dG * sn * e;
            }
            sn = Us[Nt] < 0 ? -1 : 1;
            deltaG[3] = dG * sn * e;
            for (var i = 0; i < G.Length; i++) G[i] += deltaG[i];
            err++;
        }

        private void NewStep()
        {
            for (var i = 1; i < N - 1; i++) f[i] = U[i + 1] - 2.0 * U[i] + U[i - 1];
            for (var i = 1; i < N - 1; i++) Un[i] = 2 * U[i] - Us[i] + r * f[i];
            for (var i = 0; i < N; i++) Ut[i] = 0;
            for (var j = 1; j < N - 1; j++)
            {
                for (var i = -1; i <= 1; i++)
                {
                    Ut[j] += G[1 + i] * U[j + i];                   
                }
                Ut[j] += G[3] * Us[j];
            }
            var dU = Ut[Nt] - Un[Nt];
            if (dU < -dH) Teach(1);
            if (dU > dH) Teach(-1);
            step++;

        }
        private static BitmapImage BmpImageFromBmp(Bitmap bmp)
        {
            if (bmp == null) throw new ArgumentNullException(nameof(bmp));
            using (var memory = new System.IO.MemoryStream())
            {
                bmp.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }
        private void Render()
        {
            using (var bmp = new Bitmap((int)CanvasWidth, (int)CanvasHeight))
            using (var gfx = Graphics.FromImage(bmp))
            using (var pen = new Pen(Color.Aqua))
            {
                // draw one thousand random white lines on a dark blue background
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                gfx.Clear(Color.FromArgb(30, 30, 30));
                var pen2 = new Pen(Color.Red, 3);
                var pen3 = new Pen(Color.Yellow, 3);
                Dispatcher.Invoke(() =>
                {
                    gfx?.DrawLine(pen, 0, (int)(CanvasHeight / 2), (int)CanvasWidth, (int)(CanvasHeight / 2));
                    gfx?.DrawLine(pen, (int)(CanvasWidth / 2), 0, (int)(CanvasWidth / 2), (int)(CanvasHeight));
                    Random random = new Random();
                    for (int i = 0; i < N - 1; i++)
                    {
                        gfx?.DrawLine(pen2, (float)X[i], (float)(CanvasHeight /2 + mtt * U[i]), (float)X[i+1], (float)(CanvasHeight /2+ mtt * U[i+1]));
                        gfx?.DrawLine(pen3, (float)X[i], (float)(CanvasHeight / 2 + mtt * Ut[i]), (float)X[i + 1], (float)(CanvasHeight / 2 + mtt * Ut[i + 1]));
                    }
                });

                Dispatcher.Invoke(() =>
                {
                    MyImage.Source = BmpImageFromBmp(bmp);
                });
            }
        }
        ManualResetEvent _pauseEvent = new ManualResetEvent(true);
        private void Go()
        {
            VarInit();           
            while (true)
            {

                while (pause)
                {

                }
                
                NewStep();
                for (int i = 0; i < N; i++)
                {
                    Us[i]= U[i];
                    U[i]= Un[i];
                }
                Dispatcher.Invoke(() =>
                {
                    StepTextBox.Text = step.ToString();
                    G1TextBox.Text = G[0].ToString();
                    G2TextBox.Text = G[1].ToString();
                    G3TextBox.Text = G[2].ToString();
                    G4TextBox.Text = G[3].ToString();
                });
                step++;
                Render();
              //  Thread.Sleep(10);
             //   Render();
            }
        }
        private void MyCanvas_OnLoaded(object sender, RoutedEventArgs e)
        {
            PresentationSource source = PresentationSource.FromVisual(this);

            double dpiX = 1.0, dpiY = 1.0;
            if (source != null)
            {
                dpiX = 1 / 96.0 * 96.0 * source.CompositionTarget.TransformToDevice.M11;
                dpiY = 1 / 96.0 * 96.0 * source.CompositionTarget.TransformToDevice.M22;
            }
            CanvasHeight = (int)(dpiY * MyCanvas.ActualHeight);
            CanvasWidth = (int)(dpiX * MyCanvas.ActualWidth);
            int countOfSegments = CanvasWidth / N;
            X = new double[N];
            for (int i = 0; i < X.Length; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    X[i] += countOfSegments;
                }
            }
            CanvasWidth = N * countOfSegments;

        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            StepTextBox.Text = step.ToString();
        }
        
        private void G1TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            G1TextBox.Text = G[0].ToString();
        }

        private void G2TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            G2TextBox.Text = G[1].ToString();
        }

        private void G3TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            G3TextBox.Text = G[2].ToString();
        }

        private void G4TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            G4TextBox.Text = G[3].ToString();
        }

    }


}
