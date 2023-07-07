using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;

namespace BocchiTracker.ViewModels
{
    public class MultipleItemsViewModel : SingleItemViewModel
    {
        private ObservableCollection<string> _registeredItems = new ObservableCollection<string>();
        public ObservableCollection<string> RegisteredItems
        {
            get => _registeredItems;
            set { SetProperty(ref _registeredItems, value); }
        }

        public ICommand DeleteCommand { get; private set; }

        public MultipleItemsViewModel()
        {
            DeleteCommand = new DelegateCommand<string>(OnDeleteItem);
        }

        private void AddItem(string inItem)
        {
            if (!RegisteredItems.Contains(inItem))
                RegisteredItems.Add(inItem);
        }

        private void OnDeleteItem(string inItem)
        {
            if (RegisteredItems.Contains(inItem))
                RegisteredItems.Remove(inItem);
        }

        protected override void OnReturnKey(string inItem)
        {
            AddItem(inItem);
        }

        protected override void OnSetSelectedItem(string inItem)
        {
            base.OnSetSelectedItem(inItem);
            AddItem(inItem);
        }
    }
}
