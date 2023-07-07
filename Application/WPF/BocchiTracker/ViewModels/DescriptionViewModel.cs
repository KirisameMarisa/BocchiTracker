using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ViewModels
{
    public class DescriptionViewModel : BindableBase
    {
        private string _description;
        public string Description
        {
            get => _description;
            set { SetProperty(ref _description, value); }
        }
    }
}
