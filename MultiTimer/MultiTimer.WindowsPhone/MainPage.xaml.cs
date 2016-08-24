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
        private List<Timer> timers;

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            timers = new TimerRepo().LoadTimers();
        }

        private void new_timer_button_Click(object sender, RoutedEventArgs e)
        {
            var timeblock = new TextBlock();
            timers.Add(new Timer(timeblock, TimeSpan.FromSeconds(60)));
            timeblock.Text = timers.First().TimerString();
            timer_panel.Children.Add(timeblock);
        }
    }
}
