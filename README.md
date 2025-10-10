# SystemMonitor - A System Monitoring Application

SystemMonitor is a desktop application for Windows, developed with WPF (.NET 8), used for real-time monitoring of critical computer hardware metrics.

## Features

- **CPU:** Monitors load (%), temperature (°C), and clock speed (GHz).
- **GPU:** Monitors load (%) and temperature (°C) for NVIDIA and AMD graphics cards.
- **RAM:** Tracks used memory, total memory, and usage percentage.
- **Disk:** Displays the read and write speed of the physical disk (MB/s).
- **Network:** Displays data receiving and sending speed (KB/s).
- **Interface:** Visualizes data using charts from the LiveCharts library.

## Requirements

- **Operating System:** Windows 10 or later.
- **Framework:** .NET 8 Desktop Runtime.

## Technologies Used

- **Platform:** .NET 8, WPF
- **Hardware Monitoring:** `LibreHardwareMonitorLib`
- **Charting:** `LiveCharts.Wpf`
- **Architecture:** MVVM (using `CommunityToolkit.Mvvm`)
