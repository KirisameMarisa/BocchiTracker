using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ViewModels
{
    public class ClassViewModel : SingleItemViewModel
    {
        public ClassViewModel()
        {
            HintText = "Select Class...";

            Items.Add("1");
            Items.Add("2");
            Items.Add("3");
            Items.Add("4");
            Items.Add("5");
        }
    }
}
