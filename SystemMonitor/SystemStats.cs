using LibreHardwareMonitor.Hardware;
using System;
using System.Linq;

namespace SystemMonitor
{
    public static class SystemStats
    {
        private static readonly Computer? computer;
        private static readonly ISensor? cpuLoad, cpuTemp, cpuClock;
        private static readonly string? cpuName;
        private static readonly ISensor? ramLoad, ramUsed, ramAvailable;
        private static readonly ISensor? gpuLoad, gpuTemp;
        private static readonly string? gpuName;

        static SystemStats()
        {
            var computerManager = ComputerManager.Instance;
            if (!computerManager.IsMonitoringAvailable) return;

            computer = computerManager.Computer;

            // CPU Sensors
            var cpu = computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);
            if (cpu != null)
            {
                cpuName = cpu.Name;
                cpuLoad = cpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Load && s.Name == "CPU Total");
                cpuTemp = cpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature && s.Name.Contains("Package"));
                // Be less specific to find any clock sensor, increasing compatibility
                cpuClock = cpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Clock);
            }

            // RAM Sensors
            var ram = computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Memory);
            if (ram != null)
            {
                ramLoad = ram.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Load);
                ramUsed = ram.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Data && s.Name.Contains("Used"));
                ramAvailable = ram.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Data && s.Name.Contains("Available"));
            }

            // GPU Sensors
            var gpu = computer.Hardware.FirstOrDefault(h => h.HardwareType is HardwareType.GpuNvidia or HardwareType.GpuAmd or HardwareType.GpuIntel);
            if (gpu != null)
            {
                gpuName = gpu.Name;
                gpuLoad = gpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Load && s.Name.Contains("Core"));
                gpuTemp = gpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature && s.Name.Contains("Core"));
            }
        }

        public static SystemStatsSnapshot GetSnapshot()
        {
            if (!ComputerManager.Instance.IsMonitoringAvailable || computer == null) return new SystemStatsSnapshot();

            // Update all hardware sensors
            foreach (var hardware in computer.Hardware) hardware.Update();

            return new SystemStatsSnapshot
            {
                CpuName = cpuName,
                CpuLoad = cpuLoad?.Value,
                CpuTemp = cpuTemp?.Value,
                CpuClockGHz = cpuClock?.Value / 1000f, // Convert MHz to GHz

                RamLoad = ramLoad?.Value,
                RamUsedGB = ramUsed?.Value,
                RamTotalGB = (ramUsed?.Value ?? 0) + (ramAvailable?.Value ?? 0),

                GpuName = gpuName,
                GpuLoad = gpuLoad?.Value,
                GpuTemp = gpuTemp?.Value
            };
        }
    }
}
