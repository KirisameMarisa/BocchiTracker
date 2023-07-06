using Prism.Ioc;
using Prism.Regions;
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
using System.Windows.Shapes;

namespace BocchiTracker.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IRegionManager regionManager)
        {
            InitializeComponent();

            regionManager.RegisterViewWithRegion("WatchesRegion",       typeof(WatchesView));
            regionManager.RegisterViewWithRegion("LabelsRegion",        typeof(LabelsView));
            regionManager.RegisterViewWithRegion("AssigneRegion",       typeof(AssigneView));
            regionManager.RegisterViewWithRegion("ClassRegion",         typeof(ClassView));
            regionManager.RegisterViewWithRegion("ConnectedToRegion",   typeof(ConnectedToView));
            regionManager.RegisterViewWithRegion("PriorityRegion",      typeof(PriorityView));
        }
    }
}
