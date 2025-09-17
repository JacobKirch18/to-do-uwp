using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

        private static ObservableCollection<Models.task> tasks; 

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            StorageFolder current = ApplicationData.Current.LocalFolder;
            try
            {
                StorageFile file = await current.GetFileAsync("tasks.json");
                string json = await FileIO.ReadTextAsync(file);
                tasks = JsonConvert.DeserializeObject<ObservableCollection<Models.task>>(json);
                if (tasks == null)
                {
                    tasks = new ObservableCollection<Models.task>();
                }
                //TasksListView.ItemsSource = tasks;
            }
            catch (FileNotFoundException)
            {
                tasks = new ObservableCollection<Models.task>();
                //TasksListView.ItemsSource = tasks;
            }
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            StorageFolder current = ApplicationData.Current.LocalFolder;
            StorageFile file = await current.CreateFileAsync("tasks.json", CreationCollisionOption.ReplaceExisting);
            await SaveToFile(file, JsonConvert.SerializeObject(tasks));
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

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
