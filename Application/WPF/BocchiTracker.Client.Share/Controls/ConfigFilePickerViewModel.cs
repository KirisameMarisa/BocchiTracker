using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Prism.Commands;
using Reactive.Bindings;
using System.Reactive.Linq;

namespace BocchiTracker.Client.Share.Controls
{
    public class ConfigFilePickerViewModel : BindableBase, IDialogAware
    {
        public string Title => "Choice ProjectConfig";
        public string ProjectConfigDirectory => Path.Combine("Configs", "ProjectConfigs");

        public ReactiveCollection<string>   ItemsSource         { get; set; } = new ReactiveCollection<string>();
        public ReactiveProperty<string>     Text                { get; set; } = new ReactiveProperty<string>();
        public ReactiveProperty<string>     HintText            { get; set; } = new ReactiveProperty<string>();
        public ReactiveProperty<bool>       EnableFileCreation  { get; set; } = new ReactiveProperty<bool>(false);

        public ReactiveCommand<string>      CloseDialogCommand  { get; set; }

        public event Action<IDialogResult> RequestClose;

        public ConfigFilePickerViewModel()
        {
            CloseDialogCommand = Text
                                    .Select(text => !string.IsNullOrWhiteSpace(text))
                                    .ToReactiveCommand<string>();
            CloseDialogCommand.Subscribe(CloseDialog);
        }

        protected virtual void CloseDialog(string parameter)
        {
            var result = Path.Combine(ProjectConfigDirectory, Text.Value + ".yaml");
            RaiseRequestClose(new DialogResult(ButtonResult.OK, new DialogParameters($"Config={result}")));
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            EnableFileCreation.Value = parameters.GetValue<bool>("EnableFileCreation");
            HintText.Value = EnableFileCreation.Value
                ? "Enter a new config or choose a config to edit"
                : "Chose a config to use";

            if(Directory.Exists(ProjectConfigDirectory))
            {
                var configs = Directory.GetFiles(ProjectConfigDirectory, "*.yaml");
                foreach (var config in configs)
                {
                    ItemsSource.Add(Path.GetFileNameWithoutExtension(config));
                }
            }
        }
    }
}
