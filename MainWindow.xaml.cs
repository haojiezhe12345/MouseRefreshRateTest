using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

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
        long timeLastFrame = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        static Thread show_fps;

        private void Window_Loaded(object sender, EventArgs e)
        {
            show_fps = new Thread(() => {
                int interval = 250;
                while (true)
                {
                    Thread.Sleep(interval);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        label1.Content = count * 1000 / (DateTimeOffset.Now.ToUnixTimeMilliseconds() - time) + "/s";
                        try
                        {
                            interval = int.Parse(intervalms.Text);
                        } catch { }
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

            if ((bool)showGraph.IsChecked)
            {
                long frametime = DateTimeOffset.Now.ToUnixTimeMilliseconds() - timeLastFrame;
                timeLastFrame = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                Rectangle rectangle = new Rectangle();
                //rectangle.HorizontalAlignment = HorizontalAlignment.Left;
                //rectangle.VerticalAlignment = VerticalAlignment.Bottom;
                rectangle.Width = 4;
                Canvas.SetBottom(rectangle, 0);

                rectangle.Height = frametime * 12.5;
                if (frametime > 20)
                {
                    rectangle.Fill = new SolidColorBrush { Color = Color.FromRgb(255, 0, 0) };
                }
                else
                {
                    rectangle.Fill = new SolidColorBrush { Color = Color.FromRgb((byte)(255 * frametime / 20), (byte)(255 - 255 * frametime / 20), 0) };
                }
                Canvas.SetRight(rectangle, histogramTranslate.X -= 5);
                //rectangle.Margin = new Thickness(histogramCount * 5, 0 ,0, 0);

                if (histogramTranslate.X < 0 - ((Panel)Application.Current.MainWindow.Content).ActualWidth)
                {
                    histogram.Children.RemoveAt(0);
                }

                histogram.Children.Add(rectangle);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            show_fps.Abort();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox.SelectedIndex == 0) {
                win.MouseMove += count_plus;
                win.MouseWheel -= count_plus;
                win.MouseDown -= count_plus;
            }
            else if(comboBox.SelectedIndex == 1)
            {
                win.MouseMove -= count_plus;
                win.MouseWheel += count_plus;
                win.MouseDown -= count_plus;
            }
            else if (comboBox.SelectedIndex == 2)
            {
                win.MouseMove -= count_plus;
                win.MouseWheel -= count_plus;
                win.MouseDown += count_plus;
            }
        }

        private void removeHistogram(object sender, RoutedEventArgs e)
        {
            histogram.Children.RemoveRange(0, histogram.Children.Count);
            histogramTranslate.X = 0;
        }
    }
}
