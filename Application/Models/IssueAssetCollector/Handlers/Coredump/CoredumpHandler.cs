using BocchiTracker.ApplicationInfoCollector;
using MediatR;
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
        public IFilenameGenerator _filename_generator { private set; get; }

        public CoredumpHandler(IFilenameGenerator inFilenameGenerator)
        {
            _filename_generator = inFilenameGenerator;
        }

        public virtual void Handle(int inClientID, int inPID, IntPtr inHandle, string inOutput) { }
    }
}
