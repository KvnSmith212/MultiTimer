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
        public String _name { get; set; }
        private TimeSpan _tickTime;
        public TimeSpan _time { get; set; }
        public TimeSpan _origTime { get; set; }
        private DispatcherTimer _dispatchTimer;

        public Timer(String name, TimeSpan time)
        {
            _name = name;
            _time = time;
            _origTime = time;
            _tickTime = TimeSpan.FromSeconds(1);
            InitDispatcher(_tickTime);
        }

        private void InitDispatcher(TimeSpan span) 
        {
            _dispatchTimer = new DispatcherTimer();
            _dispatchTimer.Tick += Tick;
            _dispatchTimer.Interval = span;
        }

        private void Tick(object sender, object e)
        {
            //todo: will need to change this once I start storing/adding/removing timers
            _time = _time.Subtract(TimeSpan.FromSeconds(1));
        }

        public bool IsRunning()
        {
            return _dispatchTimer.IsEnabled;
        }

        public void StartTimer()
        {
            _dispatchTimer.Start();
        }

        public void StopTimer()
        {
            _dispatchTimer.Stop();
        }

        public String ToString()
        {
            return _time.ToString();
        }
    }
}
