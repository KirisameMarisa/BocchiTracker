using BocchiTracker.IssueInfoCollector;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ViewModels
{
    public class DescriptionViewModel : BindableBase
    {
        public ReactiveProperty<string> Description { get; }

        public DescriptionViewModel(IssueInfoBundle inIssueInfoBundle)
        {
            Description = new ReactiveProperty<string>(inIssueInfoBundle.TicketData.Description);
            Description.Subscribe(value => inIssueInfoBundle.TicketData.Description = value);
        }
    }
}
