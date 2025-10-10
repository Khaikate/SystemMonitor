using LibreHardwareMonitor.Hardware;
using System;
using System.Diagnostics;
using System.Linq;

namespace SystemMonitor
{
    public static class SystemStats
    {
        private static ISensor? cpuLoad, cpuTemp, cpuClock;
        private static string? cpuName;
        private static ISensor? ramLoad, ramUsed, ramAvailable;
        private static ISensor? gpuLoad, gpuTemp;
        private static string? gpuName;

        static SystemStats()
        {
            if (!ComputerManager.IsMonitoringAvailable)
            {
                Debug.WriteLine("Skipping SystemStats initialization as monitoring is not available.");
                return;
            }

            InitializeHardwareSensors();
        }

        private static void InitializeHardwareSensors()
        {
            var computer = ComputerManager.Instance;

            var cpu = computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);
            if (cpu != null)
            {
                cpu.Update();
                cpuName = cpu.Name;
                cpuLoad = cpu.Sensors.FirstOrDefault(s => s.Name == "CPU Total");
                cpuTemp = cpu.Sensors.FirstOrDefault(s => s.Name == "CPU Package");
                cpuClock = cpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Clock && s.Name.Contains("Package")) ?? cpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Clock);
            }

            var ram = computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Memory);
            if (ram != null)
            {
                ram.Update();
                ramLoad = ram.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Load);
                ramUsed = ram.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Data && s.Name.Contains("Used"));
                ramAvailable = ram.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Data && s.Name.Contains("Available"));
            }

            var gpu = computer.Hardware.FirstOrDefault(h => h.HardwareType is HardwareType.GpuNvidia or HardwareType.GpuAmd or HardwareType.GpuIntel);
            if (gpu != null)
            {
                gpu.Update();
                gpuName = gpu.Name;
                gpuLoad = gpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Load && s.Name.Contains("Core"));
                gpuTemp = gpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature && s.Name.Contains("Core"))
                          ?? gpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature);
            }
        }

        public static SystemStatsSnapshot GetSnapshot()
        {
            if (!ComputerManager.IsMonitoringAvailable)
            {
                return new SystemStatsSnapshot();
            }

            foreach (var hw in ComputerManager.Instance.Hardware) hw.Update();

            return new SystemStatsSnapshot
            {
                CpuName = cpuName,
                CpuLoad = cpuLoad?.Value,
                CpuTemp = cpuTemp?.Value,
                CpuClockGHz = cpuClock?.Value is float mhz ? mhz / 1000f : null,

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
