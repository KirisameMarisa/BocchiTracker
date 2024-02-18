using BocchiTracker.ServiceClientData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.Config.Configs
{
    public class UserCaptureSetting
    {
        public GameCaptureType GameCaptureType { get; set; } = GameCaptureType.NotUse;

        public bool IncludeAudio = false;

        public int RecordingFrameRate { get; set; } = 30;
        
        public int RecordingMintes { get; set; } = 3;
    }

    public class UserConfig
    {
        public UserCaptureSetting UserCaptureSetting { get; set; } = new UserCaptureSetting();

        public string? ProjectConfigFilename { get; set; }

        public bool IsOpenWebBrowser { get; set; }

        public List<string> SelectedService { get; set; } = new List<string>();

        public TicketData DraftTicketData { get; set; } = new TicketData();

        public List<string> DraftUploadFiles { get; set; } = new List<string>();
    }
}
