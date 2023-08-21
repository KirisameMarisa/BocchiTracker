using BocchiTracker.Client.ViewModels;
using BocchiTracker.IssueInfoCollector;
using BocchiTracker.ModelEvent;
using Google.FlatBuffers;
using MaterialDesignThemes.Wpf;
using Prism.Events;
using Prism.Ioc;
using Prism.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
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
using System.Xml.Linq;

namespace BocchiTracker.Client.Controls
{
    /// <summary>
    /// ComboboxWithFilter.xaml の相互作用ロジック
    /// </summary>
    public partial class ComboboxWithFilter : UserControl
    {
        public static readonly DependencyProperty EditTextProperty =
            DependencyProperty.Register("EditText", typeof(string), typeof(ComboboxWithFilter), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string EditText
        {
            get { return (string)GetValue(EditTextProperty); }
            set { SetValue(EditTextProperty, value); }
        }

        public static readonly DependencyProperty HintTextProperty =
            DependencyProperty.Register("HintText", typeof(string), typeof(ComboboxWithFilter), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string HintText
        {
            get { return (string)GetValue(HintTextProperty); }
            set { SetValue(HintTextProperty, value); }
        }

        public static readonly DependencyProperty ShowPickerCommandProperty =
            DependencyProperty.Register("ShowPickerCommand", typeof(ICommand), typeof(ComboboxWithFilter), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public ICommand ShowPickerCommand
        {
            get { return (ICommand)GetValue(ShowPickerCommandProperty); }
            set { SetValue(ShowPickerCommandProperty, value); }
        }

        public static readonly DependencyProperty PickerLocationXProperty =
            DependencyProperty.Register("PickerLocationX", typeof(double), typeof(ComboboxWithFilter), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public double PickerLocationX
        {
            get { return (double)GetValue(PickerLocationXProperty); }
            set { SetValue(PickerLocationXProperty, value); }
        }

        public static readonly DependencyProperty PickerLocationYProperty =
            DependencyProperty.Register("PickerLocationY", typeof(double), typeof(ComboboxWithFilter), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public double PickerLocationY
        {
            get { return (double)GetValue(PickerLocationYProperty); }
            set { SetValue(PickerLocationYProperty, value); }
        }

        public ComboboxWithFilter()
        {
            InitializeComponent();

            Filter.MouseMove += (object s, MouseEventArgs e) => 
            {
                int cOffsetX = 12;
                Point screenPosition = Filter.PointToScreen(new Point(0, 0));

                PickerLocationX = screenPosition.X - cOffsetX;
                PickerLocationY = screenPosition.Y + Filter.RenderSize.Height;
            };
        }
    }
}
