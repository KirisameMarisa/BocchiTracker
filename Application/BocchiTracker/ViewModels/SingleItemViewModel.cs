using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BocchiTracker.ViewModels
{
    public abstract class SingleItemViewModel : BindableBase
    {
        public ICommand ShowCommand { get; private set; }

        private string _hintText;
        public string HintText
        {
            get => _hintText;
            set { SetProperty(ref _hintText, value); }
        }

        private bool _isOpen = false;
        public bool IsOpen
        {
            get => _isOpen;
            set { SetProperty(ref _isOpen, value); }
        }

        private string _editText = string.Empty;
        public string EditText
        {
            get => _editText;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    SetProperty(ref _editText, value);
                    FilterItems(_editText);
                }
            }
        }

        private string _selectedItem;
        public string SelectedItem
        {
            get => _selectedItem;
            set { if (!string.IsNullOrEmpty(value)) OnSetSelectedItem(value); }
        }

        private ObservableCollection<string> _items = new ObservableCollection<string>();
        public ObservableCollection<string> Items
        {
            get => _items;
            set { SetProperty(ref _items, value); }
        }

        private ObservableCollection<string> _filteredItems = new ObservableCollection<string>();
        public ObservableCollection<string> FilteredItems
        {
            get => _filteredItems;
            set { SetProperty(ref _filteredItems, value); }
        }

        private void FilterItems(string inItem)
        {
            FilteredItems.Clear();
            foreach (var item in Items)
            {
                var itemText = item.ToString();
                if (itemText != null && itemText.ToLower().Contains(inItem.ToLower()))
                {
                    FilteredItems.Add(item);
                }
            }
        }

        public SingleItemViewModel()
        {
            ShowCommand = new DelegateCommand(() => 
            {
                IsOpen = true;
                FilteredItems.Clear();
                foreach (var item in Items)
                {
                    FilteredItems.Add(item);
                }
            });
        }

        protected virtual void OnSetSelectedItem(string inItem)
        {
            SetProperty(ref _selectedItem, inItem);
            Keyboard.ClearFocus();
        }
    }
}
