﻿

#pragma checksum "C:\Users\ed\Downloads\4thYearProject-master\4thYearProject-master\MyMusic\Views\Streaming\CreateListFromQP.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "FDD38AC163CD12B5F727FC03F8614005"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyMusic.Views.Streaming
{
    partial class CreateListFromQP : global::MyMusic.Common.BindablePage
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::MyMusic.Converters.ItemClickedConverter ItemClickedConverter; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::MyMusic.Converters.BooleanToVisibilityConverter BooleanToVisibilityConverter; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Grid LayoutRoot; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Hub Hub; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Media.ImageBrush bkImage; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.HubSection QuickPickSection; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.HubSection ArtistResultsSection; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.HubSection TracksSection; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.CommandBar appBar; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private bool _contentLoaded;

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent()
        {
            if (_contentLoaded)
                return;

            _contentLoaded = true;
            global::Windows.UI.Xaml.Application.LoadComponent(this, new global::System.Uri("ms-appx:///Views/Streaming/CreateListFromQP.xaml"), global::Windows.UI.Xaml.Controls.Primitives.ComponentResourceLocation.Application);
 
            ItemClickedConverter = (global::MyMusic.Converters.ItemClickedConverter)this.FindName("ItemClickedConverter");
            BooleanToVisibilityConverter = (global::MyMusic.Converters.BooleanToVisibilityConverter)this.FindName("BooleanToVisibilityConverter");
            LayoutRoot = (global::Windows.UI.Xaml.Controls.Grid)this.FindName("LayoutRoot");
            Hub = (global::Windows.UI.Xaml.Controls.Hub)this.FindName("Hub");
            bkImage = (global::Windows.UI.Xaml.Media.ImageBrush)this.FindName("bkImage");
            QuickPickSection = (global::Windows.UI.Xaml.Controls.HubSection)this.FindName("QuickPickSection");
            ArtistResultsSection = (global::Windows.UI.Xaml.Controls.HubSection)this.FindName("ArtistResultsSection");
            TracksSection = (global::Windows.UI.Xaml.Controls.HubSection)this.FindName("TracksSection");
            appBar = (global::Windows.UI.Xaml.Controls.CommandBar)this.FindName("appBar");
        }
    }
}



