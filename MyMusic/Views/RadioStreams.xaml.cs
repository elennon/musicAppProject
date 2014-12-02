using MyMusic.Common;
using MyMusic.Models;
using MyMusic.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace MyMusic.Views
{

    public sealed partial class RadioStreams : Page
    {
        public RadioStreamsViewModel rsvm = new RadioStreamsViewModel();

        public RadioStreams()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            lstGenre.ItemsSource = await rsvm.Getgenres();    
            //lstGenre.ItemsSource = rsvm.GetXmlGenres().GroupBy(p => p.RadioGenre).Select(g => g.First());
        }

        private void lstGenre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var gid = ((ListBox)sender).SelectedValue;
            this.Frame.Navigate(typeof(ChooseRadio), gid);
        }
    }
}
