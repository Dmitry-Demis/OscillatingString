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

        private double[] Ut = new double[N];
        private double[] U = new double[N];
        private double[] Un = new double[N];
        private double[] Us = new double[N];
        private double[] f = new double[N];
        private double[] G = new double[4];
        private int Nt;
        private double r;

        private long step,
            err;

        private int Wxl,
            Wyl,
            Wxr,
            Wyr,
            Wyc,
            bx,
            dx,
            rx;

        public long Step { get; set; }


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

        private void Teach(double e)
        {
            var deltaG = new double[4];
            int sn;
            for (var i = -1; i < 1; i++)
            {
                sn = U[Nt + i] < 0 ? -1 : 1;
                deltaG[2 + i] = dG * sn * e;
            }
            sn = U[Nt] < 0 ? -1 : 1;
            deltaG[4] = dG * sn * e;
            for (var i = 0; i < 4; i++) G[i] += deltaG[i];
            err++;
        }

        private void NewStep()
        {
            for (var i = 1; i < N-1; i++) f[i] = U[i + 1] - 2 * U[i] + U[i - 1];
            for (var i = 1; i < N - 1; i++) Un[i] = 2 * U[i] - Us[i] + r * f[i];
            for (var i = 0; i < N; i++) Ut[i] = 0;
            for (var j = 1; j < N-1; j++)
            {
                for (var i = -1; i < 1; i++)
                {
                    Ut[j] += G[2 + i] * U[j + i];
                    Ut[j] += G[4] * Us[j];
                }
            }
            var dU = Ut[Nt] - Un[Nt];
            if (dU < -dH)  Teach(1);
            if (dU > dH) Teach(-1);
            step++;
            Step = step;
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
            using (var bmp = new Bitmap((int)MyCanvas.ActualWidth, (int)MyCanvas.ActualHeight))
            using (var gfx = Graphics.FromImage(bmp))
            using (var pen = new Pen(Color.Aqua))
            {
                // draw one thousand random white lines on a dark blue background
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                gfx.Clear(Color.FromArgb(30, 30, 30));
                var pen2 = new Pen(Color.Bisque);
                this.Dispatcher.Invoke(() =>
                {
                    gfx?.DrawLine(pen, 0, (int)(MyCanvas.ActualHeight / 2), (int)MyCanvas.ActualWidth, (int)(MyCanvas.ActualHeight / 2));
                    gfx?.DrawLine(pen, (int)(MyCanvas.ActualWidth / 2), 0, (int)(MyCanvas.ActualWidth / 2), (int)(MyCanvas.ActualHeight));
                    Random random = new Random();
                    for (int i = 0; i < G.Length-1; i++)
                    {
                        gfx?.DrawLine(pen2, (int) G[i], random.Next(1, (int)MyCanvas.ActualHeight), (int)G[i+1], random.Next(1, (int)MyCanvas.ActualHeight));
                    }
                });

                this.Dispatcher.Invoke(() =>
                {
                    MyImage.Source = BmpImageFromBmp(bmp);
                });
            }
        }

        private void Go()
        {
            while (true)
            {
                this.Dispatcher.Invoke(() =>
                {
                    StepTextBox.Text = Step.ToString();
                });
                Step++;
                Render();
            }
        }
        private void MyCanvas_OnLoaded(object sender, RoutedEventArgs e)
        {

        }
       
        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            StepTextBox.Text = Step.ToString();
        }
    }

   
}
