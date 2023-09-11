using BocchiTracker.Client.ViewModels.IssueListParts;
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

namespace BocchiTracker.Client.Views.IssueListParts
{
    /// <summary>
    /// IssuesView.xaml の相互作用ロジック
    /// </summary>
    public partial class IssuesView : UserControl
    {
        public static readonly DependencyProperty SearchControlVisibleProperty =
            DependencyProperty.Register("SearchControlVisible", typeof(bool), typeof(IssuesView), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSearchControlVisibleUpdate));

        public bool SearchControlVisible
        {
            get { return (bool)GetValue(SearchControlVisibleProperty); }
            set { SetValue(SearchControlVisibleProperty, value); }
        }

        public static readonly DependencyProperty IDColumnVisibleProperty =
            DependencyProperty.Register("IDColumnVisible", typeof(bool), typeof(IssuesView), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnGridViewColumnVisibleUpdate));

        public bool IDColumnVisible
        {
            get { return (bool)GetValue(IDColumnVisibleProperty); }
            set { SetValue(IDColumnVisibleProperty, value); }
        }

        public static readonly DependencyProperty JumpColumnVisibleProperty =
            DependencyProperty.Register("JumpColumnVisible", typeof(bool), typeof(IssuesView), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnGridViewColumnVisibleUpdate));

        public bool JumpColumnVisible
        {
            get { return (bool)GetValue(JumpColumnVisibleProperty); }
            set { SetValue(JumpColumnVisibleProperty, value); }
        }

        public static readonly DependencyProperty AssignColumnVisibleProperty =
            DependencyProperty.Register("AssignColumnVisible", typeof(bool), typeof(IssuesView), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnGridViewColumnVisibleUpdate));

        public bool AssignColumnVisible
        {
            get { return (bool)GetValue(AssignColumnVisibleProperty); }
            set { SetValue(AssignColumnVisibleProperty, value); }
        }

        public static readonly DependencyProperty StatusColumnVisibleProperty =
            DependencyProperty.Register("StatusColumnVisible", typeof(bool), typeof(IssuesView), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnGridViewColumnVisibleUpdate));

        public bool StatusColumnVisible
        {
            get { return (bool)GetValue(StatusColumnVisibleProperty); }
            set { SetValue(StatusColumnVisibleProperty, value); }
        }

        public static readonly DependencyProperty SummaryColumnVisibleProperty =
            DependencyProperty.Register("SummaryColumnVisible", typeof(bool), typeof(IssuesView), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnGridViewColumnVisibleUpdate));

        public bool SummaryColumnVisible
        {
            get { return (bool)GetValue(SummaryColumnVisibleProperty); }
            set { SetValue(SummaryColumnVisibleProperty, value); }
        }

        public IssuesView()
        {
            InitializeComponent();


        }

        private static void OnSearchControlVisibleUpdate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IssuesView context)
            {
                var vm = context.DataContext as IssuesViewModel;
                vm.IssueSearch.IsVisible = (bool)e.NewValue;
            }
        }

        private static void OnGridViewColumnVisibleUpdate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IssuesView context)
            {
                if (context.IssueList.View is GridView gridView)
                {
                    foreach (GridViewColumn column in gridView.Columns)
                    {
                        switch(column.Header.ToString())
                        {
                            case "ID":
                                column.Width = context.IDColumnVisible ? column.Width : 0; break;
                            case "Jump":
                                column.Width = context.JumpColumnVisible ? column.Width : 0; break;
                            case "Assign":
                                column.Width = context.AssignColumnVisible ? column.Width : 0; break;
                            case "Status":
                                column.Width = context.StatusColumnVisible ? column.Width : 0; break;
                            case "Summary":
                                column.Width = context.SummaryColumnVisible ? column.Width : 0; break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}
