using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ViewModels
{
    public class LabelsViewModel : MultipleItemsViewModel
    {
        public LabelsViewModel() 
        {
            HintText = "Labels";

            Items.Add("Bug");
            Items.Add("Question");
            Items.Add("Feature");
        }
    }
}
