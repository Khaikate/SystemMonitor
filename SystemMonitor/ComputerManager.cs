using LibreHardwareMonitor.Hardware;
using System;
using System.Diagnostics;

namespace SystemMonitor
{
    public sealed class ComputerManager
    {
        // Thread-safe Singleton implementation using Lazy<T>
        private static readonly Lazy<ComputerManager> lazyInstance = new(() => new ComputerManager());
        public static ComputerManager Instance => lazyInstance.Value;

        public Computer Computer { get; }
        public bool IsMonitoringAvailable { get; }

        // Private constructor ensures a single instance
        private ComputerManager()
        {
            Computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
            };

            try
            {
                Computer.Open();
                IsMonitoringAvailable = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing hardware monitoring: {ex.Message}");
                IsMonitoringAvailable = false;
                // Silently ignore if lacking admin rights, the UI will show a message.
            }
        }
    }
}
