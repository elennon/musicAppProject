using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using MyMusic.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMusic.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            var navigationService = this.CreateNavigationService();
            SimpleIoc.Default.Register<INavigationService>(() => navigationService);

            SimpleIoc.Default.Register<IDialogService, DialogService>();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<RadioViewModel>();

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
           
        }

        private INavigationService CreateNavigationService()
        {
            var navigationService = new NavigationService();
            navigationService.Configure("Main", typeof(MainPage));
            navigationService.Configure("Radio", typeof(RadioStreams));
            navigationService.Configure("Collection", typeof(Collection));
            navigationService.Configure("NowPlaying", typeof(NowPlaying));
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
    }
}
