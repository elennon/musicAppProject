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

namespace MyMusic.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Splash : Page
    {
        public Splash()
        {
            this.InitializeComponent(); 
            ExtendeSplashScreen();
        }

        async void ExtendeSplashScreen()
        {
            await Task.Delay(TimeSpan.FromSeconds(3)); 
            Frame.Navigate(typeof(MainPage)); 
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
