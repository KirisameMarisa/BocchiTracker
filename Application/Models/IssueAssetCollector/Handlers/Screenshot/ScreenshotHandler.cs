using BocchiTracker.ApplicationInfoCollector;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace BocchiTracker.IssueAssetCollector.Handlers.Screenshot
{
    public class ScreenshotHandler : IHandle
    {
        public IFilenameGenerator _filename_generator { private set; get; }

        public ScreenshotHandler(IFilenameGenerator inFilenameGenerator)
        {
            _filename_generator = inFilenameGenerator;
        }

        public virtual void Handle(int inClientID, int inPID, IntPtr inHandle, string inOutput) { }
    }
}
