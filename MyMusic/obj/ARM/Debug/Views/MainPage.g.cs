﻿

#pragma checksum "C:\Users\ed\Downloads\4thYearProject-master\4thYearProject-master\MyMusic\Views\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "D289AD3FA3A06659F7B614464B4A0E8D"
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
    partial class MainPage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 105 "..\..\..\Views\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.ListViewBase)(target)).ItemClick += this.Streaming_ItemClick;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 81 "..\..\..\Views\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.ListViewBase)(target)).ItemClick += this.Collection_ItemClick;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 63 "..\..\..\Views\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.ListViewBase)(target)).ItemClick += this.RadioStream_ItemClick;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 117 "..\..\..\Views\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.btnNowPlaying_Click;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 118 "..\..\..\Views\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.ShortCutButton_Click;
                 #line default
                 #line hidden
                break;
            case 6:
                #line 119 "..\..\..\Views\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.TestButton_Click;
                 #line default
                 #line hidden
                break;
            case 7:
                #line 122 "..\..\..\Views\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.FillDbButton_Click;
                 #line default
                 #line hidden
                break;
            case 8:
                #line 123 "..\..\..\Views\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.UpdateButton_Click;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


