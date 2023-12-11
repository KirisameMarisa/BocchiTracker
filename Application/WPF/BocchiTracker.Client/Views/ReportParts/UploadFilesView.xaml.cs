using BocchiTracker.Client.ViewModels.ReportParts;
using System;
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

namespace BocchiTracker.Client.Views.ReportParts
{
    /// <summary>
    /// UploadFilesView.xaml の相互作用ロジック
    /// </summary>
    public partial class UploadFilesView : UserControl
    {
        private UploadItem _curSelectItem = null;
        private Grid _curSelectElement = null;

        public UploadFilesView()
        {
            InitializeComponent();
        }

        private void OnItemClicked(object sender, MouseButtonEventArgs e)
        {
            if (sender is Grid element && element.DataContext is UploadItem selectedItem)
            {
                if (_curSelectElement != null)
                {
                    var visual = _curSelectElement;
                    if (visual != null)
                        visual.Background = element.Background;
                }

                if(_curSelectItem != null) 
                {
                    var item = _curSelectItem;
                    if (item != null)
                        item.IsSelected = false;
                }

                element.Background = Brushes.LightGray;
                _curSelectElement = element;

                selectedItem.IsSelected = true;
                _curSelectItem = selectedItem;
            }
        }
    }
}
