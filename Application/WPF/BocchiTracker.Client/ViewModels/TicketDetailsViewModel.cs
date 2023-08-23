using BocchiTracker.Data;
using BocchiTracker.IssueInfoCollector;
using BocchiTracker.ServiceClientAdapters.Data;
using Prism.Events;
using Prism.Ioc;
using Prism.Unity;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Unity;
using Prism.Mvvm;
using BocchiTracker.ApplicationInfoCollector;
using BocchiTracker.ServiceClientData;
using BocchiTracker.Client.Controls;
using BocchiTracker.ModelEvent;

namespace BocchiTracker.Client.ViewModels
{
    public class TicketLabels : BindableBase
    {
        public class DisplayViewModel : MultipleItemDisplayViewModel
        {
            public TicketProperty _ticketProperty { get; set; }

            public DisplayViewModel(TicketProperty inTicketProperty)
            {
                _ticketProperty = inTicketProperty;
            }

            public override void TransferParameter(IEnumerable<object> inItems)
            {
                _ticketProperty.Labels.Clear();
                foreach (var item in inItems)
                    _ticketProperty.Labels.Add(item as string);
            }
        }

        public class ComboboxViewModel : ComboboxWithFilterViewModel
        {
            private MultipleItemDisplayViewModel _displayViewModel;

            public ComboboxViewModel(IEventAggregator inEventAggregator, MultipleItemDisplayViewModel inDisplayViewModel) 
                : base(inEventAggregator, "Labels", ESelectType.Multiple)
            {
                _displayViewModel = inDisplayViewModel;
            }

            public override void ProcessSelectedItems(IEnumerable<object> inItems)
            {
                foreach(var item in inItems)
                    _displayViewModel.AddItem(item);
            }
        }

        public DisplayViewModel Display { get; set; }
        public ComboboxViewModel Combobox { get; set; }

        public TicketLabels(IEventAggregator inEventAggregator, TicketProperty ticketProperty) 
        {
            Display = new DisplayViewModel(ticketProperty);
            Combobox = new ComboboxViewModel(inEventAggregator, Display);
        }

        public void Clear()
        {
            Combobox.EditText.Value = string.Empty;
            Display.Items.Clear();
            Display._ticketProperty.Watchers.Clear();
        }
    }

    public class TicketWatchers : BindableBase
    {
        public class DisplayViewModel : MultipleItemDisplayViewModel
        {
            public TicketProperty _ticketProperty { get; set; }

            public DisplayViewModel(TicketProperty inTicketProperty)
            {
                _ticketProperty = inTicketProperty;
            }

            public override void TransferParameter(IEnumerable<object> inItems)
            {
                _ticketProperty.Watchers.Clear();
                foreach (var item in inItems)
                    _ticketProperty.Watchers.Add(item as UserData);
            }
        }

        public class ComboboxViewModel : ComboboxWithFilterViewModel
        {
            private MultipleItemDisplayViewModel _displayViewModel;

            public ComboboxViewModel(IEventAggregator inEventAggregator, MultipleItemDisplayViewModel inDisplayViewModel)
                : base(inEventAggregator, "Watchers", ESelectType.Multiple)
            {
                _displayViewModel = inDisplayViewModel;
            }

            public override void ProcessSelectedItems(IEnumerable<object> inItems)
            {
                foreach (var item in inItems)
                    _displayViewModel.AddItem(item);
            }
        }

        public DisplayViewModel Display { get; set; }
        public ComboboxViewModel Combobox { get; set; }

        public TicketWatchers(IEventAggregator inEventAggregator, TicketProperty ticketProperty) 
        {
            Display = new DisplayViewModel(ticketProperty);
            Combobox = new ComboboxViewModel(inEventAggregator, Display);
        }

        public void Clear()
        {
            Combobox.EditText.Value = string.Empty;
            Display.Items.Clear();
            Display._ticketProperty.Watchers.Clear();
        }
    }

    public class ConnectTo : BindableBase
    {
        public ReactiveCollection<object> Items { get; } = new ReactiveCollection<object>();
        public ReactiveProperty<object> Selected { get; set; } = new ReactiveProperty<object>();
        public ReactiveProperty<string> HintText { get; set; } = new ReactiveProperty<string>("Connected To...");

        public TicketProperty _ticketProperty { get; set; }

        public ConnectTo(TicketProperty ticketProperty)
        { 
            _ticketProperty = ticketProperty;
            _ticketProperty.AppStatusBundles.AppConnected       = this.Connected;
            _ticketProperty.AppStatusBundles.AppDisconnected    = this.Disconnected;

            Selected.Subscribe(x => { _ticketProperty.AppStatusBundles.TrackerApplication = x as AppStatusBundle; });
        }

        public void Connected(AppStatusBundle inAppStatusBundle)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var foundItems = Items.Where(x => x == inAppStatusBundle).ToList();
                if (foundItems.Count() == 0)
                {
                    Items.Add(inAppStatusBundle);
                }
            });
        }

        public void Disconnected(AppStatusBundle inAppStatusBundle)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var foundItems = Items.Where(x => x == inAppStatusBundle).ToList();
                foreach (var removeItem in foundItems)
                {
                    Items.Remove(removeItem);
                }

                if((Selected.Value as AppStatusBundle) == inAppStatusBundle)
                {
                    Selected.Value = null;
                }
            });
        }
    }

    public class TicketAssign : ComboboxWithFilterViewModel
    {
        public TicketProperty _ticketProperty { get; set; }

        public TicketAssign(IEventAggregator inEventAggregator, TicketProperty ticketProperty) 
            : base(inEventAggregator, "Assign") { _ticketProperty = ticketProperty; }

        public override void ProcessSelectedItems(IEnumerable<object> inItems)
        {
            if(inItems.Count() > 0)
                _ticketProperty.Assign.Value = inItems.ElementAt(0) as UserData;
        }

        public void Clear()
        {
            EditText.Value = string.Empty;
            _ticketProperty.Assign.Value = UserData.sUnknown;
        }
    }

    public class TicketClass : BindableBase
    {
        public ReactiveCollection<object> Items { get; } = new ReactiveCollection<object>();
        public ReactiveProperty<object> Selected { get; set; } = new ReactiveProperty<object>();
        public ReactiveProperty<string> HintText { get; set; } = new ReactiveProperty<string>("Class");

        public TicketProperty _ticketProperty { get; set; }

        public TicketClass(TicketProperty ticketProperty) 
        {
            _ticketProperty = ticketProperty;
            Selected.Subscribe(x => { _ticketProperty.Class.Value = x?.ToString(); });
        }

        public void Clear()
        {
            Selected.Value = null;
            _ticketProperty.Class.Value = string.Empty;
        }
    }

    public class TicketPriority : BindableBase
    {
        public ReactiveCollection<object> Items { get; } = new ReactiveCollection<object>();
        public ReactiveProperty<object> Selected { get; set; } = new ReactiveProperty<object>();
        public ReactiveProperty<string> HintText { get; set; } = new ReactiveProperty<string>("Priority");

        public TicketProperty _ticketProperty { get; set; }

        public TicketPriority(TicketProperty ticketProperty) 
        { 
            _ticketProperty = ticketProperty;
            Selected.Subscribe(x => { _ticketProperty.Priority.Value = x?.ToString(); });
        }

        public void Clear()
        {
            Selected.Value = null;
            _ticketProperty.Priority.Value = string.Empty;
        }
    }

    public class TicketDetailsViewModel : BindableBase
    {
        public TicketProperty TicketProperty { get; set; }

        public TicketClass TicketClass { get; set; }

        public TicketPriority TicketPriority { get; set; }

        public TicketAssign TicketAssign { get; set; }

        public TicketLabels TicketLabels { get; set; }

        public TicketWatchers TicketWatchers { get; set; }

        public ConnectTo ConnectTo { get; set; }

        public TicketDetailsViewModel(IEventAggregator inEventAggregator, TicketProperty inTicketProperty) 
        {
            TicketProperty = inTicketProperty;
            TicketClass = new TicketClass(inTicketProperty);
            TicketPriority = new TicketPriority(inTicketProperty);
            TicketAssign = new TicketAssign(inEventAggregator, inTicketProperty);
            TicketLabels = new TicketLabels(inEventAggregator, inTicketProperty);
            TicketWatchers = new TicketWatchers(inEventAggregator, inTicketProperty);
            ConnectTo = new ConnectTo(inTicketProperty);

            inEventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);

            inEventAggregator
                .GetEvent<PopulateUIEvent>()
                .Subscribe(OnPopulateUI, ThreadOption.UIThread);

            inEventAggregator
                .GetEvent<IssueSubmittedEvent>()
                .Subscribe(OnIssueSubmittedEvent, ThreadOption.UIThread);
        }


        private void OnConfigReload(ConfigReloadEventParameter inParam)
        {
            foreach (var item in inParam.ProjectConfig.IssueGrades)
                TicketClass.Items.Add(item);

            foreach (var item in inParam.ProjectConfig.Priorities)
                TicketPriority.Items.Add(item);

            var issue_info_bundle = (Application.Current as PrismApplication).Container.Resolve<IssueInfoBundle>();
            TicketLabels.Combobox.Initialize(issue_info_bundle.LabelListService.GetUnifiedData().Select(x => x.Name));
            TicketAssign.Initialize(issue_info_bundle.UserListService.GetUnifiedData());
            TicketWatchers.Combobox.Initialize(issue_info_bundle.UserListService.GetUnifiedData());
        }

        private void OnIssueSubmittedEvent(IssueSubmittedEventParameter inParam)
        {
            TicketClass.Clear();
            TicketPriority.Clear();
            TicketAssign.Clear();
            TicketLabels.Clear();
            TicketWatchers.Clear();
        }

        private void OnPopulateUI()
        {
            TicketClass.Selected.Value      = TicketProperty.Class.Value;
            TicketPriority.Selected.Value   = TicketProperty.Priority.Value;
            TicketAssign.EditText.Value     = TicketProperty.Assign.Value.Name;

            {
                var temporaryList = new List<string>(TicketProperty.Labels);
                foreach (var item in temporaryList)
                    TicketLabels.Display.AddItem(item);
            }

            {
                var temporaryList = new List<UserData>(TicketProperty.Watchers);
                foreach (var item in temporaryList)
                    TicketWatchers.Display.AddItem(item.Name);
            }
        }
    }
}
