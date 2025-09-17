using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace to_do_uwp.ViewModels
{
    public class ToDoItemsListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Models.ToDoItemsList toDoItemsList;

        public ObservableCollection<ToDoItemViewModel> Items {  get; set; }

        public string Name
        {
            get => toDoItemsList.Name;
            set
            {
                toDoItemsList.Name = value;
                NotifyPropertyChanged();
            }
        }

        public ToDoItemsListViewModel()
        {
            this.toDoItemsList = new Models.ToDoItemsList();

            Items = new ObservableCollection<ToDoItemViewModel>();

            foreach (var item in toDoItemsList.Items)
            {
                Items.Add(new ToDoItemViewModel
                {
                    Name = item.Name,
                    DueDate = item.DueDate,
                });
            }
        }

        public void AddItem(ToDoItemViewModel item)
        {
            toDoItemsList.Items.Add(new Models.ToDoItem
            {
                Name = item.Name,
                DueDate = item.DueDate,
            });
            Items.Add(item);
        }

        public void DeleteItem(ToDoItemViewModel item)
        {
            if (item != null && Items.Contains(item))
            {
                toDoItemsList.Items.RemoveAll(i => i.Name == item.Name && i.DueDate == item.DueDate);
                Items.Remove(item);
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
