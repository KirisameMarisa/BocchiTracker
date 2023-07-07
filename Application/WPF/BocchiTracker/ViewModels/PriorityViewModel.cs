using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ViewModels
{
    public class PriorityViewModel : SingleItemViewModel
    {
        public PriorityViewModel()
        {
            HintText = "Priority";

            Items.Add("High");
            Items.Add("Middle");
            Items.Add("Low");
        }
    }
}
