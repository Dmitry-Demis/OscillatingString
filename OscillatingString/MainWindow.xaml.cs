using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        public MainWindow()
        {
            InitializeComponent();
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
        private void MyCanvas_OnLoaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
