﻿#pragma checksum "..\..\..\Pages\ShowLog.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C06DCE77F508FDDD4820B117B55CAB77"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.18444
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using ControlCenter.UI.Controls;
using ControlCenter.UI.Converter;
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


namespace ControlCenter.UI.Pages {
    
    
    /// <summary>
    /// ShowLog
    /// </summary>
    public partial class ShowLog : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 44 "..\..\..\Pages\ShowLog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox listBoxLogSource;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\..\Pages\ShowLog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox listBoxType;
        
        #line default
        #line hidden
        
        
        #line 70 "..\..\..\Pages\ShowLog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid dataGridLog;
        
        #line default
        #line hidden
        
        
        #line 106 "..\..\..\Pages\ShowLog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal ControlCenter.UI.Controls.DataPager dataPagerLog;
        
        #line default
        #line hidden
        
        
        #line 124 "..\..\..\Pages\ShowLog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textBoxKeyWord;
        
        #line default
        #line hidden
        
        
        #line 141 "..\..\..\Pages\ShowLog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textBoxEquipment;
        
        #line default
        #line hidden
        
        
        #line 158 "..\..\..\Pages\ShowLog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textBoxTask;
        
        #line default
        #line hidden
        
        
        #line 173 "..\..\..\Pages\ShowLog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker datePickerStart;
        
        #line default
        #line hidden
        
        
        #line 183 "..\..\..\Pages\ShowLog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker datePickerEnd;
        
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
            System.Uri resourceLocater = new System.Uri("/ControlCenter.UI;component/pages/showlog.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Pages\ShowLog.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
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
            
            #line 37 "..\..\..\Pages\ShowLog.xaml"
            ((System.Windows.Controls.Grid)(target)).AddHandler(System.Windows.Controls.Primitives.ButtonBase.ClickEvent, new System.Windows.RoutedEventHandler(this.ButtonClick));
            
            #line default
            #line hidden
            
            #line 38 "..\..\..\Pages\ShowLog.xaml"
            ((System.Windows.Controls.Grid)(target)).AddHandler(System.Windows.Controls.Primitives.Selector.SelectionChangedEvent, new System.Windows.Controls.SelectionChangedEventHandler(this.SearchConditionChanged));
            
            #line default
            #line hidden
            return;
            case 2:
            this.listBoxLogSource = ((System.Windows.Controls.ListBox)(target));
            return;
            case 3:
            this.listBoxType = ((System.Windows.Controls.ListBox)(target));
            return;
            case 4:
            this.dataGridLog = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 5:
            this.dataPagerLog = ((ControlCenter.UI.Controls.DataPager)(target));
            return;
            case 6:
            this.textBoxKeyWord = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.textBoxEquipment = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            this.textBoxTask = ((System.Windows.Controls.TextBox)(target));
            return;
            case 9:
            this.datePickerStart = ((System.Windows.Controls.DatePicker)(target));
            return;
            case 10:
            this.datePickerEnd = ((System.Windows.Controls.DatePicker)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

