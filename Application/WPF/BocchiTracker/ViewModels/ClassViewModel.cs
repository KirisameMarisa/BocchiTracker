using BocchiTracker.IssueInfoCollector;
using Prism.Ioc;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BocchiTracker.ViewModels
{
    public class ClassViewModel : SingleItemViewModel
    {
        public ClassViewModel()
        {
            HintText = "Class";

            var issue_info_bundle = (Application.Current as PrismApplication).Container.Resolve<IssueInfoBundle>();
        }
    }
}
