using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace to_do_uwp.ViewModels
{
    public class ToDoItemViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Models.ToDoItem toDoItem;

        public ToDoItemViewModel()
        {
            this.toDoItem = new Models.ToDoItem();
        }

        public string Name
        {
            get => toDoItem.Name;

            set
            {
                toDoItem.Name = value;
                NotifyPropertyChanged();
            }
        }

        public string DueDate
        {
            get => toDoItem.DueDate;

            set
            {
                toDoItem.DueDate = value;
                NotifyPropertyChanged();
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
