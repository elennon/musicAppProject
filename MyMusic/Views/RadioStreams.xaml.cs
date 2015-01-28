using MyMusic.Common;
using MyMusic.Models;
using MyMusic.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace MyMusic.Views
{

    public sealed partial class RadioStreams : BindablePage
    {
        public static RadioStreams thisRadioStreams;
        public Hub thisHub = new Hub();
        public ImageBrush thisHubImage = new ImageBrush();

        public NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }
  
        public RadioStreams()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            thisRadioStreams = this;
            thisHub = RadioHub;
            thisHubImage = bkImage;
        }

        #region NavigationHelper registration

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        //protected override void OnNavigatedFrom(NavigationEventArgs e)
        //{
        //    this.navigationHelper.OnNavigatedFrom(e);
        //}

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #endregion

    }
}

