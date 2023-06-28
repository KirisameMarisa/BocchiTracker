using BocchiTracker.ApplicationInfoCollector;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.IssueAssetCollector.Handlers
{
    internal interface IHandle
    {
        void Handle(int inClientID, int inPID, IntPtr inHandle, string inOutput);
    }
}
