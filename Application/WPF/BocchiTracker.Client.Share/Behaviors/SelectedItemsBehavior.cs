using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Xaml.Behaviors;
using System.Windows.Controls.Primitives;

namespace BocchiTracker.Client.Share.Behaviors
{
    [TypeConstraint(typeof(Selector))]
    public class SelectedItemsBehavior : Behavior<Selector>
    {
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems",
                                        typeof(IEnumerable),
                                        typeof(SelectedItemsBehavior),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public IEnumerable SelectedItems
        {
            get => (IEnumerable)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += SelectionChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SelectionChanged -= SelectionChanged;
            base.OnDetaching();
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            dynamic selector = AssociatedObject;
            SelectedItems = Enumerable.ToArray(selector.SelectedItems);
        }
    }
}
