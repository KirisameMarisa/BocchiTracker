using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
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
    public abstract class MultipleItemsViewModel : SingleItemViewModel
    {
        public ReactiveCollection<string> RegisteredItems { get; }

        public ICommand DeleteCommand { get; private set; }

        public MultipleItemsViewModel()
        {
            DeleteCommand = new DelegateCommand<string>(OnDeleteItem);
            RegisteredItems = new ReactiveCollection<string>();
            RegisteredItems.CollectionChanged += (_, __) => OnUpdateRegisteredItems();
        }

        private void AddItem(string inItem)
        {
            if (string.IsNullOrEmpty(inItem))
                return;

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
            AddItem(inItem);
        }

        protected virtual void OnUpdateRegisteredItems() { }
    }
}
