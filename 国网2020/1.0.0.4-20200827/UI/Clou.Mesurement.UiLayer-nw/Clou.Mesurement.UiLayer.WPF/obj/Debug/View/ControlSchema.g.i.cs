﻿#pragma checksum "..\..\..\View\ControlSchema.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "CC5D96217D7F37B395694EF56FF45EF9"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Clou.Mesurement.UiLayer.ViewModel.Schema;
using Clou.Mesurement.UiLayer.WPF.Model;
using Clou.Mesurement.UiLayer.WPF.Schema;
using Clou.Mesurement.UiLayer.WPF.Schema.Error;
using DevComponents.WPF.Controls;
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


namespace Clou.Mesurement.UiLayer.WPF.View {
    
    
    /// <summary>
    /// ControlSchema
    /// </summary>
    public partial class ControlSchema : Clou.Mesurement.UiLayer.WPF.Model.DockControlDisposable, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 167 "..\..\..\View\ControlSchema.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox comboBoxSchemas;
        
        #line default
        #line hidden
        
        
        #line 178 "..\..\..\View\ControlSchema.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal DevComponents.WPF.Controls.AdvTree treeSchema;
        
        #line default
        #line hidden
        
        
        #line 219 "..\..\..\View\ControlSchema.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox listBoxParaValues;
        
        #line default
        #line hidden
        
        
        #line 259 "..\..\..\View\ControlSchema.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel stackPanel1;
        
        #line default
        #line hidden
        
        
        #line 282 "..\..\..\View\ControlSchema.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Clou.Mesurement.UiLayer.WPF.Schema.DataGridCheckParaValue dataGridGeneral;
        
        #line default
        #line hidden
        
        
        #line 291 "..\..\..\View\ControlSchema.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Clou.Mesurement.UiLayer.WPF.Schema.Error.ControlTreeError controlEror;
        
        #line default
        #line hidden
        
        
        #line 310 "..\..\..\View\ControlSchema.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox checkBoxError;
        
        #line default
        #line hidden
        
        
        #line 349 "..\..\..\View\ControlSchema.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox listBoxParaConfig;
        
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
            System.Uri resourceLocater = new System.Uri("/Clou.Mesurement.UiLayer.WPF;component/view/controlschema.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\View\ControlSchema.xaml"
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
            
            #line 136 "..\..\..\View\ControlSchema.xaml"
            ((System.Windows.Controls.ContextMenu)(target)).AddHandler(System.Windows.Controls.MenuItem.ClickEvent, new System.Windows.RoutedEventHandler(this.MenuItem_Click));
            
            #line default
            #line hidden
            return;
            case 2:
            this.comboBoxSchemas = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 3:
            this.treeSchema = ((DevComponents.WPF.Controls.AdvTree)(target));
            
            #line 180 "..\..\..\View\ControlSchema.xaml"
            this.treeSchema.ActiveItemChanged += new System.Windows.RoutedEventHandler(this.AdvTree_ActiveItemChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.listBoxParaValues = ((System.Windows.Controls.ListBox)(target));
            
            #line 223 "..\..\..\View\ControlSchema.xaml"
            this.listBoxParaValues.AddHandler(System.Windows.DragDrop.DragOverEvent, new System.Windows.DragEventHandler(this.OnDragOver));
            
            #line default
            #line hidden
            
            #line 224 "..\..\..\View\ControlSchema.xaml"
            this.listBoxParaValues.AddHandler(System.Windows.DragDrop.DropEvent, new System.Windows.DragEventHandler(this.OnDrop));
            
            #line default
            #line hidden
            
            #line 225 "..\..\..\View\ControlSchema.xaml"
            this.listBoxParaValues.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(this.listBoxParaValues_PreviewMouseMove);
            
            #line default
            #line hidden
            
            #line 226 "..\..\..\View\ControlSchema.xaml"
            this.listBoxParaValues.QueryContinueDrag += new System.Windows.QueryContinueDragEventHandler(this.listBoxParaValues_QueryContinueDrag);
            
            #line default
            #line hidden
            return;
            case 5:
            this.stackPanel1 = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 6:
            this.dataGridGeneral = ((Clou.Mesurement.UiLayer.WPF.Schema.DataGridCheckParaValue)(target));
            return;
            case 7:
            this.controlEror = ((Clou.Mesurement.UiLayer.WPF.Schema.Error.ControlTreeError)(target));
            return;
            case 8:
            this.checkBoxError = ((System.Windows.Controls.CheckBox)(target));
            
            #line 308 "..\..\..\View\ControlSchema.xaml"
            this.checkBoxError.Checked += new System.Windows.RoutedEventHandler(this.checkBoxError_Checked);
            
            #line default
            #line hidden
            
            #line 309 "..\..\..\View\ControlSchema.xaml"
            this.checkBoxError.Unchecked += new System.Windows.RoutedEventHandler(this.checkBoxError_Checked);
            
            #line default
            #line hidden
            return;
            case 9:
            this.listBoxParaConfig = ((System.Windows.Controls.ListBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 10:
            
            #line 389 "..\..\..\View\ControlSchema.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ButtonParaInfo_Click);
            
            #line default
            #line hidden
            break;
            case 11:
            
            #line 398 "..\..\..\View\ControlSchema.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ButtonParaInfo_Click);
            
            #line default
            #line hidden
            break;
            case 12:
            
            #line 428 "..\..\..\View\ControlSchema.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ButtonParaInfo_Click);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

