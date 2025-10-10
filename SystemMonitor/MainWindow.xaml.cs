
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace SystemMonitor
{
    public partial class MainWindow
    {
        private readonly DispatcherTimer timer = new();
        private readonly SystemStatsViewModel viewModel = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = viewModel;

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += async (_, __) => await viewModel.RefreshAsync();
            timer.Start();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            timer.Stop();
            try 
            { 
                ComputerManager.Instance.Close(); 
            }
            catch 
            { 
                // Ignore exceptions on close
            }
            base.OnClosed(e);
        }
    }
}
