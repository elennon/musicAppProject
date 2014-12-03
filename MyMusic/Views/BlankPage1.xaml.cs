using MyMusic.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    
    //public class ContactGroup
    //{
    //    public string Title { get; set; }
    //    public List<TrackViewModel> Tracks { get; set; }
    //}

    public sealed partial class BlankPage1 : Page
    {
        private TracksViewModel trkView = new TracksViewModel();
        public BlankPage1()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            

            CollectionViewSource listViewSource = new CollectionViewSource();
            listViewSource.IsSourceGrouped = true;
            listViewSource.Source = GetContactGroups();
            listViewSource.ItemsPath = new PropertyPath("Tracks");
            lstViewDetail.ItemsSource = listViewSource.View;
            lstViewSummary.ItemsSource = listViewSource.View.CollectionGroups;

        }

        private List<ContactGroup> GetContactGroups()
        {
            List<ContactGroup> trackGroups = new List<ContactGroup>();
            List<ContactGroup> tempGroups = new List<ContactGroup>();
            ObservableCollection<TrackViewModel> allSongs = trkView.GetTracks();
            ObservableCollection<TrackViewModel> songsNotNumbers = new ObservableCollection<TrackViewModel>();
            List<char> firstLetters = new List<char>();
            var t = allSongs.GroupBy(a => a.Name.Substring(0, 1)).Select(g => g.FirstOrDefault().Name);     // get a list of alphabetical letters that songs in collection begin with
            foreach (string item in t)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (Char.IsLetter(item[i]))
                    {
                        firstLetters.Add(Char.ToUpper(item[i]));
                        break;
                    }
                }
            }
            ContactGroup tGroup = new ContactGroup();
            foreach (Char item in firstLetters.OrderBy(a => a.ToString()).Distinct())
            {
                var tracksWithLetter = allSongs.Where(a => Char.ToLower(a.Name[0]) == Char.ToLower(item)).ToList();
                tGroup = new ContactGroup() { Title = item.ToString(), Tracks = allSongs.Where(a => Char.ToLower(a.Name[0]) == Char.ToLower(item)).ToList() };
                tempGroups.Add(tGroup);  
                foreach (var tr in tracksWithLetter)    // collect all tracks that start with a letter
	            {
                    songsNotNumbers.Add(tr);
	            }                
            }
            ObservableCollection<TrackViewModel> numberSongs = new ObservableCollection<TrackViewModel>();  // for all songs that start with numbers
            foreach (var item in allSongs)
            {
                
                if(songsNotNumbers.Contains(item) == false)
                { numberSongs.Add(item); }
            }
            ContactGroup numbersGroup = new ContactGroup() { Title = "#", Tracks = numberSongs.ToList() };
            trackGroups.Add(numbersGroup);
            foreach (var item in tempGroups)
            {
                trackGroups.Add(item);
            }

            return trackGroups;
        }

        private void SemanticZoom_ViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            if (e.IsSourceZoomedInView == false)
            {
                e.DestinationItem.Item = e.SourceItem.Item;
            }
        }


        private void lstViewDetail_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ListView lstView = (ListView)sender;
            string hh = lstView.SelectedValue.ToString();
            // this.Frame.Navigate(typeof(NowPlaying), GetListToPlay(Convert.ToInt32(hh)));
        }
        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }


        private void Border_Tapped(object sender, TappedRoutedEventArgs e)
        {
            semanticZoom.IsZoomedInViewActive = false;
        }
    }
}
