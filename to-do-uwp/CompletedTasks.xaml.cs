using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using to_do_uwp.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
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
    /// A Completed Tasks Page
    /// </summary>
    public sealed partial class CompletedTasks : Page
    {
        public ToDoItemViewModel Item { get; set; }
        public ToDoItemsListViewModel CompletedItemsList { get; set; }

        public CompletedTasks()
        {
            this.InitializeComponent();

            Item = new ToDoItemViewModel();
            CompletedItemsList = new ToDoItemsListViewModel();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            StorageFolder current = ApplicationData.Current.LocalFolder;

            try
            {
                StorageFile completedFile = await current.GetFileAsync("completed_items.json");
                string json = await FileIO.ReadTextAsync(completedFile);
                var loadedCompletedItems = JsonConvert.DeserializeObject<ObservableCollection<ViewModels.ToDoItemViewModel>>(json);

                CompletedItemsList.Items.Clear();
                if (loadedCompletedItems != null)
                {
                    foreach (var item in loadedCompletedItems)
                    {
                        CompletedItemsList.AddItem(item);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                CompletedItemsList.Items.Clear();
            }
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Settings));
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(About));
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button.DataContext as ToDoItemViewModel;
            CompletedItemsList.DeleteItem(item);

            StorageFolder current = ApplicationData.Current.LocalFolder;

            StorageFile completedFile = await current.CreateFileAsync("completed_items.json", CreationCollisionOption.ReplaceExisting);
            await SaveToFile(completedFile, JsonConvert.SerializeObject(CompletedItemsList?.Items ?? new ObservableCollection<ViewModels.ToDoItemViewModel>()));
        }

        private async Task SaveToFile(StorageFile file, string content)
        {
            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                using (DataWriter dataWriter = new DataWriter(stream))
                {
                    dataWriter.WriteString(content);
                    await dataWriter.StoreAsync();
                }
            }
        }
    }
}
