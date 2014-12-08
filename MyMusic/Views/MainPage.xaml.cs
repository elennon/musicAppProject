﻿using MyMusic.Common;
using MyMusic.Models;
using MyMusic.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MyMusic.Views
{
    
    public sealed partial class MainPage : Page
    {
        private TracksViewModel trkView = new TracksViewModel();
        private RadioStreamsViewModel rdoView = new RadioStreamsViewModel();
        
        private readonly NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public MainPage()
        {
            InitializeComponent();

            //this.NavigationCacheMode = NavigationCacheMode.Required;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            ApplicationSettingsHelper.SaveSettingsValue(Constants.AppState, Constants.ForegroundAppActive);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            lstOptions.SelectedIndex = -1;            
        }

        private void lstOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = (ListBox)sender;

            if (lb.SelectedIndex != -1)
            {
                string title = ((ListBoxItem)lb.SelectedItem).Tag.ToString();
                switch (title)
                {
                    case "Stream":
                        this.Frame.Navigate(typeof(Streaming));
                        break;
                    case "Collection":
                        this.Frame.Navigate(typeof(Collection));
                        break;
                    case "Radio":
                        //RadioStream rs = new RadioStream { RadioUrl = "apples" };     
                        //this.Frame.Navigate(typeof(NowPlaying), rs);
                        this.Frame.Navigate(typeof(RadioStreams));
                        break;
                }
            }
        }

       

        private void btnNowPlaying_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(NowPlaying));
        }

        private void FillDbButton_Click(object sender, RoutedEventArgs e)
        {
            trkView.fillDB();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            rdoView.AddRadios();
        }

        private void ShortCutButton_Click(object sender, RoutedEventArgs e)
        {
            //this.Frame.Navigate(typeof(BasicPage1));
        }

        #region NavigationHelper

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {

        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the app that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("cleaning windows");
        }
    }
}
