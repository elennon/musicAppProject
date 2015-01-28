using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace MyMusic
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class bp : Page
    {
        public bp()
        {
            this.InitializeComponent();
        }

        private DispatcherTimer clock = new DispatcherTimer();
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            clock.Interval = new TimeSpan(0, 0, 10);
            clock.Tick += clock_Tick;
            //clock.Start();
        }

        void clock_Tick(object sender, object e)
        {
            throw new NotImplementedException();
        }


        private void ClosePopupClicked(object sender, RoutedEventArgs e)
        {
            // if the Popup is open, then close it 
            if (StandardPopup.IsOpen) { StandardPopup.IsOpen = false; }
        }

        // Handles the Click event on the Button on the page and opens the Popup. 
        private async void ShowPopupOffsetClicked(object sender, RoutedEventArgs e)
        {
            // open the Popup if it isn't open already 
            if (!StandardPopup.IsOpen) { StandardPopup.IsOpen = true; }
            await Task.Delay(TimeSpan.FromSeconds(1));
            if (StandardPopup.IsOpen) { StandardPopup.IsOpen = false; }
        } 






    }
}
