﻿using BocchiTracker.ProcessLinkQuery.Queries;
using Google.FlatBuffers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ProcessLink.ProcessData
{
    public class ProcessDataPlayerPosition : IProcessData
    {
        private readonly PlayerPosition _data;

        public ProcessDataPlayerPosition(PlayerPosition inData)
        {
            _data = inData;
        }

        public async Task Process(IMediator inMediator, int inClientID)
        {
            var status = new Dictionary<string, dynamic>();
            status["X"] = _data.X;
            status["Y"] = _data.Y;
            status["Z"] = _data.Z;
            status["Stage"] = _data.Stage;

            await inMediator.Send(new ModelEventBus.AppStatusQueryEvent(new ModelEventBus.AppStatus
            {
                QueryID = (int)QueryID.PlayerPosition,
                ClientID = inClientID,
                Status = status
            }));
        }
    }
}
