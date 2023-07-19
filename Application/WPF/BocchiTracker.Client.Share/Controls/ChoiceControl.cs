using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.TinyLinq;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows.Input;

namespace BocchiTracker.Client.Share.Controls
{
    public abstract class ComboboxBase
    {
        protected CompositeDisposable _disposable = new CompositeDisposable();

        public ICommand ReturnKeyCommand { get; private set; }

        public ICommand ShowItemCommand { get; private set; }

        public ReactiveCollection<object> Items { get; } = new ReactiveCollection<object>();

        public ReactiveCollection<object> FilteredItems { get; } = new ReactiveCollection<object>();

        public ReactiveProperty<bool> IsOpen { get; } = new ReactiveProperty<bool>(false);

        public ReactiveProperty<string> HintText { get; } = new ReactiveProperty<string>();

        public ReactiveProperty<object> Selected { get; } = new ReactiveProperty<object>();

        public ReactiveProperty<bool> IsEditable { get; } = new ReactiveProperty<bool>();

        public ReactiveProperty<string> EditText { get; } = new ReactiveProperty<string>();

        public ComboboxBase()
        {
            EditText
                .Where(value => !string.IsNullOrEmpty(value))
                .Subscribe(value => FilterItems(value))
                .AddTo(_disposable);

            Selected
                .Where(value => value != null)
                .Subscribe(value => OnSelected(value))
                .AddTo(_disposable);

            ShowItemCommand = new DelegateCommand(OnShowItems);
            ReturnKeyCommand = new DelegateCommand<object>(OnReturnKey);
        }

        public void OnShowItems()
        {
            IsOpen.Value = true;
            FilteredItems.ClearOnScheduler();
            foreach (var item in Items)
                FilteredItems.AddOnScheduler(item);
        }

        public void AddItem(object inItem)
        {
            Items.Add(inItem);
        }

        public void RemoveItem(object inItem)
        {
            Items.Remove(inItem);
        }

        public void FilterItems(string inFilterItem)
        {
            for (int i = FilteredItems.Count - 1; i >= 0; i--)
            {
                var item = FilteredItems[i];
                var itemText = item.ToString();
                if (itemText != null && !itemText.ToLower().Contains(inFilterItem.ToLower()))
                {
                    FilteredItems.RemoveAt(i);
                }
            }

            foreach (var item in Items)
            {
                var itemText = item.ToString();
                if (itemText != null && itemText.ToLower().Contains(inFilterItem.ToLower()))
                {
                    if (!FilteredItems.Contains(item))
                    {
                        FilteredItems.Add(item);
                    }
                }
            }
        }

        public abstract void OnSelected(object inItem);

        public abstract void OnReturnKey(object inItem);
    }

    public abstract class OneChoiceControl : ComboboxBase
    {
        public OneChoiceControl(string inHintText)
        {
            HintText.Value = inHintText;
        }

        public override void OnReturnKey(object inItem) { }
    }

    public abstract class MultipleChoicesControl : ComboboxBase
    {
        public ReactiveCollection<object> RegisteredItems { get; } = new ReactiveCollection<object>();

        public ICommand DeleteRegisterdItemCommand { get; private set; }

        public MultipleChoicesControl(string inHintText)
        {
            HintText.Value = inHintText;

            RegisteredItems.CollectionChanged += (_, __) => OnUpdateRegisteredItems();

            DeleteRegisterdItemCommand = new DelegateCommand<object>(OnDeleteRegisterdItem);
        }

        private void RegisteredItem(object inItem)
        {
            if (inItem == null)
                return;

            if (!RegisteredItems.Contains(inItem))
                RegisteredItems.Add(inItem);
        }

        private void OnDeleteRegisterdItem(object inItem)
        {
            if (RegisteredItems.Contains(inItem))
                RegisteredItems.Remove(inItem);
        }

        public override void OnReturnKey(object inItem)
        {
            RegisteredItem(inItem);
        }

        public override void OnSelected(object inItem)
        {
            RegisteredItem(inItem);
        }

        public abstract void OnUpdateRegisteredItems();
    }
}
