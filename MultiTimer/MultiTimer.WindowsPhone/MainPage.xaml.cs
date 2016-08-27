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
                var block = timer_panel.Children.
                    OfType<TextBlock>().
                    First(x => x.Name.Equals(timer._name + "_block"));
                block.Text = timer.ToString();
            }
        }

        #endregion

        #region new_timer_button_Click

        private void new_timer_button_Click(object sender, RoutedEventArgs e)
        {
            var timer = InitSingleTimer();
            var timeBlock = InitTimeBlock(timer);
            var startButton = InitStartButton(timer);

            _timers.Add(timer);
            _timerBlocks.Add(timeBlock);
            _timerButtons.Add(startButton);

            timer_panel.Children.Insert(0, timeBlock);
            timer_panel.Children.Insert(0, startButton);
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

        private TextBlock InitTimeBlock(Timer timer)
        {
            var timeBlock = new TextBlock();
            timeBlock.Text = timer.ToString();
            timeBlock.Name = timer._name + "_block";

            return timeBlock;
        }

        private Button InitStartButton(Timer timer)
        {
            var startButton = new Button();
            startButton.Name = timer._name + "_button";
            startButton.Click += toggle_button_Handler();

            return startButton;
        }

        private TimeSpan GetTimeSpan()
        {
            var timeSpan = "0:" + hours.Text + ":" + minutes.Text + ":" + seconds.Text;
            try
            {
                Debug.WriteLine("User created timespan: " + timeSpan);
                return TimeSpan.Parse(timeSpan);
            }
            catch (Exception e)
            {
                //todo: handle exception more gracefully
                time_error.Visibility = Visibility.Visible;
                return TimeSpan.Zero;
            }
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
