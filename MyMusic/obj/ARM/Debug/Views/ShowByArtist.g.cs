﻿

#pragma checksum "C:\Users\ed\Downloads\4thYearProject-master\4thYearProject-master\MyMusic\Views\ShowByArtist.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "4C2FEFD06C6160EF0AE91DBD0596D644"
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
    partial class ShowByArtist : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 25 "..\..\..\Views\ShowByArtist.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.Border_Tapped;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 19 "..\..\..\Views\ShowByArtist.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.Image_Tapped;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 109 "..\..\..\Views\ShowByArtist.xaml"
                ((global::Windows.UI.Xaml.Controls.ListViewBase)(target)).ContainerContentChanging += this.ItemListView_ContainerContentChanging;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}

