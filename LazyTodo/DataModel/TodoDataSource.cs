using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyTodo.DataModel
{
    public class Todo
    {
        public string TodoTitle { get; set; }
        public Todo(string title)
        {
            this.TodoTitle = title;
            Debug.WriteLine(title);
        }
    }
    public class TodoDataSource
    {
        private ObservableCollection<Todo> _todoCollection = new ObservableCollection<Todo>();
        public ObservableCollection<Todo> TodoCollection 
        {
            get { return this._todoCollection; }
            private set { }
        }
        public TodoDataSource()
        {
            for (int i = 1; i <= 5; i++)
            {
                Todo todo = new Todo("My New Todo " + i);
                _todoCollection.Add(todo);
            }
        }
        public void Add(string title)
        {
            Todo todo = new Todo(title);
            _todoCollection.Add(todo);
        }
    }
}
