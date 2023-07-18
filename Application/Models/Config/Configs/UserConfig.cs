using BocchiTracker.ServiceClientData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.Config.Configs
{
    public class UserConfig
    {
        public List<string> SelectedService { get; set; } = new List<string>();

        public TicketData DraftTicketData { get; set; } = new TicketData();

        public List<string> DraftUploadFiles { get; set; } = new List<string>();
    }
}
