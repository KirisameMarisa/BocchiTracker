using Reactive.Bindings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Specialized;
using System.Reactive.Linq;

namespace BocchiTracker.Client.Controls
{
    public class Item
    {
        public ReactiveProperty<bool> IsSelected { get; set; } = new ReactiveProperty<bool>();

        public ReactiveProperty<string> Name { get; set; } = new ReactiveProperty<string>();

        public Item(string inName, Action<string> inOnSelectValueChanged) 
        {
            Name.Value = inName;
            IsSelected.Subscribe(x => { if(x)  inOnSelectValueChanged(Name.Value); });
        }
    }

    public enum ESelectType
    {
        Single,
        Multiple
    }

    public partial class PickerDialog : Window
    {
        public ObservableCollection<Item> Items { private get; set; } = new ObservableCollection<Item>();

        private ESelectType SelectType = ESelectType.Single;

        public PickerDialog(string inInitializeText, ObservableCollection<object> inSourceItems, ESelectType inSelectType = ESelectType.Single)
        {
            InitializeComponent();

            SelectType = inSelectType;

            foreach(var item in inSourceItems)
            {
                var newItem = new Item(item.ToString(), OnSelectValueChanged);
                if (item.ToString() == inInitializeText)
                    newItem.IsSelected.Value = true;
                Items.Add(newItem);
            }
            this.ListContent.ItemsSource = Items;
            this.CloseButton.Click += (object s, RoutedEventArgs e) => { this.Close(); };
        }

        public void CollectionUpdate(string inEditText, List<object> inSourceItems)
        {
            foreach (var item in Items)
            {
                if (inEditText.ToLower() == item.Name.Value.ToString().ToLower())
                {
                    item.IsSelected.Value = true;
                    this.Close();
                }
            }

            List<string> sourceItems = inSourceItems.Select(x => x.ToString()).ToList();
            List<string> items       = Items.Select(x => x.Name.Value).ToList();
            List<string> except      = items.Except(sourceItems).ToList();

            foreach (var removeItem in except)
            {
                var temp = Items.Where(x => x.Name.Value == removeItem).FirstOrDefault() ?? null;
                if (temp != null)
                    Items.Remove(temp);
            }

            foreach(var addItem in sourceItems.Except(items))
            {
                Items.Add(new Item(addItem, OnSelectValueChanged));
            }
        }

        private void OnSelectValueChanged(string inSelectName)
        {
            foreach (var item in Items)
            {
                switch (SelectType)
                {
                    case ESelectType.Single:
                        {
                            if (item.Name.Value != inSelectName)
                                item.IsSelected.Value = false;
                        }
                        break;
                    case ESelectType.Multiple:
                        break;
                }
            }
        }

        public List<string> GetSelectedItems()
        {
            return Items.Where(x => x.IsSelected.Value).Select(x => x.Name.Value).ToList();
        }
    }
}
