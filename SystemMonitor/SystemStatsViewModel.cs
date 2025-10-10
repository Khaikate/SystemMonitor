using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SystemMonitor;

public partial class SystemStatsViewModel : ObservableObject
{
    // CPU
    [ObservableProperty] private string? cpuName;
    [ObservableProperty] private float? cpuLoad;
    [ObservableProperty] private float? cpuTemp;
    [ObservableProperty] private float? cpuClockGHz;

    // RAM
    [ObservableProperty] private float? ramLoad;
    [ObservableProperty] private float? ramUsedGB;
    [ObservableProperty] private float? ramTotalGB;

    // GPU
    [ObservableProperty] private string? gpuName;
    [ObservableProperty] private float? gpuLoad;
    [ObservableProperty] private float? gpuTemp;

    public async Task RefreshAsync()
    {
        if (!ComputerManager.Instance.IsMonitoringAvailable)
        {
            return; // Don't do anything if we can't monitor
        }

        try
        {
            var snap = await Task.Run(SystemStats.GetSnapshot);
            if (snap != null)
            {
                CpuName = snap.CpuName;
                CpuLoad = snap.CpuLoad;
                CpuTemp = snap.CpuTemp;
                CpuClockGHz = snap.CpuClockGHz;

                RamLoad = snap.RamLoad;
                RamUsedGB = snap.RamUsedGB;
                RamTotalGB = snap.RamTotalGB;

                GpuName = snap.GpuName;
                GpuLoad = snap.GpuLoad;
                GpuTemp = snap.GpuTemp;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error refreshing system stats: {ex.Message}");
        }
    }
}
