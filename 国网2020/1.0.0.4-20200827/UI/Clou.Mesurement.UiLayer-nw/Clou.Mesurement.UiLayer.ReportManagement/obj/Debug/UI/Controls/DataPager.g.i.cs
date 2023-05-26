﻿#pragma checksum "..\..\..\..\UI\Controls\DataPager.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "DDD38F9ABD8BE9C4A3D60CD23CCC86B7"
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


namespace SmartReportPrint.UI.Controls {
    
    
    /// <summary>
    /// DataPager
    /// </summary>
    public partial class DataPager : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 7 "..\..\..\..\UI\Controls\DataPager.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal SmartReportPrint.UI.Controls.DataPager dp;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\..\..\UI\Controls\DataPager.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cboPageSize;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\..\..\UI\Controls\DataPager.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnFirst;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\..\..\UI\Controls\DataPager.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnPrev;
        
        #line default
        #line hidden
        
        
        #line 77 "..\..\..\..\UI\Controls\DataPager.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbPageIndex;
        
        #line default
        #line hidden
        
        
        #line 86 "..\..\..\..\UI\Controls\DataPager.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnNext;
        
        #line default
        #line hidden
        
        
        #line 93 "..\..\..\..\UI\Controls\DataPager.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnLast;
        
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
            System.Uri resourceLocater = new System.Uri("/Clou.Mesurement.UiLayer.ReportManagement;component/ui/controls/datapager.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\UI\Controls\DataPager.xaml"
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
            this.dp = ((SmartReportPrint.UI.Controls.DataPager)(target));
            
            #line 9 "..\..\..\..\UI\Controls\DataPager.xaml"
            this.dp.Loaded += new System.Windows.RoutedEventHandler(this.DataPager_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.cboPageSize = ((System.Windows.Controls.ComboBox)(target));
            
            #line 52 "..\..\..\..\UI\Controls\DataPager.xaml"
            this.cboPageSize.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.cbpPageSize_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.btnFirst = ((System.Windows.Controls.Button)(target));
            
            #line 53 "..\..\..\..\UI\Controls\DataPager.xaml"
            this.btnFirst.Click += new System.Windows.RoutedEventHandler(this.btnFirst_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btnPrev = ((System.Windows.Controls.Button)(target));
            
            #line 61 "..\..\..\..\UI\Controls\DataPager.xaml"
            this.btnPrev.Click += new System.Windows.RoutedEventHandler(this.btnPrev_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.tbPageIndex = ((System.Windows.Controls.TextBox)(target));
            
            #line 78 "..\..\..\..\UI\Controls\DataPager.xaml"
            this.tbPageIndex.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.tbPageIndex_PreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 79 "..\..\..\..\UI\Controls\DataPager.xaml"
            this.tbPageIndex.LostFocus += new System.Windows.RoutedEventHandler(this.tbPageIndex_LostFocus);
            
            #line default
            #line hidden
            return;
            case 6:
            this.btnNext = ((System.Windows.Controls.Button)(target));
            
            #line 86 "..\..\..\..\UI\Controls\DataPager.xaml"
            this.btnNext.Click += new System.Windows.RoutedEventHandler(this.btnNext_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.btnLast = ((System.Windows.Controls.Button)(target));
            
            #line 93 "..\..\..\..\UI\Controls\DataPager.xaml"
            this.btnLast.Click += new System.Windows.RoutedEventHandler(this.btnLast_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 100 "..\..\..\..\UI\Controls\DataPager.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.btnRefresh_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

