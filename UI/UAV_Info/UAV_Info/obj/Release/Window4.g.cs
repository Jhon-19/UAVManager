#pragma checksum "..\..\Window4.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "FFBC0DA0C8732D5D3F2ADE34AD6FB8AE9E8A3CCA82E6C9E09686C1F5ABC1D577"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Converters;
using MaterialDesignThemes.Wpf.Transitions;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
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
using UAV_Info;


namespace UAV_Info {
    
    
    /// <summary>
    /// Window4
    /// </summary>
    public partial class Window4 : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 71 "..\..\Window4.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button W1_Button;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\Window4.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button W2_Button;
        
        #line default
        #line hidden
        
        
        #line 87 "..\..\Window4.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button W3_Button;
        
        #line default
        #line hidden
        
        
        #line 102 "..\..\Window4.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button W5_Button;
        
        #line default
        #line hidden
        
        
        #line 136 "..\..\Window4.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox FilledComboBoxEnabledCheckBox2;
        
        #line default
        #line hidden
        
        
        #line 142 "..\..\Window4.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox FilledComboBox2;
        
        #line default
        #line hidden
        
        
        #line 151 "..\..\Window4.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button UAV_Setting;
        
        #line default
        #line hidden
        
        
        #line 159 "..\..\Window4.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal UAV_Info.HUD hud;
        
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
            System.Uri resourceLocater = new System.Uri("/UAV_Info;component/window4.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\Window4.xaml"
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
            
            #line 10 "..\..\Window4.xaml"
            ((UAV_Info.Window4)(target)).MouseMove += new System.Windows.Input.MouseEventHandler(this.Windows_Move);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 22 "..\..\Window4.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Min_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 26 "..\..\Window4.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Change_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 30 "..\..\Window4.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Close_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.W1_Button = ((System.Windows.Controls.Button)(target));
            
            #line 73 "..\..\Window4.xaml"
            this.W1_Button.Click += new System.Windows.RoutedEventHandler(this.W1_Button_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.W2_Button = ((System.Windows.Controls.Button)(target));
            
            #line 81 "..\..\Window4.xaml"
            this.W2_Button.Click += new System.Windows.RoutedEventHandler(this.W2_Button_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.W3_Button = ((System.Windows.Controls.Button)(target));
            
            #line 89 "..\..\Window4.xaml"
            this.W3_Button.Click += new System.Windows.RoutedEventHandler(this.W3_Button_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.W5_Button = ((System.Windows.Controls.Button)(target));
            
            #line 104 "..\..\Window4.xaml"
            this.W5_Button.Click += new System.Windows.RoutedEventHandler(this.W5_Button_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.FilledComboBoxEnabledCheckBox2 = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 10:
            this.FilledComboBox2 = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 11:
            this.UAV_Setting = ((System.Windows.Controls.Button)(target));
            
            #line 153 "..\..\Window4.xaml"
            this.UAV_Setting.Click += new System.Windows.RoutedEventHandler(this.UAV_Setting_Clicked);
            
            #line default
            #line hidden
            return;
            case 12:
            this.hud = ((UAV_Info.HUD)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

