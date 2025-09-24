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
    /// The Main Page of the application
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ToDoItemViewModel Item { get; set; }
        public ToDoItemsListViewModel ItemsList { get; set; }
        public ToDoItemsListViewModel CompletedItemsList { get; set; }

        public MainPage()
        {
            this.InitializeComponent();

            Item = new ToDoItemViewModel();
            ItemsList = new ToDoItemsListViewModel();
            CompletedItemsList = new ToDoItemsListViewModel();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            StorageFolder current = ApplicationData.Current.LocalFolder;
            try
            {
                StorageFile toDoFile = await current.GetFileAsync("items.json");
                string json = await FileIO.ReadTextAsync(toDoFile);
                var loadedItems = JsonConvert.DeserializeObject<ObservableCollection<ViewModels.ToDoItemViewModel>>(json);

                ItemsList.Items.Clear();
                if (loadedItems != null)
                {
                    foreach (var item in loadedItems)
                    {
                        ItemsList.AddItem(item);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                ItemsList.Items.Clear();
            }

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

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            StorageFolder current = ApplicationData.Current.LocalFolder;

            StorageFile toDoFile = await current.CreateFileAsync("items.json", CreationCollisionOption.ReplaceExisting);
            await SaveToFile(toDoFile, JsonConvert.SerializeObject(ItemsList.Items));
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

        private void CompletedTasksButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CompletedTasks));
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Settings));
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(About));
        }

        private async void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog addItemDialog = new ContentDialog
            {
                Title = "Add New Task",
                PrimaryButtonText = "Add",
                CloseButtonText = "Cancel"
            };

            StackPanel panel = new StackPanel();

            TextBox itemNameTextBox = new TextBox
            {
                PlaceholderText = "Task Name"
            };
            panel.Children.Add(itemNameTextBox);

            TextBox dueDateTextBox = new TextBox
            {
                PlaceholderText = "Due Date (e.g., 12-31-2025, Dec. 31, or Wednesday)"
            };
            panel.Children.Add(dueDateTextBox);

            addItemDialog.Content = panel;

            ContentDialogResult result = await addItemDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                string itemName = itemNameTextBox.Text.Trim();
                string dueDate = dueDateTextBox.Text.Trim();

                if (!string.IsNullOrEmpty(itemName) && !string.IsNullOrEmpty(dueDate))
                {
                    ViewModels.ToDoItemViewModel newItem = new ViewModels.ToDoItemViewModel
                    {
                        Name = itemName,
                        DueDate = dueDate
                    };

                    ItemsList.AddItem(newItem);
                }
                else
                {
                    ContentDialog errorDialog = new ContentDialog
                    {
                        Title = "Error",
                        Content = "Both fields are required.",
                        CloseButtonText = "OK"
                    };
                    await errorDialog.ShowAsync();
                }
            }
        }

        private async void CompleteButton_Click(Object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button.DataContext as ToDoItemViewModel;
            ItemsList.DeleteItem(item);
            CompletedItemsList.AddItem(item);

            StorageFolder current = ApplicationData.Current.LocalFolder;
            StorageFile completedFile = await current.CreateFileAsync("completed_items.json", CreationCollisionOption.ReplaceExisting);
            await SaveToFile(completedFile, JsonConvert.SerializeObject(CompletedItemsList.Items));
        }
    }
}
