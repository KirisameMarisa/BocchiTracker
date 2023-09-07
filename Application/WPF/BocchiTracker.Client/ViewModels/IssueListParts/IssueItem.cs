using BocchiTracker.ApplicationInfoCollector;
using BocchiTracker.CrossServiceReporter;
using BocchiTracker.ModelEvent;
using BocchiTracker.ServiceClientData;
using Prism.Commands;
using Prism.Events;
using System;
using System.Windows.Input;

namespace BocchiTracker.Client.ViewModels.IssueListParts
{
    public class IssueItem
    {
        public Func<AppStatusBundle> CurrentConnectionStatus { get; set; }
        public Func<ServiceDefinitions> CurrentService { get; set; }

        public ICommand HyperlinkCommand { get; private set; }

        public ICommand JumpPlayerLocationCommand { get; private set; }

        public TicketData TicketData { get; set; }

        private readonly IEventAggregator _eventAggregator;
        private readonly IIssueOpener _issueOpener;

        public IssueItem(TicketData inTicketData, IEventAggregator inEventAggregator, IIssueOpener inIssueOpener)
        {
            TicketData = inTicketData;

            HyperlinkCommand = new DelegateCommand(OnOpenInBrowser);
            JumpPlayerLocationCommand = new DelegateCommand(OnJumpPlayerLocation, TicketData.CanJumpPlayer);

            _eventAggregator = inEventAggregator;
            _issueOpener = inIssueOpener;
        }

        private void OnJumpPlayerLocation()
        {
            var connectApplication = CurrentConnectionStatus?.Invoke();
            if (connectApplication == null)
                return;

            string stage = string.Empty;
            float x = float.NaN, y = float.NaN, z = float.NaN;
            if (TicketData.CustomFields.TryGetValue("PlayerPosition.x", out float outX))
                x = outX;

            if (TicketData.CustomFields.TryGetValue("PlayerPosition.y", out float outY))
                y = outY;

            if (TicketData.CustomFields.TryGetValue("PlayerPosition.z", out float outZ))
                z = outZ;

            if (TicketData.CustomFields.TryGetValue("PlayerPosition.stage", out string outstage))
                stage = outstage;

            _eventAggregator.GetEvent<RequestQueryEvent>().Publish(new JumpRequestEventParameter(connectApplication.AppBasicInfo.ClientID, x, y, z, stage));
        }

        public void OnOpenInBrowser()
        {
            if (CurrentService == null)
                return;

            ServiceDefinitions service = CurrentService.Invoke();
            _issueOpener.Open(service, TicketData.Id);
        }
    }
}
