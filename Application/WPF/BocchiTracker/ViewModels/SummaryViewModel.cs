using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ViewModels
{
    public class SummaryViewModel : BindableBase
    {
        private string _ticket_type;
        public string TicketType
        {
            get => _ticket_type;
            set { SetProperty(ref _ticket_type, value); }
        }

        private string _summary;
        public string Summary
        {
            get => _summary;
            set { SetProperty(ref _summary, value); }
        }
    }
}
