using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

namespace SystemMonitor
{
    public partial class MainViewModel : ObservableObject
    {
        // Hardware stats properties
        [ObservableProperty] private string? cpuName;
        [ObservableProperty] private float? cpuLoad;
        [ObservableProperty] private float? cpuTemp;
        [ObservableProperty] private float? cpuClockGHz;
        [ObservableProperty] private float? ramLoad;
        [ObservableProperty] private float? ramUsedGB;
        [ObservableProperty] private float? ramTotalGB;
        [ObservableProperty] private string? gpuName;
        [ObservableProperty] private float? gpuLoad;
        [ObservableProperty] private float? gpuTemp;

        // UI performance property
        [ObservableProperty] private int? fps;

        private readonly FpsCounter fpsCounter;

        public MainViewModel()
        {
            // Initialize the FPS counter and subscribe to its updates
            fpsCounter = new FpsCounter();
            fpsCounter.PropertyChanged += FpsCounter_PropertyChanged;

            if (ComputerManager.Instance.IsMonitoringAvailable)
            {
                StartMonitoring();
            }
            else
            {
                LoadSampleData();
                MessageBox.Show("Hardware monitoring is not available.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void FpsCounter_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FpsCounter.Fps))
            {
                Fps = fpsCounter.Fps;
            }
        }

        private void StartMonitoring()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    var snapshot = SystemStats.GetSnapshot();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        UpdateProperties(snapshot);
                    });

                    await Task.Delay(1000); // Refresh rate
                }
            });
        }

        private void UpdateProperties(SystemStatsSnapshot snapshot)
        {
            CpuName = snapshot.CpuName;
            CpuLoad = snapshot.CpuLoad;
            CpuTemp = snapshot.CpuTemp;
            CpuClockGHz = snapshot.CpuClockGHz;

            RamLoad = snapshot.RamLoad;
            RamUsedGB = snapshot.RamUsedGB;
            RamTotalGB = snapshot.RamTotalGB;

            GpuName = snapshot.GpuName;
            GpuLoad = snapshot.GpuLoad;
            GpuTemp = snapshot.GpuTemp;
            // Fps is now updated by FpsCounter, no longer from the snapshot
        }

        private void LoadSampleData()
        {
            var sampleSnapshot = new SystemStatsSnapshot
            {
                CpuName = "Intel Core i9-9900K",
                CpuLoad = 35.5f,
                CpuTemp = 60.0f,
                CpuClockGHz = 4.7f,
                RamLoad = 50.0f,
                RamUsedGB = 8.1f,
                RamTotalGB = 16.0f,
                GpuName = "NVIDIA GeForce RTX 2080 Ti",
                GpuLoad = 70.0f,
                GpuTemp = 75.0f,
            };
            UpdateProperties(sampleSnapshot);
            Fps = 60; // Sample FPS
        }
    }
}
