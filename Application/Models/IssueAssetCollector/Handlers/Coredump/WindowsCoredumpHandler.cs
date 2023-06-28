﻿#if WINDOWS
using BocchiTracker.ApplicationInfoCollector;
using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.IssueAssetCollector.Handlers.Coredump
{
    public class WindowsCoredumpHandler : CoredumpHandler
    {
        private string _proc_dump_app;

        public WindowsCoredumpHandler(IFilenameGenerator inFilenameGenerator, string inProcDumpPath)
            : base(inFilenameGenerator)
        {
            _proc_dump_app = inProcDumpPath;
        }

        public override void Handle(int inClientID, int inPID, IntPtr inHandle, string inOutput)
        {
            if(!File.Exists(_proc_dump_app))
            {
                //!< needs to set procdump app path.
                return;
            }

            using (Process proc = new Process())
            {
                string output = Path.Combine(inOutput, _filename_generator.Generate() + ".dmp");
                proc.StartInfo = new ProcessStartInfo
                {
                    FileName = _proc_dump_app,
                    UseShellExecute = false,
                    Verb = "RunAs",
                    Arguments = $"-ma {inPID} {output}"
                };
                proc.Start();
                proc.WaitForExit();
            }
        }
    }
}
#endif