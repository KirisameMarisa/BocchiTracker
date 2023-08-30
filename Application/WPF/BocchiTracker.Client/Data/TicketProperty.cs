using BocchiTracker.ApplicationInfoCollector;
using BocchiTracker.IssueInfoCollector;
using BocchiTracker.ServiceClientAdapters.Data;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using BocchiTracker.ServiceClientData;
using Prism.Events;
using System.Net.Sockets;
using System.Diagnostics;
using BocchiTracker.ModelEvent;

namespace BocchiTracker.Data
{
    public class TicketProperty
    {
        [Required(ErrorMessage = "Required")]
        public ReactiveProperty<string> Summary { get; }

        [Required(ErrorMessage = "Required")]
        public ReactiveProperty<string> TicketType { get; }

        [Required(ErrorMessage = "Required")]
        public ReactiveProperty<string> Description { get; }

        public ReactiveProperty<string> Class { get; }

        public ReactiveProperty<string> Priority { get; }

        public ReactiveProperty<UserData> Assign { get; }

        public ReactiveCollection<UserData> Watchers { get; }

        public ReactiveCollection<string> Labels { get; }

        public AppStatusBundles AppStatusBundles { get; set; }

        public TicketProperty(IEventAggregator inEventAggregator, IssueInfoBundle inIssueInfoBundle, AppStatusBundles inAppStatusBundles)
        {
            AppStatusBundles = inAppStatusBundles;

            Summary = new ReactiveProperty<string>(inIssueInfoBundle.TicketData.Summary);
            Summary.Subscribe(value => inIssueInfoBundle.TicketData.Summary = value);

            TicketType = new ReactiveProperty<string>();
            TicketType.Subscribe(value => inIssueInfoBundle.TicketData.TicketType = value);

            Description = new ReactiveProperty<string>(inIssueInfoBundle.TicketData.Description);
            Description.Subscribe(value => inIssueInfoBundle.TicketData.Description = value);

            Class = new ReactiveProperty<string>(inIssueInfoBundle.TicketData.Class);
            Class.Subscribe(value => 
            { 
                inIssueInfoBundle.TicketData.Class = value; Trace.TraceInformation(value); 
            });

            Priority = new ReactiveProperty<string>(inIssueInfoBundle.TicketData.Priority);
            Priority.Subscribe(value => inIssueInfoBundle.TicketData.Priority = value);

            Assign = new ReactiveProperty<UserData>(inIssueInfoBundle.TicketData.Assign);
            Assign.Subscribe(value => inIssueInfoBundle.TicketData.Assign = value);

            Labels = new ReactiveCollection<string>(/*inIssueInfoBundle.TicketData.Lables*/);
            Labels.CollectionChanged += (_, __) => { inIssueInfoBundle.TicketData.Labels = Labels.ToList(); };

            Watchers = new ReactiveCollection<UserData>(/*inIssueInfoBundle.TicketData.Lables*/);
            Watchers.CollectionChanged += (_, __) => { inIssueInfoBundle.TicketData.Watchers = Watchers.ToList(); };

            inEventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);
        }

        private void OnConfigReload(ConfigReloadEventParameter inParam)
        {
            if(inParam.UserConfig != null)
            {
                Summary.Value       = inParam.UserConfig.DraftTicketData.Summary;
                TicketType.Value    = inParam.UserConfig.DraftTicketData.TicketType;
                Description.Value   = inParam.UserConfig.DraftTicketData.Description;
                Class.Value         = inParam.UserConfig.DraftTicketData.Class;
                Priority.Value      = inParam.UserConfig.DraftTicketData.Priority;
                Assign.Value        = inParam.UserConfig.DraftTicketData.Assign;

                foreach (var value in inParam.UserConfig.DraftTicketData.Labels)
                {
                    if (value == null)
                        continue;
                    Labels.Add(value);
                }

                foreach (var value in inParam.UserConfig.DraftTicketData.Watchers)
                {
                    if (value == null)
                        continue;
                    Watchers.Add(value);
                }
            }
        }
    }
}
