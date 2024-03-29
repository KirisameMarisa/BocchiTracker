﻿using BocchiTracker.ApplicationInfoCollector;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.IssueAssetCollector.Handlers.Screenshot
{
    public class ScreenshotHandler : IHandle
    {
        public IFilenameGenerator _filenameGenerator { private set; get; }

        public ScreenshotHandler(IFilenameGenerator inFilenameGenerator)
        {
            _filenameGenerator = inFilenameGenerator;
        }

        public virtual void Handle(AppStatusBundle inAppStatusBundle, int inPID, string inOutput) { }
    }
}
