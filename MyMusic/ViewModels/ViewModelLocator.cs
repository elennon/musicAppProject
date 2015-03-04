using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using MyMusic.Common;
using MyMusic.ViewModels.StreamingPlaylists;
using MyMusic.Views;
using MyMusic.Views.Streaming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMusic.ViewModels
{
    public class ViewModelLocator
    {
        //public IMainViewModel View1Model { get { return IocContainer.Get<MainViewModel>(); } }
        //public RadioViewModel View2Model { get { return IocContainer.Get<RadioViewModel>(); } }
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            var navigationService = this.CreateNavigationService();
            SimpleIoc.Default.Register<INavigationService>(() => navigationService);

            SimpleIoc.Default.Register<IDialogService, DialogService>();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<RadioViewModel>();
            SimpleIoc.Default.Register<testViewModel>();
            SimpleIoc.Default.Register<CollectionViewModel>();
            SimpleIoc.Default.Register<NowPlayingViewModel>();
            SimpleIoc.Default.Register<CreatePlaylistViewModel>();
            SimpleIoc.Default.Register<ViewPlaylistViewModel>();
            SimpleIoc.Default.Register<SavedPlaylistsViewModel>();
            SimpleIoc.Default.Register<AddToPlaylistViewModel>();
            SimpleIoc.Default.Register<GSSignInViewModel>();
            SimpleIoc.Default.Register<GSMainPageViewModel>();

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default); 
           

        }

        private INavigationService CreateNavigationService()
        {
            var navigationService = new NavigationService();
            navigationService.Configure("Main", typeof(MainPage));
            navigationService.Configure("Radio", typeof(RadioStreams));
            navigationService.Configure("Collection", typeof(Collection));
            navigationService.Configure("NowPlaying", typeof(NowPlaying));
            navigationService.Configure("Albums", typeof(Albums));
            navigationService.Configure("ShowAllTracks", typeof(ShowAllTracks));
            navigationService.Configure("CreatePlaylist", typeof(CreatePlaylist));
            navigationService.Configure("ViewPlaylist", typeof(ViewPlaylist));
            navigationService.Configure("SavedPlaylists", typeof(SavedPlaylists));
            navigationService.Configure("AddToPlaylist", typeof(AddToPlaylist));
            navigationService.Configure("GSSignIn", typeof(GSSignIn));
            navigationService.Configure("GSMainPage", typeof(GSMainPage));
            navigationService.Configure("test", typeof(Views.test));
            
            return navigationService;
        }

        public MainViewModel MainViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }
        public RadioViewModel RadioViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<RadioViewModel>();
            }
        }
        public testViewModel testViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<testViewModel>();
            }
        }
        public CollectionViewModel CollectionViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CollectionViewModel>();
            }
        }
        public NowPlayingViewModel NowPlayingViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<NowPlayingViewModel>();
            }
        }
        public CreatePlaylistViewModel CreatePlaylistViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CreatePlaylistViewModel>();
            }
        }
        public ViewPlaylistViewModel ViewPlaylistViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewPlaylistViewModel>();
            }
        }
        public SavedPlaylistsViewModel SavedPlaylistsViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SavedPlaylistsViewModel>();
            }
        }
        public AddToPlaylistViewModel AddToPlaylistViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AddToPlaylistViewModel>();
            }
        }
        public GSSignInViewModel GSSignInViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<GSSignInViewModel>();
            }
        }
        public GSMainPageViewModel GSMainPageViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<GSMainPageViewModel>();
            }
        }
    }
}
