using BocchiTracker.ApplicationInfoCollector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.IssueAssetCollector.Handlers.Coredump
{
    public class CoredumpHandler : IHandle
    {
        public IFilenameGenerator _filenameGenerator { private set; get; }

        public CoredumpHandler(IFilenameGenerator inFilenameGenerator)
        {
            _filenameGenerator = inFilenameGenerator;
        }

        public virtual void Handle(int inClientID, int inPID, string inOutput) { }
    }
}
