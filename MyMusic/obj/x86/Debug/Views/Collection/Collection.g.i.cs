﻿

#pragma checksum "C:\Users\ed\Downloads\4thYearProject-master\4thYearProject-master\MyMusic\Views\Collection\Collection.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "D544D26C584B603D88ADD129B5AF58AA"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyMusic.Views
{
    partial class Collection : global::MyMusic.Common.BindablePage
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::MyMusic.Converters.ItemClickedConverter ItemClickedConverter; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::MyMusic.Converters.BooleanToVisibilityConverter BooleanToVisibilityConverter; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Hub CollectionHub; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Media.ImageBrush bkImage; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.HubSection AllTracksSection; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.HubSection TopPlaySection; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.HubSection ArtistSection; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.HubSection AlbumSection; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.HubSection GenreSection; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.HubSection QuickPickSection; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.CommandBar appBar; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.AppBarButton appBarShuffle; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.AppBarButton appBarEdit; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.AppBarButton appBarAddToQp; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.AppBarButton appBarShowBinned; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private bool _contentLoaded;

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent()
        {
            if (_contentLoaded)
                return;

            _contentLoaded = true;
            global::Windows.UI.Xaml.Application.LoadComponent(this, new global::System.Uri("ms-appx:///Views/Collection/Collection.xaml"), global::Windows.UI.Xaml.Controls.Primitives.ComponentResourceLocation.Application);
 
            ItemClickedConverter = (global::MyMusic.Converters.ItemClickedConverter)this.FindName("ItemClickedConverter");
            BooleanToVisibilityConverter = (global::MyMusic.Converters.BooleanToVisibilityConverter)this.FindName("BooleanToVisibilityConverter");
            CollectionHub = (global::Windows.UI.Xaml.Controls.Hub)this.FindName("CollectionHub");
            bkImage = (global::Windows.UI.Xaml.Media.ImageBrush)this.FindName("bkImage");
            AllTracksSection = (global::Windows.UI.Xaml.Controls.HubSection)this.FindName("AllTracksSection");
            TopPlaySection = (global::Windows.UI.Xaml.Controls.HubSection)this.FindName("TopPlaySection");
            ArtistSection = (global::Windows.UI.Xaml.Controls.HubSection)this.FindName("ArtistSection");
            AlbumSection = (global::Windows.UI.Xaml.Controls.HubSection)this.FindName("AlbumSection");
            GenreSection = (global::Windows.UI.Xaml.Controls.HubSection)this.FindName("GenreSection");
            QuickPickSection = (global::Windows.UI.Xaml.Controls.HubSection)this.FindName("QuickPickSection");
            appBar = (global::Windows.UI.Xaml.Controls.CommandBar)this.FindName("appBar");
            appBarShuffle = (global::Windows.UI.Xaml.Controls.AppBarButton)this.FindName("appBarShuffle");
            appBarEdit = (global::Windows.UI.Xaml.Controls.AppBarButton)this.FindName("appBarEdit");
            appBarAddToQp = (global::Windows.UI.Xaml.Controls.AppBarButton)this.FindName("appBarAddToQp");
            appBarShowBinned = (global::Windows.UI.Xaml.Controls.AppBarButton)this.FindName("appBarShowBinned");
        }
    }
}



