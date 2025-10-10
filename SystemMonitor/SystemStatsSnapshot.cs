namespace SystemMonitor;

public record SystemStatsSnapshot
{
    // CPU
    public float? CpuLoad { get; init; }
    public float? CpuTemp { get; init; }
    public float? CpuClockGHz { get; init; }

    // RAM
    public float? RamLoad { get; init; }
    public float? RamUsedGB { get; init; }
    public float? RamTotalGB { get; init; }

    // GPU
    public string? GpuName { get; init; }
    public float? GpuLoad { get; init; }
    public float? GpuTemp { get; init; }

    // Disk
    public float? DiskReadMBs { get; init; }
    public float? DiskWriteMBs { get; init; }

    // Network
    public float? NetRecvKBs { get; init; }
    public float? NetSentKBs { get; init; }

    public override string ToString()
    {
        return $"CPU: {CpuLoad:F1}% | {CpuTemp:F1}°C | {CpuClockGHz:F2} GHz\n" +
               $"RAM: {RamLoad:F1}% ({RamUsedGB:F1}/{RamTotalGB:F1} GB)\n" +
               $"GPU: {GpuName} | {GpuLoad:F1}% | {GpuTemp:F1}°C\n" +
               $"Disk: R {DiskReadMBs:F1} MB/s | W {DiskWriteMBs:F1} MB/s\n" +
               $"Net: ↓ {NetRecvKBs:F1} KB/s | ↑ {NetSentKBs:F1} KB/s";
    }
}
