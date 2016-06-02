using System;
using System.Collections.Generic;
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

namespace UIAssistant.Plugin.KeybindsManiacs
{
    /// <summary>
    /// Settings.xaml の相互作用ロジック
    /// </summary>
    public partial class Settings : UserControl
    {
        public Settings()
        {
            InitializeComponent();

        }

        private void addKeybind_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (DataContext as SettingsViewModel);
            viewModel.AddNewKeybind();
            SelectItemAt(listBox.Items.Count - 1);
        }

        private void removeKeybind_Click(object sender, RoutedEventArgs e)
        {
            System.ComponentModel.IEditableCollectionView items = listBox.Items;
            if (items.CanRemove)
            {
                var selectedIndex = listBox.SelectedIndex;
                items.Remove(listBox.SelectedItem);
                if (selectedIndex > 0)
                {
                    listBox.SelectedIndex = selectedIndex - 1;
                    SelectItemAt(listBox.SelectedIndex);
                }
            }
        }

        private void addMode_Click(object sender, RoutedEventArgs e)
        {
            //newMode.Text
        }

        private void removeMode_Click(object sender, RoutedEventArgs e)
        {
            if (mode.Text == Consts.DefaultMode)
            {
                UIAssistantAPI.NotifyInfoMessage("Cannot remove it", $"{Consts.DefaultMode} mode is not removable");
                return;
            }
        }

        private void mode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var modeName = mode.SelectedItem as string;
            var viewModel = (DataContext as SettingsViewModel);
            viewModel.SwitchMode(modeName);
            if (listBox != null && viewModel.Current != null)
            {
                listBox.ItemsSource = viewModel.Current.Keybinds;
                enableDefaultKeybinds.IsChecked = viewModel.Current.IsEnabledWindowsKeybinds;
            }
        }

        private void defaultMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewModel = (DataContext as SettingsViewModel);
            viewModel.Settings.Mode = defaultMode.SelectedValue as string;
        }

        private void listBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.None)
            {
                if (listBox.SelectedIndex < listBox.Items.Count - 1)
                {
                    listBox.SelectedIndex = listBox.SelectedIndex + 1;
                }
                SelectItemAt(listBox.SelectedIndex);
            }
        }

        private void SelectItemAt(int index)
        {
            listBox.Focus();
            listBox.ScrollIntoView(listBox.Items[index]);
            var listBoxItem = (ListBoxItem)listBox.ItemContainerGenerator.ContainerFromIndex(index);
            listBoxItem?.Focus();
        }
    }
}
