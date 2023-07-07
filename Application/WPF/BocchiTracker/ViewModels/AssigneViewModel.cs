using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ViewModels
{
    public class AssigneViewModel : SingleItemViewModel
    {
        public AssigneViewModel()
        {
            HintText = "Assigne";

            Items.Add("Apple");
            Items.Add("Banana");
            Items.Add("Cherry");
            Items.Add("Kiwi");
            Items.Add("Orange");
        }
    }
}
