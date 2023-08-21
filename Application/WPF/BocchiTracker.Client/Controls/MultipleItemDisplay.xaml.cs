using System;
using System.Collections;
using System.Collections.Generic;
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

namespace BocchiTracker.Client.Controls
{
    /// <summary>
    /// MultipleChoice.xaml の相互作用ロジック
    /// </summary>
    public partial class MultipleItemDisplay : UserControl
    {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource", typeof(IEnumerable), typeof(MultipleItemDisplay), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty DeleteItemCommandProperty =
            DependencyProperty.Register("DeleteItemCommand", typeof(ICommand), typeof(MultipleItemDisplay), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public ICommand DeleteItemCommand
        {
            get { return (ICommand)GetValue(DeleteItemCommandProperty); }
            set { SetValue(DeleteItemCommandProperty, value); }
        }

        public MultipleItemDisplay()
        {
            InitializeComponent();
        }
    }
}
