using System.Windows;
using System.Windows.Controls;

namespace BocchiTracker.CustomControl
{
    /// <summary>
    /// MultipleChoice.xaml の相互作用ロジック
    /// </summary>
    public partial class MultipleChoice : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(MultipleChoicesControl), typeof(MultipleChoice), new PropertyMetadata(null));

        public MultipleChoicesControl ViewModel
        {
            get { return (MultipleChoicesControl)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(MultipleChoice), new PropertyMetadata(null));

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public MultipleChoice()
        {
            InitializeComponent();
        }
    }
}
