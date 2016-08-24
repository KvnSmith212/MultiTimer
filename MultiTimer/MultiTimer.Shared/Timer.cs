using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MultiTimer
{
    class Timer
    {
        private TextBlock timeBlock;
        private TimeSpan tickTime;
        private TimeSpan time;
        private DispatcherTimer dispatchTimer;

        public Timer(TextBlock block, TimeSpan time)
        {
            timeBlock = block;
            this.time = time;

            tickTime = TimeSpan.FromSeconds(1);
            InitDispatcher(tickTime);
        }

        public String TimerString()
        {
            return time.ToString();
        }

        private void InitDispatcher(TimeSpan span) 
        {
            dispatchTimer = new DispatcherTimer();
            dispatchTimer.Tick += tick;
            dispatchTimer.Interval = span;
            dispatchTimer.Start();
        }

        void tick(object sender, object e)
        {
            Debug.WriteLine("Timer firing off... " + time.ToString());
            time = time.Subtract(TimeSpan.FromSeconds(1));
            timeBlock.Text = time.ToString();
        }
    }
}
