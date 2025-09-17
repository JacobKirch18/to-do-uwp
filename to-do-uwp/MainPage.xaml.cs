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

        public MainPage()
        {
            this.InitializeComponent();

            Item = new ToDoItemViewModel();
            ItemsList = new ToDoItemsListViewModel(); 
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            StorageFolder current = ApplicationData.Current.LocalFolder;
            try
            {
                StorageFile file = await current.GetFileAsync("items.json");
                string json = await FileIO.ReadTextAsync(file);
                ItemsList.Items = JsonConvert.DeserializeObject<ObservableCollection<ViewModels.ToDoItemViewModel>>(json);
                if (ItemsList.Items == null)
                {
                    ItemsList.Items = new ObservableCollection<ViewModels.ToDoItemViewModel>();
                }
                //TasksListView.ItemsSource = tasks;
            }
            catch (FileNotFoundException)
            {
                ItemsList.Items = new ObservableCollection<ViewModels.ToDoItemViewModel>();
                //TasksListView.ItemsSource = tasks;
            }
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            StorageFolder current = ApplicationData.Current.LocalFolder;
            StorageFile file = await current.CreateFileAsync("items.json", CreationCollisionOption.ReplaceExisting);
            await SaveToFile(file, JsonConvert.SerializeObject(ItemsList.Items));
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
            ContentDialog addTaskDialog = new ContentDialog
            {
                Title = "Add New Task",
                PrimaryButtonText = "Add",
                CloseButtonText = "Cancel"
            };

            StackPanel panel = new StackPanel();

            TextBox taskNameTextBox = new TextBox
            {
                PlaceholderText = "Task Name"
            };
            panel.Children.Add(taskNameTextBox);

            TextBox dueDateTextBox = new TextBox
            {
                PlaceholderText = "Due Date (e.g., 12-31-2025, Dec. 31, or Wednesday)"
            };
            panel.Children.Add(dueDateTextBox);

            addTaskDialog.Content = panel;

            ContentDialogResult result = await addTaskDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                string taskName = taskNameTextBox.Text.Trim();
                string dueDate = dueDateTextBox.Text.Trim();

                if (!string.IsNullOrEmpty(taskName) && !string.IsNullOrEmpty(dueDate))
                {
                    ViewModels.ToDoItemViewModel newTask = new ViewModels.ToDoItemViewModel
                    {
                        Name = taskName,
                        DueDate = dueDate
                    };

                    ItemsList.Items.Add(newTask);
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

        private void CompleteButton_Click(Object sender, RoutedEventArgs e)
        {

        }
    }
}
