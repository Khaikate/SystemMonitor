namespace SystemMonitor;

public record SystemStatsSnapshot
{
    // CPU
    public string? CpuName { get; init; }
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

    public override string ToString()
    {
        return $"CPU: {CpuName} | {CpuLoad:F1}% | {CpuTemp:F1}°C | {CpuClockGHz:F2} GHz\n" +
               $"RAM: {RamLoad:F1}% ({RamUsedGB:F1}/{RamTotalGB:F1} GB)\n" +
               $"GPU: {GpuName} | {GpuLoad:F1}% | {GpuTemp:F1}°C";
    }
}
