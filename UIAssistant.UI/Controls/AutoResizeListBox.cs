using System;
using System.Windows;
using System.Windows.Controls;

namespace UIAssistant.UI.Controls
{
    public class AutoResizeListBox : DependencyObject
    {
        public static int GetItemsCountPerPage(DependencyObject obj)
        {
            return (int)obj.GetValue(ItemsCountPerPageProperty);
        }

        public static void SetItemsCountPerPage(DependencyObject obj, int value)
        {
            obj.SetValue(ItemsCountPerPageProperty, value);
        }

        public static readonly DependencyProperty ItemsCountPerPageProperty =
            DependencyProperty.RegisterAttached("ItemsCountPerPage", typeof(int), typeof(AutoResizeListBox), new PropertyMetadata(0, OnItemsCountPerPageChanged));

        private static void OnItemsCountPerPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listBox = d as ListBox;
            listBox.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler((lb, arg) => UpdateSize(listBox)));
        }

        static double _itemHeight = double.NaN;
        static void UpdateSize(ListBox listBox)
        {
            if (listBox.Items.Count == 0)
            {
                listBox.Height = listBox.Padding.Top + listBox.Padding.Bottom + listBox.BorderThickness.Top + listBox.BorderThickness.Bottom + 2;
                return;
            }

            _itemHeight = GetItemHeight(listBox);
            if (double.IsNaN(_itemHeight))
            {
                return;
            }
            var maxCount = GetItemsCountPerPage(listBox);
            var newHeight = Math.Min(maxCount, listBox.Items.Count) * _itemHeight;
            newHeight += listBox.Padding.Top + listBox.Padding.Bottom + listBox.BorderThickness.Top + listBox.BorderThickness.Bottom + 2;

            if (listBox.ActualHeight != newHeight)
                listBox.Height = newHeight;
            return;
        }

        static double GetItemHeight(ListBox listBox)
        {
            var gen = listBox.ItemContainerGenerator;
            var item = gen.ContainerFromIndex(0) as FrameworkElement;
            if (item == null)
            {
                return _itemHeight;
            }
            return item.ActualHeight;
        }
    }
}
