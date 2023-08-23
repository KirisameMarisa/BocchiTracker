using BocchiTracker.ServiceClientData;
using BocchiTracker.ServiceClientAdapters.Data;
using BocchiTracker.IssueInfoCollector.MetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BocchiTracker.ServiceClientAdapters;
using System.IO;
using System.IO.Abstractions;
using BocchiTracker.ModelEvent;
using Prism.Events;
using System.Threading;

namespace BocchiTracker.IssueInfoCollector
{
    public class IssueInfoBundle
    {
        public List<ServiceDefinitions>         PostServices            { get; set; } = new List<ServiceDefinitions>();

        public MetaListService<IdentifierData>  LabelListService        { get; } = new LabelListService();

        public MetaListService<IdentifierData>  PriorityListService     { get; } = new PriorityListService();
        
        public MetaListService<IdentifierData>  TicketTypeListService   { get; } = new TicketTypeListService();

        public MetaListService<IdentifierData>  CustomFieldsListService { get; } = new CustomFieldListService();

        public MetaListService<UserData>        UserListService         { get; } = new UserListService();

        public List<ServiceDefinitions>         IssuePostServices       { get; set; } = new List<ServiceDefinitions>();

        public TicketData                       TicketData              { get; set; } = new TicketData();

        public async Task Initialize(IDataRepository inRepository, IEventAggregator inEventAggregator)
        {
            inEventAggregator.GetEvent<ProgressingEvent>().Publish(new ProgressEventParameter { Message = "Initialize: Getting Labels" });
            await LabelListService.Load(inRepository);

            inEventAggregator.GetEvent<ProgressingEvent>().Publish(new ProgressEventParameter { Message = "Initialize: Getting Priorities" });
            await PriorityListService.Load(inRepository);

            inEventAggregator.GetEvent<ProgressingEvent>().Publish(new ProgressEventParameter { Message = "Initialize: Getting TicketTypes" });
            await TicketTypeListService.Load(inRepository);

            inEventAggregator.GetEvent<ProgressingEvent>().Publish(new ProgressEventParameter { Message = "Initialize: Getting Users" });
            await UserListService.Load(inRepository);

            inEventAggregator.GetEvent<ProgressingEvent>().Publish(new ProgressEventParameter { Message = "Initialize: Getting CustomFields" });
            await CustomFieldsListService.Load(inRepository);
        }
    }
}
