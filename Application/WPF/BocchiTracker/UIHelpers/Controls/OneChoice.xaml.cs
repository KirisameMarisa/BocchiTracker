using System.Windows;
using System.Windows.Controls;
using BocchiTracker.UIHelpers.Controls;

namespace BocchiTracker.UIHelpers.Controls
{
    /// <summary>
    /// OneChoice.xaml の相互作用ロジック
    /// </summary>
    public partial class OneChoice : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(OneChoiceControl), typeof(OneChoice), new PropertyMetadata(null));

        public OneChoiceControl ViewModel
        {
            get { return (OneChoiceControl)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(OneChoice), new PropertyMetadata(null));

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public OneChoice()
        {
            InitializeComponent();
        }
    }
}
