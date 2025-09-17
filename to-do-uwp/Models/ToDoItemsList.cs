using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace to_do_uwp.Models
{
    public class ToDoItemsList
    {
        public string Name {  get; set; }
        public List<ToDoItem> Items { get; set; }

        public ToDoItemsList() {
            Name = "To-Do List";
            Items = new List<ToDoItem>();
        }
    }
}
