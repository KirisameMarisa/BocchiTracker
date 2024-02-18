using BocchiTracker.ApplicationInfoCollector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.IssueAssetCollector.Handlers.Movie
{
    public class OBSStudioHandler : MovieHandler
    {
        public OBSStudioHandler(IFilenameGenerator inFilenameGenerator) : base(inFilenameGenerator)
        {

        }

        public override void Handle(AppStatusBundle inAppStatusBundle, int inPID, string inOutput)
        {

        }
    }
}
