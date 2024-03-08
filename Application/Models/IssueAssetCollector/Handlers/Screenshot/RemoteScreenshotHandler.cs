using BocchiTracker.ApplicationInfoCollector;
using BocchiTracker.ModelEvent;
using BocchiTracker.ProcessLinkQuery.Queries;
using Prism.Events;
using SixLabors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BocchiTracker.IssueAssetCollector.Handlers.Screenshot
{
    public class RemoteScreenshotSaveProcess
    {
        public string Output { get; set; } = string.Empty;

        private IEventAggregator _mediator;

        public RemoteScreenshotSaveProcess(IEventAggregator inMediator) 
        {
            _mediator = inMediator;
            _mediator
                .GetEvent<ReceiveScreenshotEvent>()
                .Subscribe(Handle, ThreadOption.PublisherThread);
        }

        public void Handle(ReceiveScreenshotEventParameter inEvent)
        {
            var data = inEvent;
            if (data.ImageData == null)
                return;

            ImageProcessor.Instnace.Save(Output, data.ImageData, data.Width, data.Height);
        }
    }

    public class RemoteScreenshotHandler : ScreenshotHandler
    {
        private IEventAggregator _eventAggregator;
        public RemoteScreenshotSaveProcess SaveProcess { get; private set; }

        public RemoteScreenshotHandler(IEventAggregator inEventAggregator, IFilenameGenerator inFilenameGenerator)
            : base(inFilenameGenerator)
        {
            this._eventAggregator = inEventAggregator;
            this.SaveProcess = new RemoteScreenshotSaveProcess(inEventAggregator);
        }

        public override void Handle(AppStatusBundle inAppStatusBundle, int inPID, string inOutput)
        {
            this.SaveProcess.Output = Path.Combine(inOutput, _filenameGenerator.Generate(inAppStatusBundle) + ".png");

            _eventAggregator
                .GetEvent<RequestQueryEvent>()
                .Publish(new ScreenshotRequestEventParameter(inAppStatusBundle.AppBasicInfo.ClientID));
        }
    }
}
