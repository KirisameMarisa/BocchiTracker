using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ViewModels
{
    public class WatchesViewModel : MultipleItemsViewModel
    {
        public WatchesViewModel()
        {
            HintText = "Add Watches...";

            Items.Add("Apple");
            Items.Add("Banana");
            Items.Add("Cherry");
            Items.Add("Kiwi");
            Items.Add("Orange");
        }
    }
}
