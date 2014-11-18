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
        public string UniqueId { get; private set; }
        public DateTime TodoDateTime { get; set; }
        public string Description { get; set; }
        public Todo(string title)
        {
            this.TodoTitle = title;
            //Debug.WriteLine(title);
            this.UniqueId = "" + getTimestamp(DateTime.Now);
            //Debug.WriteLine(this.UniqueId);
            this.TodoDateTime = new DateTime(2014,11,28,10,20,30);
            this.Description = "this is the description";
        }
        private double getTimestamp(DateTime dateTime) 
        {
            DateTime refDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            double timestamp = (dateTime.ToUniversalTime() - refDateTime).TotalSeconds;
            return timestamp;
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


        /*static*/
        private static TodoDataSource _todoDataSource = new TodoDataSource();
        public static TodoDataSource getInstance()
        {
            return _todoDataSource;
        }
        public static Todo GetTodoFromId(string searchId)
        {
            var matches = _todoDataSource.TodoCollection.Where((item) => item.UniqueId.Equals(searchId));
            Debug.WriteLine(((Todo)matches.First()).TodoTitle);
            if (matches.Count() >= 1) return matches.First();
            return new Todo("null");
        }
    }
}
