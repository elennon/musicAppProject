﻿

#pragma checksum "C:\Users\ed\Downloads\4thYearProject-master\4thYearProject-master\MyMusic\Views\Playlists\SavedPlaylists.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "0CBDDAE6868DA1A464F6BA6377F1495A"
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
    partial class SavedPlaylists : global::MyMusic.Common.BindablePage
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::MyMusic.Converters.ItemClickedConverter ItemClickedConverter; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::MyMusic.Converters.BooleanToVisibilityConverter BooleanToVisibilityConverter; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Grid LayoutRoot; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.StackPanel TitlePanel; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Grid ContentRoot; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Image imgNote; 
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
            global::Windows.UI.Xaml.Application.LoadComponent(this, new global::System.Uri("ms-appx:///Views/Playlists/SavedPlaylists.xaml"), global::Windows.UI.Xaml.Controls.Primitives.ComponentResourceLocation.Application);
 
            ItemClickedConverter = (global::MyMusic.Converters.ItemClickedConverter)this.FindName("ItemClickedConverter");
            BooleanToVisibilityConverter = (global::MyMusic.Converters.BooleanToVisibilityConverter)this.FindName("BooleanToVisibilityConverter");
            LayoutRoot = (global::Windows.UI.Xaml.Controls.Grid)this.FindName("LayoutRoot");
            TitlePanel = (global::Windows.UI.Xaml.Controls.StackPanel)this.FindName("TitlePanel");
            ContentRoot = (global::Windows.UI.Xaml.Controls.Grid)this.FindName("ContentRoot");
            imgNote = (global::Windows.UI.Xaml.Controls.Image)this.FindName("imgNote");
            appBar = (global::Windows.UI.Xaml.Controls.CommandBar)this.FindName("appBar");
        }
    }
}



