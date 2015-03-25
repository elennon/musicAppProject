using MyMusic.Common;
using System;
using System.Collections.Generic;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace MyMusic.Views
{
    public sealed partial class Collection : BindablePage
    {
        public static Collection CollectionView;
        public AppBarButton shuffle, edit, addToQp, showBinned;
        public Hub collHub;
        public HubSection allTracksSec, topPlaysSec, artistSec, albumSec, genreSec, qpSec;
        public AppBar appBarr;
        public ListView allTracksLv;

        public NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public Collection()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            CollectionView = this;
            edit = appBarEdit; shuffle = appBarShuffle; addToQp = appBarAddToQp; showBinned = appBarShowBinned;
            collHub = CollectionHub;
            allTracksSec = AllTracksSection; topPlaysSec = TopPlaySection; artistSec = ArtistSection; albumSec = AlbumSection;
            genreSec = GenreSection; qpSec = QuickPickSection;
            appBarr = appBar;
            
        }

        #region NavigationHelper registration

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #endregion

       
    }
}
