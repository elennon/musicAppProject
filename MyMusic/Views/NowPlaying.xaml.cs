using MyMusic.Common;
using MyMusic.HelperClasses;
using MyMusic.Models;
using MyMusic.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;


namespace MyMusic.Views
{

    public sealed partial class NowPlaying : BindablePage
    {

        private readonly NavigationHelper navigationHelper;
    
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public NowPlaying()
        {
            this.InitializeComponent();
           
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;          
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
           
        }

    }
}



        //private async void Log(string read)
        //{
        //    read = read + Environment.NewLine;
        //    try
        //    {
        //        StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///Assets/Media/debugFile.txt"));
        //        await Windows.Storage.FileIO.AppendTextAsync(file, read);
        //        string text = await Windows.Storage.FileIO.ReadTextAsync(file);                
        //    }
        //    catch (Exception ex)
        //    {
        //        string error = ex.Message;
        //    }           
        //}


