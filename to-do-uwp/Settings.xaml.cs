using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace to_do_uwp
{
    /// <summary>
    /// A Settings Page
    /// </summary>
    public sealed partial class Settings : Page
    {
        private const string ThemeSettingKey = "AppTheme";
        private bool isThemeComboBoxSelectionIntialized = false;

        public Settings()
        {
            this.InitializeComponent();
            LoadThemeSelection();
            isThemeComboBoxSelectionIntialized = true;
        }

        private void LoadThemeSelection()
        {
            string themeTag = ApplicationData.Current.LocalSettings.Values[ThemeSettingKey] as string ?? "Default";

            foreach (ComboBoxItem item in ThemeComboBox.Items)
            {
                if (((string)item.Tag) == themeTag)
                {
                    ThemeComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private async void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isThemeComboBoxSelectionIntialized) return;

            var selectedItem = (ComboBoxItem)ThemeComboBox.SelectedItem;
            string themeTag = selectedItem.Tag.ToString();

            ApplicationData.Current.LocalSettings.Values[ThemeSettingKey] = themeTag;

            var dialog = new ContentDialog
            {
                Title = "Restart Dialog",
                Content = "The app must restart to apply the new theme. Restart now? (No data will be lost)",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No",
                RequestedTheme = this.RequestedTheme
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                await Windows.ApplicationModel.Core.CoreApplication.RequestRestartAsync(string.Empty);
            }
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void CompletedTasksButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CompletedTasks));
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(About));
        }
    }
}
