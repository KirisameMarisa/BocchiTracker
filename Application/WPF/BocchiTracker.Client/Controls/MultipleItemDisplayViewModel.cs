using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BocchiTracker.Client.Controls
{
    public abstract class MultipleItemDisplayViewModel : BindableBase
    {
        public ReactiveCollection<object> Items { get; set; } = new ReactiveCollection<object>();
        public ICommand DeleteItemCommand { get; set; }

        public MultipleItemDisplayViewModel()
        {
            Items.CollectionChanged += (_, __) => TransferParameter(Items);
            DeleteItemCommand = new DelegateCommand<object>(OnDeleteItem);
        }

        public void AddItem(object inItem)
        {
            if (inItem == null)
                return;

            if (!Items.Contains(inItem))
                Items.Add(inItem);
        }

        private void OnDeleteItem(object inItem)
        {
            if (Items.Contains(inItem))
                Items.Remove(inItem);
        }

        public abstract void TransferParameter(IEnumerable<object> inItems);
    }
}
