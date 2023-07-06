using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace BocchiTracker.Behaviors
{
    internal class AssetDropHandler : Behavior<UIElement>
    {
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
                //!< Event Hanlder?
            }
        }
    }
}
