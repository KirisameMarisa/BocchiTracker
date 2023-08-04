using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BocchiTracker.Client.Config.Controls
{
    public partial class SetURL : UserControl
    {
        public static readonly DependencyProperty URLTextProperty =
            DependencyProperty.Register("URLText", typeof(string), typeof(SetURL), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string URLText
        {
            get { return (string)GetValue(URLTextProperty); }
            set { SetValue(URLTextProperty, value); }
        }

        public static readonly DependencyProperty URLHintProperty =
            DependencyProperty.Register("URLHint", typeof(string), typeof(SetURL), new PropertyMetadata(null));

        public string URLHint
        {
            get { return (string)GetValue(URLHintProperty); }
            set { SetValue(URLHintProperty, value); }
        }

        public static readonly DependencyProperty ProxyURLTextProperty =
            DependencyProperty.Register("ProxyURLText", typeof(string), typeof(SetURL), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string ProxyURLText
        {
            get { return (string)GetValue(ProxyURLTextProperty); }
            set { SetValue(ProxyURLTextProperty, value); }
        }

        public static readonly DependencyProperty ProxyURLHintProperty =
            DependencyProperty.Register("ProxyURLHint", typeof(string), typeof(SetURL), new PropertyMetadata(null));

        public string ProxyURLHint
        {
            get { return (string)GetValue(ProxyURLHintProperty); }
            set { SetValue(ProxyURLHintProperty, value); }
        }

        public static readonly DependencyProperty EnableInputProxyProperty =
            DependencyProperty.Register("EnableInputProxy", typeof(bool), typeof(SetURL), new PropertyMetadata(false));

        public bool EnableInputProxy
        {
            get { return (bool)GetValue(EnableInputProxyProperty); }
            set { SetValue(EnableInputProxyProperty, value); }
        }

        public SetURL()
        {
            InitializeComponent();
        }
    }
}
