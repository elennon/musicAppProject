﻿

#pragma checksum "C:\Users\ed\Downloads\4thYearProject-master\4thYearProject-master\MyMusic\Views\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "33D05CC6C1ECDBE7684775141A81DD8F"
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
                #line 43 "..\..\..\Views\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.Selector)(target)).SelectionChanged += this.lstOptions_SelectionChanged;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 56 "..\..\..\Views\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.btnNowPlaying_Click;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 57 "..\..\..\Views\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.ShortCutButton_Click;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 58 "..\..\..\Views\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.LogButton_Click;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 61 "..\..\..\Views\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.FillDbButton_Click;
                 #line default
                 #line hidden
                break;
            case 6:
                #line 62 "..\..\..\Views\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.UpdateButton_Click;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


