using MyMusic.ViewModels;
using MyMusic.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MyMusic.Common
{
    public class BindablePage : Page
    {
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var navigableViewModel = this.DataContext as INavigable;
            if (navigableViewModel != null)
                navigableViewModel.Activate(e.Parameter);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            ((App)Application.Current).isResumingFromTermination = false;       // set this here because its past the point of dealing with resuming from termination or resuming

            var navigableViewModel = this.DataContext as INavigable;
            if (navigableViewModel != null)
            {
                if (navigableViewModel.GetType() == typeof(NowPlayingViewModel))
                {
                    navigableViewModel.Deactivate(null);
                }  
                else
                navigableViewModel.Deactivate(e.Parameter);
            }
        }
    }
}
