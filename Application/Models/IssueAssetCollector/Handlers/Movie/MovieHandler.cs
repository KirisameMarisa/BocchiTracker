using BocchiTracker.ApplicationInfoCollector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.IssueAssetCollector.Handlers.Movie
{
    public class MovieHandler : IHandle
    {
        public IFilenameGenerator _filenameGenerator { private set; get; }

        public MovieHandler(IFilenameGenerator inFilenameGenerator)
        {
            _filenameGenerator = inFilenameGenerator;
        }

        public virtual void Handle(AppStatusBundle inAppStatusBundle, int inPID, string inOutput) { }
    }
}
