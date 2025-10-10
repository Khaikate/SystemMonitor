using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Windows.Media;

namespace SystemMonitor
{
    public partial class FpsCounter : ObservableObject
    {
        [ObservableProperty]
        private int fps;

        private int frameCount;
        private DateTime lastTime;

        public FpsCounter()
        {
            lastTime = DateTime.Now;
            CompositionTarget.Rendering += OnRendering;
        }

        private void OnRendering(object? sender, EventArgs e)
        {
            frameCount++;
            DateTime now = DateTime.Now;
            double timeSinceLastUpdate = (now - lastTime).TotalSeconds;

            if (timeSinceLastUpdate >= 1.0)
            {
                Fps = (int)(frameCount / timeSinceLastUpdate);
                frameCount = 0;
                lastTime = now;
            }
        }
    }
}
