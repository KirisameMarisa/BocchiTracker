using BocchiTracker.ApplicationInfoCollector;
using BocchiTracker.ModelEventBus;
using BocchiTracker.ProcessLinkQuery.Queries;
using MediatR;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BocchiTracker.IssueAssetCollector.Handlers.Screenshot
{
    public class ReceiveScreenshotEventBusHandler : IRequestHandler<ReceiveScreenshotEvent>
    {
        public static string Output { get; set; } = string.Empty;

        public Task Handle(ReceiveScreenshotEvent request, CancellationToken cancellationToken)
        {
            var data = request.ScreenshotData;
            using var image = Image.LoadPixelData<Byte4>(data.ImageData, data.Width, data.Height);
            
            image.SaveAsPng(Output);
            return Task.CompletedTask;
        }
    }

    public class RemoteScreenshotHandler : ScreenshotHandler
    {
        private IMediator _mediator;

        public RemoteScreenshotHandler(IMediator inMediator, IFilenameGenerator inFilenameGenerator)
            : base(inFilenameGenerator)
        {
            this._mediator = inMediator;
        }

        public override void Handle(int inClientID, int inPID, IntPtr inHandle, string inOutput)
        {
            ReceiveScreenshotEventBusHandler.Output = Path.Combine(inOutput, _filename_generator.Generate() + ".png");
            _mediator.Send(new ModelEventBus.RequestQueryEvent(inClientID, QueryID.ScreenshotData));
        }
    }
}
