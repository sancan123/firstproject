﻿#pragma checksum "..\..\..\..\UI\Pages\DataBaseDisplay.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "CA3428501085063EE0F2BA2A9B571B10"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using SmartReportPrint.UI.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace SmartReportPrint.UI.Pages {
    
    
    /// <summary>
    /// DataBaseDisplay
    /// </summary>
    public partial class DataBaseDisplay : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 34 "..\..\..\..\UI\Pages\DataBaseDisplay.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox listBoxDisplayType;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\..\..\UI\Pages\DataBaseDisplay.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox comboBoxTableName;
        
        #line default
        #line hidden
        
        
        #line 57 "..\..\..\..\UI\Pages\DataBaseDisplay.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox listBoxColumns;
        
        #line default
        #line hidden
        
        
        #line 112 "..\..\..\..\UI\Pages\DataBaseDisplay.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal SmartReportPrint.UI.Controls.FKModelListUI fkModelListUI;
        
        #line default
        #line hidden
        
        
        #line 121 "..\..\..\..\UI\Pages\DataBaseDisplay.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock textBlockSave;
        
        #line default
        #line hidden
        
        
        #line 140 "..\..\..\..\UI\Pages\DataBaseDisplay.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox listBoxLeft;
        
        #line default
        #line hidden
        
        
        #line 153 "..\..\..\..\UI\Pages\DataBaseDisplay.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox listBoxDisplay;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Clou.Mesurement.UiLayer.ReportManagement;component/ui/pages/databasedisplay.xaml" +
                    "", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\UI\Pages\DataBaseDisplay.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 23 "..\..\..\..\UI\Pages\DataBaseDisplay.xaml"
            ((System.Windows.Controls.ContextMenu)(target)).AddHandler(System.Windows.Controls.MenuItem.ClickEvent, new System.Windows.RoutedEventHandler(this.MenuItemClick));
            
            #line default
            #line hidden
            return;
            case 2:
            this.listBoxDisplayType = ((System.Windows.Controls.ListBox)(target));
            
            #line 35 "..\..\..\..\UI\Pages\DataBaseDisplay.xaml"
            this.listBoxDisplayType.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.listBoxDisplayType_SelectionChanged_1);
            
            #line default
            #line hidden
            return;
            case 3:
            this.comboBoxTableName = ((System.Windows.Controls.ComboBox)(target));
            
            #line 48 "..\..\..\..\UI\Pages\DataBaseDisplay.xaml"
            this.comboBoxTableName.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.selectedTableChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.listBoxColumns = ((System.Windows.Controls.ListBox)(target));
            return;
            case 5:
            this.fkModelListUI = ((SmartReportPrint.UI.Controls.FKModelListUI)(target));
            return;
            case 6:
            
            #line 120 "..\..\..\..\UI\Pages\DataBaseDisplay.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click_1);
            
            #line default
            #line hidden
            return;
            case 7:
            this.textBlockSave = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            this.listBoxLeft = ((System.Windows.Controls.ListBox)(target));
            return;
            case 9:
            this.listBoxDisplay = ((System.Windows.Controls.ListBox)(target));
            return;
            case 10:
            
            #line 170 "..\..\..\..\UI\Pages\DataBaseDisplay.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.buttonSelect_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            
            #line 179 "..\..\..\..\UI\Pages\DataBaseDisplay.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.buttonRemove_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

