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
        private List<StackPanel> _timerPanels;
        private DispatcherTimer _masterTimer;
        private int _timerCount;

        #region setup

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            InitTimers();
            _timerPanels = new List<StackPanel>();
            InitMasterTimer();
            _timerCount = 0;
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

        private StackPanel FindTimerPanel(Timer timer)
        {
            return _timerPanels.First(x => ((TextBlock)x.Children.First()).Name.Equals(timer._name + "_block"));
        }

        private TextBlock FindTimerBlock(Timer timer)
        {
            return (TextBlock)FindTimerPanel(timer).Children.First();
        }

        #endregion

        #region new_timer_button_Click

        private void new_timer_button_Click(object sender, RoutedEventArgs e)
        {
            var timer = InitSingleTimer();
            var timerPanel = InitTimerPanel();
            var timeBlock = InitTimeBlock(timer);
            var toggleButton = InitToggleButton(timer);
            var removeButton = InitRemoveButton(timer);

            timerPanel.Children.Insert(0, removeButton);
            timerPanel.Children.Insert(0, toggleButton);
            timerPanel.Children.Insert(0, timeBlock);
            _timerPanels.Add(timerPanel);
            main_panel.Children.Insert(0, timerPanel);

            _timers.Add(timer);
        }

        private Timer InitSingleTimer()
        {
            var name = "time_" + _timerCount;
            var timer = new Timer(name, GetTimeSpan());

            _timerCount++;
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
            toggleButton.Margin = new Thickness(0, 0, 20, 0);

            return toggleButton;
        }

        private Button InitRemoveButton(Timer timer)
        {
            var removeButton = new Button();
            removeButton.Name = timer._name + "_remove";
            removeButton.Click += remove_button_Handler();
            removeButton.Content = "Remove?";

            return removeButton;
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

        #region remove_button_Click

        private void remove_button_Click(object sender, RoutedEventArgs e)
        {
            var name = ((Button)sender).Name;
            var timer = _timers.First(x => x._name.Equals(name.Replace("_remove", "")));
            var panel = FindTimerPanel(timer);

            main_panel.Children.Remove(panel);
            _timerPanels.Remove(panel);
            _timers.Remove(timer);
        }

        #endregion

        #region event handlers
        private RoutedEventHandler toggle_button_Handler()
        {
            return new RoutedEventHandler(toggle_button_Click);
        }

        private RoutedEventHandler remove_button_Handler()
        {
            return new RoutedEventHandler(remove_button_Click);
        }
        #endregion
    }
}
