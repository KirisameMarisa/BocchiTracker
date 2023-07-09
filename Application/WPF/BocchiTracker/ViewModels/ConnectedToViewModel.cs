using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ViewModels
{
    public class ConnectedToViewModel : SingleItemViewModel
    {
        public ConnectedToViewModel()
        {
            HintText.Value = "Connected To";
            Items.Add("Connected To Windows11 SampleGame, CL:12252");
        }
    }
}
