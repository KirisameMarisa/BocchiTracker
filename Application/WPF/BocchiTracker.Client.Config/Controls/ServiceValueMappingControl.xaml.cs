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

namespace BocchiTracker.Client.Config.Controls
{
    public partial class ServiceValueMappingControl : UserControl
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<ServiceValueMapping>), typeof(ServiceValueMappingControl), new PropertyMetadata(null));

        public IEnumerable<ServiceValueMapping> ItemsSource
        {
            get { return (IEnumerable<ServiceValueMapping>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty AddItemCommandProperty =
            DependencyProperty.Register("AddItemCommand", typeof(ICommand), typeof(ServiceValueMappingControl), new PropertyMetadata(null));

        public ICommand AddItemCommand
        {
            get { return (ICommand)GetValue(AddItemCommandProperty); }
            set { SetValue(AddItemCommandProperty, value); }
        }

        public static readonly DependencyProperty RemoveItemCommandProperty =
            DependencyProperty.Register("RemoveItemCommand", typeof(ICommand), typeof(ServiceValueMappingControl), new PropertyMetadata(null));

        public ICommand RemoveItemCommand
        {
            get { return (ICommand)GetValue(RemoveItemCommandProperty); }
            set { SetValue(RemoveItemCommandProperty, value); }
        }

        public static readonly DependencyProperty DefinitionHintProperty =
            DependencyProperty.Register("DefinitionHint", typeof(string), typeof(ServiceValueMappingControl), new PropertyMetadata(null));

        public string DefinitionHint
        {
            get { return (string)GetValue(DefinitionHintProperty); }
            set { SetValue(DefinitionHintProperty, value); }
        }

        public ServiceValueMappingControl()
        {
            InitializeComponent();
        }
    }
}
