using BocchiTracker.Client.Config.Controls;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BocchiTracker.Client.Config.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public ObservableCollection<ServiceValueMapping> PriorityValueMappings { get; set; } = new ObservableCollection<ServiceValueMapping>();

        public ICommand AddItemCommand { get; private set; }

        public ICommand RemoveItemCommand { get; private set; }

        public MainWindowViewModel()
        {
            AddItemCommand = new DelegateCommand<string>(OnAddItem);
            RemoveItemCommand = new DelegateCommand<string>(OnRemoveItem);
        }

        public void OnAddItem(string inValue)
        {
            PriorityValueMappings.Add(new ServiceValueMapping { Definition = inValue });
        }

        public void OnRemoveItem(string inValue)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var itemToRemove = PriorityValueMappings.FirstOrDefault(kvp => kvp.Definition == inValue);
                if (itemToRemove != null)
                {
                    PriorityValueMappings.Remove(itemToRemove);
                }
            });
        }
    }
}
