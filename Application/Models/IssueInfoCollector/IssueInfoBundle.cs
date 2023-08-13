using BocchiTracker.Config;
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
using BocchiTracker.ServiceClientData;

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

        public async Task Initialize(IDataRepository inRepository)
        {
            await LabelListService.Load(inRepository);
            await PriorityListService.Load(inRepository);
            await TicketTypeListService.Load(inRepository);
            await UserListService.Load(inRepository);
            await CustomFieldsListService.Load(inRepository);
        }
    }
}
