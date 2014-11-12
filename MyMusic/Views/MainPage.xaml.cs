﻿using MyMusic.Common;
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
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
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
                        this.Frame.Navigate(typeof(RadioStreams));
                        break;
                }
            }
        }



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

        
    }
}
