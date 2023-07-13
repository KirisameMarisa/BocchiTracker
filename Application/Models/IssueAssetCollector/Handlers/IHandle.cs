using BocchiTracker.ApplicationInfoCollector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.IssueAssetCollector.Handlers
{
    public interface IHandle
    {
        void Handle(int inClientID, int inPID, string inOutput);
    }
}
