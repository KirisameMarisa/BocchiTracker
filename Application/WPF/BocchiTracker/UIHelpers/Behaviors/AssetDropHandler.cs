using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BocchiTracker.IssueAssetCollector;
using Microsoft.Xaml.Behaviors;
using Prism.Ioc;
using Prism.Unity;

namespace BocchiTracker.UIHelpers.Behaviors
{
    public class AssetDropHandler : Behavior<UIElement>
    {
        public static readonly DependencyProperty FilesDroppedCommandProperty =
            DependencyProperty.Register(nameof(FilesDroppedCommand), typeof(ICommand), typeof(AssetDropHandler));

        public ICommand FilesDroppedCommand
        {
            get => (ICommand)GetValue(FilesDroppedCommandProperty);
            set => SetValue(FilesDroppedCommandProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewDragOver += OnDragOver;
            AssociatedObject.Drop += OnDrop;
        }

        private void OnDragOver(object inSender, DragEventArgs ioEvent)
        {
            if (ioEvent.Data.GetDataPresent(DataFormats.FileDrop))
            {
                ioEvent.Effects = DragDropEffects.Copy;
            }
            else
            {
                ioEvent.Effects = DragDropEffects.None;
            }
            ioEvent.Handled = true;
        }

        private void OnDrop(object inSender, DragEventArgs ioEvent)
        {
            if (ioEvent.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])ioEvent.Data.GetData(DataFormats.FileDrop);
                FilesDroppedCommand?.Execute(files);
            }
        }
    }
}
