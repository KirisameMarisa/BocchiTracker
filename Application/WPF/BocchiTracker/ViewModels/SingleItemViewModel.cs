using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.TinyLinq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BocchiTracker.ViewModels
{
    public abstract class SingleItemViewModel : BindableBase
    {
        private CompositeDisposable _disposable = new CompositeDisposable();
        
        public ICommand RetrunKeyCommand { get; private set; }
        public ICommand ShowCommand { get; private set; }

        public ReactiveProperty<string> HintText { get; }

        public ReactiveProperty<bool> IsOpen { get; }

        public ReactiveProperty<string> EditText { get; }

        public ReactiveProperty<string> SelectedItem { get; }

        public ReactiveCollection<string> Items { get; }

        public ReactiveCollection<string> FilteredItems { get; }

        public SingleItemViewModel()
        {
            HintText        = new ReactiveProperty<string>().AddTo(_disposable);
            IsOpen          = new ReactiveProperty<bool>().AddTo(_disposable);
            SelectedItem    = new ReactiveProperty<string>().AddTo(_disposable);
            EditText        = new ReactiveProperty<string>().AddTo(_disposable);
            Items           = new ReactiveCollection<string>();
            FilteredItems   = new ReactiveCollection<string>();

            EditText
                .Where(value => !string.IsNullOrEmpty(value))
                .Subscribe(value => FilterItems(value))
                .AddTo(_disposable);

            SelectedItem
                .Where(value => !string.IsNullOrEmpty(value))
                .Subscribe(value =>
                {
                    OnSetSelectedItem(value);
                    Keyboard.ClearFocus();
                })
                .AddTo(_disposable);

            ShowCommand = new DelegateCommand(() =>
            {
                IsOpen.Value = true;
                FilteredItems.ClearOnScheduler();
                foreach (var item in Items)
                {
                    FilteredItems.AddOnScheduler(item);
                }
            });

            RetrunKeyCommand = new DelegateCommand<string>(OnReturnKey);
        }

        private void FilterItems(string inItem)
        {
            for (int i = FilteredItems.Count - 1; i >= 0; i--)
            {
                var item = FilteredItems[i];
                var itemText = item.ToString();
                if (itemText != null && !itemText.ToLower().Contains(inItem.ToLower()))
                {
                    FilteredItems.RemoveAt(i);
                }
            }

            foreach (var item in Items)
            {
                var itemText = item.ToString();
                if (itemText != null && itemText.ToLower().Contains(inItem.ToLower()))
                {
                    if (!FilteredItems.Contains(item))
                    {
                        FilteredItems.Add(item);
                    }
                }
            }
        }

        protected virtual void OnReturnKey(string inItem) {}

        protected virtual void OnSetSelectedItem(string inItem) {}
    }
}
