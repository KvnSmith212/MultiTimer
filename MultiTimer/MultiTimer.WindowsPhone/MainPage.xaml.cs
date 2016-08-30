using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MultiTimer
{
    public sealed partial class MainPage : Page
    {
        private List<Timer> _timers;
        private List<TextBlock> _timerBlocks;
        private List<Button> _timerButtons;
        private List<StackPanel> _timerPanels;
        private DispatcherTimer _masterTimer;

        #region setup

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            InitTimers();
            _timerBlocks = new List<TextBlock>();
            _timerButtons = new List<Button>();
            _timerPanels = new List<StackPanel>();
            InitMasterTimer();
        }

        private void InitTimers()
        {
            _timers = new TimerRepo().LoadTimers();
        }

        private void InitMasterTimer()
        {
            _masterTimer = new DispatcherTimer();
            _masterTimer.Tick += UpdateAllTimers;
            _masterTimer.Interval = TimeSpan.FromSeconds(1);
            _masterTimer.Start();
        }

        #endregion

        #region timer logic

        private void UpdateAllTimers(object sender, object e)
        {
            foreach (var timer in _timers)
            {
                if (timer._time.CompareTo(TimeSpan.Zero) <= 0)
                {
                    timer.StopTimer();
                    timer._time = timer._origTime;
                }

                var block = FindTimerBlock(timer);
                block.Text = timer.ToString();
            }
        }

        private TextBlock FindTimerBlock(Timer timer)
        {
            var panel = _timerPanels.First(x => ((TextBlock)x.Children.First()).Name.Equals(timer._name + "_block"));
            return (TextBlock)panel.Children.First();
        }

        #endregion

        #region new_timer_button_Click

        private void new_timer_button_Click(object sender, RoutedEventArgs e)
        {
            var timer = InitSingleTimer();
            var timerPanel = InitTimerPanel();
            var timeBlock = InitTimeBlock(timer);
            var toggleButton = InitToggleButton(timer);

            timerPanel.Children.Insert(0, toggleButton);
            timerPanel.Children.Insert(0, timeBlock);
            _timerBlocks.Add(timeBlock);
            _timerButtons.Add(toggleButton);

            _timerPanels.Add(timerPanel);
            main_panel.Children.Insert(0, timerPanel);

            _timers.Add(timer);
        }

        private Timer InitSingleTimer()
        {
            var name = "time_";
            if (!_timers.Any())
                name = name + "0";
            else
                name = name + _timers.Count;
            var timer = new Timer(name, GetTimeSpan());
            //todo: this will need to be changed once we start saving/adding/removing timers from the list
            return timer;
        }

        private StackPanel InitTimerPanel()
        {
            var panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;

            return panel;
        }

        private TextBlock InitTimeBlock(Timer timer)
        {
            var timeBlock = new TextBlock();
            timeBlock.Text = timer.ToString();
            timeBlock.Name = timer._name + "_block";
            timeBlock.VerticalAlignment = VerticalAlignment.Center;
            timeBlock.Margin = new Thickness(0, 0, 20, 0);
            timeBlock.FontSize = 20;

            return timeBlock;
        }

        private Button InitToggleButton(Timer timer)
        {
            var toggleButton = new Button();
            toggleButton.Name = timer._name + "_button";
            toggleButton.Click += toggle_button_Handler();
            toggleButton.Content = "toggle";

            return toggleButton;
        }

        private TimeSpan GetTimeSpan()
        {
            var timeSpan = "";
            if (String.IsNullOrEmpty(hours.Text))
                hours.Text = "0";
            if (String.IsNullOrEmpty(minutes.Text))
                minutes.Text = "0";
            if (String.IsNullOrEmpty(seconds.Text))
                seconds.Text = "0";
            timeSpan = "0:" + hours.Text + ":" + minutes.Text + ":" + seconds.Text;

            return TimeSpan.Parse(timeSpan);
        }

        #endregion

        #region toggle_button_Click

        private void toggle_button_Click(object sender, RoutedEventArgs e)
        {
            var name = ((Button)sender).Name;
            var timer = _timers.First(x => x._name.Equals(name.Replace("_button", "")));

            if (timer.IsRunning())
                timer.StopTimer();
            else
                timer.StartTimer();
        }

        #endregion

        #region event handlers
        private RoutedEventHandler toggle_button_Handler()
        {
            return new RoutedEventHandler(toggle_button_Click);
        }

        #endregion
    }
}
