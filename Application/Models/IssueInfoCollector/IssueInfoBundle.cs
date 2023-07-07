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

namespace BocchiTracker.IssueInfoCollector
{
    public class IssueInfoBundle
    {
        public MetaListService<IdentifierData>  LabelListService        { get; } = new LabelListService();

        public MetaListService<IdentifierData>  PriorityListService     { get; } = new PriorityListService();
        
        public MetaListService<IdentifierData>  TicketTypeListService   { get; } = new TicketTypeListService();
        
        public MetaListService<UserData>        UserListService         { get; } = new UserListService();

        public List<IssueServiceDefinitions>    IssuePostServices       { get; set; } = new List<IssueServiceDefinitions>();

        public TicketData                       TicketData              { get; set; } = new TicketData();

        public bool                             IsInitializeed          { get; private set; } = false;

        public async Task Initialize(IDataRepository inRepository)
        {
            await LabelListService.Load(inRepository);
            await PriorityListService.Load(inRepository);
            await TicketTypeListService.Load(inRepository);
            await UserListService.Load(inRepository);

            IsInitializeed = true;
        }
    }
}
