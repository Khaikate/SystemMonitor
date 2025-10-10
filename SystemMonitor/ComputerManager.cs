using LibreHardwareMonitor.Hardware;
using System.Diagnostics;

public static class ComputerManager
{
    public static readonly Computer Instance = new()
    {
        IsCpuEnabled = true,
        IsGpuEnabled = true,
        IsMemoryEnabled = true
    };

    public static bool IsMonitoringAvailable { get; private set; }

    static ComputerManager()
    {
        try 
        { 
            Instance.Open();
            IsMonitoringAvailable = true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Lỗi khi khởi tạo theo dõi phần cứng: {ex.Message}");
            IsMonitoringAvailable = false;
            /* ignore nếu không có quyền admin */ 
        }
    }
}
