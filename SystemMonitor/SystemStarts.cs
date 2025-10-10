using LibreHardwareMonitor.Hardware;
using System;
using System.Diagnostics;
using System.Linq;

namespace SystemMonitor
{
    public static class SystemStats
    {
        private static ISensor? cpuLoad, cpuTemp, cpuClock;
        private static ISensor? ramLoad, ramUsed, ramAvailable;
        private static ISensor? gpuLoad, gpuTemp;
        private static string? gpuName;

        private static PerformanceCounter? diskReadCounter, diskWriteCounter;
        private static PerformanceCounter? netRecvCounter, netSentCounter;

        static SystemStats()
        {
            if (!ComputerManager.IsMonitoringAvailable)
            {
                Debug.WriteLine("Skipping SystemStats initialization as monitoring is not available.");
                return;
            }

            InitializeHardwareSensors();
            InitializePerformanceCounters();
        }

        private static void InitializeHardwareSensors()
        {
            var computer = ComputerManager.Instance;

            var cpu = computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);
            if (cpu != null)
            {
                cpu.Update();
                // Use exact sensor names based on user's debug output for maximum reliability
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

            var gpu = computer.Hardware.FirstOrDefault(h => h.HardwareType is HardwareType.GpuNvidia or HardwareType.GpuAmd);
            if (gpu != null)
            {
                gpu.Update();
                gpuName = gpu.Name;
                gpuLoad = gpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Load && s.Name.Contains("Core"));
                gpuTemp = gpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature && s.Name.Contains("Core"))
                          ?? gpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature);
            }
        }

        private static void InitializePerformanceCounters()
        {
            try
            {
                diskReadCounter = new PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", "_Total");
                diskWriteCounter = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", "_Total");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not initialize Disk counters: {ex.Message}");
            }

            try
            {
                var category = new PerformanceCounterCategory("Network Interface");
                var instanceNames = category.GetInstanceNames();
                var bestInterface = instanceNames.FirstOrDefault(s => !s.Contains("Loopback") && IsAdapterActive(s));
                if (bestInterface != null)
                {
                    netRecvCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", bestInterface);
                    netSentCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", bestInterface);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not initialize Network counters: {ex.Message}");
            }
        }

        private static bool IsAdapterActive(string adapterName)
        {
            try
            {
                var upCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", adapterName);
                upCounter.NextValue();
                System.Threading.Thread.Sleep(100);
                return upCounter.NextValue() > 0;
            }
            catch { return false; }
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
                CpuLoad = cpuLoad?.Value,
                CpuTemp = cpuTemp?.Value,
                CpuClockGHz = cpuClock?.Value is float mhz ? mhz / 1000f : null,

                RamLoad = ramLoad?.Value,
                RamUsedGB = ramUsed?.Value,
                RamTotalGB = (ramUsed?.Value ?? 0) + (ramAvailable?.Value ?? 0),

                GpuName = gpuName,
                GpuLoad = gpuLoad?.Value,
                GpuTemp = gpuTemp?.Value,

                DiskReadMBs = diskReadCounter?.NextValue() / (1024 * 1024),
                DiskWriteMBs = diskWriteCounter?.NextValue() / (1024 * 1024),

                NetRecvKBs = netRecvCounter?.NextValue() / 1024,
                NetSentKBs = netSentCounter?.NextValue() / 1024
            };
        }
    }
}
