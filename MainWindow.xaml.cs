using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace MouseRefreshRateTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        static long count = 0;
        long time = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        static Thread show_fps;

        private void Window_Loaded(object sender, EventArgs e)
        {
            show_fps = new Thread(() => {
                while (true)
                {
                    Thread.Sleep(250);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        label1.Content = count * 1000 / (DateTimeOffset.Now.ToUnixTimeMilliseconds() - time) + "/s";
                    });
                    count = 0;
                    time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                }
            });
            show_fps.Start();
        }

        private void count_plus(object sender, MouseEventArgs e)
        {
            count++;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            show_fps.Abort();
        }
    }
}
