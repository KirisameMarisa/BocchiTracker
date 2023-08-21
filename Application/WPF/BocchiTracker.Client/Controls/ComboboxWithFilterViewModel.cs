using BocchiTracker.ModelEvent;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Reactive.Bindings;
using System.Diagnostics;
using System.Windows.Documents;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace BocchiTracker.Client.Controls
{
    public class PickerDialogHandler
    {
        private PickerDialog _pickerWindow;

        private ObservableCollection<object> _filteredItems;
        private ESelectType _selectType;
        private bool _isPickerWindowOpened = false;
        private Action< IEnumerable<string> > _onCloseEvent;

        public PickerDialogHandler(ObservableCollection<object> inFilteredItems, ESelectType inSelectType, Action<IEnumerable<string>> inOnCloseEvent)
        {
            _filteredItems = inFilteredItems;
            _selectType = inSelectType;
            _onCloseEvent = inOnCloseEvent;
        }

        public void ShowPicker(string inInitializeText, double inLeft, double inTop)
        {
            if (!_isPickerWindowOpened)
            {
                OpenPickerWindow(inInitializeText, inLeft, inTop);
            }
            _pickerWindow.Topmost = true;
        }

        public void UpdatePickerLocation(double inLeft, double inTop)
        {
            if (_pickerWindow != null)
            {
                _pickerWindow.Left = inLeft;
                _pickerWindow.Top = inTop;
            }
        }

        public void UpdatePickerActive(bool inState)
        {
            if (_pickerWindow != null)
                _pickerWindow.Topmost = inState;
        }

        public void UpdateCollection(string inEditText, List<object> inSourceItems)
        {
            if (_pickerWindow != null)
                _pickerWindow.CollectionUpdate(inEditText, inSourceItems);
        }

        private void OpenPickerWindow(string inInitializeText, double inLeft, double inTop)
        {
            _isPickerWindowOpened = true;
            _pickerWindow = new PickerDialog(inInitializeText, _filteredItems, _selectType);
            _pickerWindow.Closed += (sender, e) => { _isPickerWindowOpened = false; };
            _pickerWindow.Left = inLeft;
            _pickerWindow.Top = inTop;
            _pickerWindow.Height = 400;
            _pickerWindow.Width = 300;
            _pickerWindow.Closed += (object sender, EventArgs e) => _onCloseEvent(_pickerWindow.GetSelectedItems());
            _pickerWindow.Show();
        }
    }

    public abstract class ComboboxWithFilterViewModel : BindableBase
    {
        public ObservableCollection<object> Items { get; set; } = new ObservableCollection<object>();
        private ObservableCollection<object> _filteredItems = new ObservableCollection<object>();
        private PickerDialogHandler _pickerHandler;

        public ReactiveProperty<double> PickerLocationX { get; } = new ReactiveProperty<double>();
        public ReactiveProperty<double> PickerLocationY { get; } = new ReactiveProperty<double>();
        public ReactiveProperty<string> EditText { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> HintText { get; } = new ReactiveProperty<string>();

        public ICommand ShowPickerCommand { get; set; }

        private ESelectType _selectType;

        public ComboboxWithFilterViewModel(IEventAggregator inEventAggregator, string inHintText, ESelectType inSelectType = ESelectType.Single)
        {
            _selectType = inSelectType;
            _pickerHandler = new PickerDialogHandler(_filteredItems, inSelectType, OnPickerWindowClosed);

            ShowPickerCommand = new DelegateCommand(() => _pickerHandler.ShowPicker(EditText.Value, PickerLocationX.Value, PickerLocationY.Value));
            inEventAggregator.GetEvent<WindowMouseMoveEvent>().Subscribe(() => _pickerHandler.UpdatePickerLocation(PickerLocationX.Value, PickerLocationY.Value));
            inEventAggregator.GetEvent<WindowActiveChangedEvent>().Subscribe(_pickerHandler.UpdatePickerActive);

            EditText.Subscribe(OnEditTextUpdate);
            HintText.Value = inHintText;
        }

        public void Initialize(IEnumerable<object> inItems)
        {
            foreach(var item in inItems)
            {
                Items.Add(item);
                _filteredItems.Add(item);
            }
        }

        private void OnEditTextUpdate(string inText)
        {
            _filteredItems.Clear();
            foreach (var item in Items)
            {
                if (string.IsNullOrEmpty(inText) || item.ToString().ToLower().Contains(inText.ToLower()))
                {
                    _filteredItems.Add(item);
                }
            }
            _pickerHandler.UpdateCollection(inText, _filteredItems.ToList());
        }

        public void OnPickerWindowClosed(IEnumerable<string> inSelectedItems)
        {
            List<object> result = new List<object>();

            foreach(var selectedItem in inSelectedItems) 
            {
                foreach(var item in Items)
                {
                    if(selectedItem == item.ToString())
                    {
                        result.Add(item);
                        break;
                    }
                }
            }

            switch(_selectType)
            {
                case ESelectType.Single:
                    {
                        if (result.Count > 0)
                            EditText.Value = result[0].ToString();
                    }
                    break;
                case ESelectType.Multiple:
                    {
                        EditText.Value = string.Empty;
                    }
                    break;
            }

            ProcessSelectedItems(result);
        }

        public abstract void ProcessSelectedItems(IEnumerable<object> inItems);
    }
}