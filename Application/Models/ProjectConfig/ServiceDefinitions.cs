using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.Config
{
    public enum IssueServiceDefinitions
    {
        JIRA,
        Redmine,
        Slack,
        Github,
        Glitlab,
        Discord,
    }

    public enum UploadServiceDefinitions
    {
        JIRA,
        Redmine,
        Slack,
        Github,
        Glitlab,
        Discord,
        Explorer,
        Dropbox,
    }
}
