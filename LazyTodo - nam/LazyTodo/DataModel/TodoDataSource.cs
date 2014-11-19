using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace LazyTodo.DataModel
{
    public class MyMedia
    {
        //private StorageFile File { get; set; }
        public Uri fileUri { get; set; }
        public Uri imageUri { get; set; }
        public enum MyMediaType { Image, Video, Else };
        public MyMediaType myMediaType { get; set; }
        //public BitmapImage bitmapImage { get; set; }
        public MyMedia()
        {
            Debug.WriteLine("empty MyMedia constructed");
        }
        public MyMedia(StorageFile file)
        {
            //this.File = file;
            this.fileUri = new Uri(file.Path);
            Debug.WriteLine("fileUri: " + this.fileUri);
            //this.bitmapImage = new BitmapImage();
            string contentType = file.ContentType.ToString();
            if (contentType.Contains("image")) 
            {
                this.myMediaType = MyMediaType.Image;
            }
            else if (contentType.Contains("video"))
            {
                this.myMediaType = MyMediaType.Video;
            }
            else { this.myMediaType = MyMediaType.Else; }

            //Debug.WriteLine(contentType);
        }
        public void SetImageSource(Uri baseUri)
        {
            
            if (this.myMediaType == MyMediaType.Image) 
            {
                //var stream = await this.File.OpenReadAsync();
                //bitmapImage.SetSource(stream);
                //bitmapImage.UriSource = this.fileUri;
                this.imageUri = this.fileUri;
            }
            else if (this.myMediaType == MyMediaType.Video)
            {
                //bitmapImage.UriSource = new Uri(baseUri, "Assets/DarkGray.png");
                this.imageUri = new Uri(baseUri, "Assets/DarkGray.png");
            }
            Debug.WriteLine("imageUri: " + this.imageUri);
            
        }

        public static async Task<StorageFile> getFileFromUriAsync(/*Uri fileUri*/)
        {
            return await StorageFile.GetFileFromPathAsync("C:\\Data\\Users\\Public\\Pictures\\Camera Rol\\WP_20141119_001.jpg");
            //return await StorageFile.GetFileFromApplicationUriAsync(fileUri);
            //return await StorageFile.CreateStreamedFileFromUriAsync(fileUri.ToString(), fileUri, null);
        }

    }
    public class Todo : IComparable<Todo>
    {
        public string UniqueId { get; set; }
        public string TodoTitle { get; set; }
        public ObservableCollection<MyMedia> Attachments { get; set; }
        public DateTime TodoDateTime { get; set; }
        public string Description { get; set; }
        public Todo()
        {
            this.UniqueId = "";
            this.TodoTitle = "";
            this.Description = "";
        }
        public Todo(string title)
        {
            var myGuid = Guid.NewGuid();
            //Debug.WriteLine("Unique Id is .. " + myGuid);
            this.UniqueId = "" + myGuid;
            this.TodoTitle = title;
            this.Attachments = new ObservableCollection<MyMedia>();
            this.TodoDateTime = new DateTime(2014, 11, 28, 10, 20, 30);
            this.Description = "this is the description";
        }
        public void AddMedia(StorageFile file, Uri baseUri)
        {
            Debug.WriteLine("AddMedia: Method Entered");

            MyMedia myMedia = new MyMedia(file);
            myMedia.SetImageSource(baseUri);
            this.Attachments.Add(myMedia);
            /*this.NotifyPropertyChanged("Attachments");*/
        }
        public int CompareTo(Todo other)
        {
            if (this.TodoDateTime.Equals(other.TodoDateTime))
            {
                return this.TodoTitle.CompareTo(other.TodoTitle);
            }
            return this.TodoDateTime.CompareTo(other.TodoDateTime);
        }

    }
    public class TodoDataSource
    {
        private SortedSet<Todo> _createdTodo = new SortedSet<Todo>();
        public SortedSet<Todo> CreatedTodo
        {
            get { return this._createdTodo; }
            private set { }
        }
        public TodoDataSource()
        {
            /*
            for (int i = 1; i <= 5; i++)
            {
                Todo todo = new Todo("My New Todo " + i);
                //StorageFile.New;
                //todo.AddMedia();
                _todoCollection.Add(todo);
            }
             */ 
        }
        public void Add(string title)
        {
            Todo todo = new Todo(title);
            _createdTodo.Add(todo);
        }

        /** serializer **/
        private string JSONFILENAME = "lazytodo.json";
        public async Task writeJsonAsync()
        {
            StorageFile storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(JSONFILENAME, CreationCollisionOption.ReplaceExisting);
            var stream = await storageFile.OpenStreamForWriteAsync();

            // List<Todo> eiei = this._todoCollection.ToList();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(SortedSet<Todo>));
            serializer.WriteObject(stream, this._createdTodo);
            stream.Dispose();
        }
        public async Task readJsonAsync()
        {
            List<Todo> todoList = new List<Todo>();
            try
            {
                var serializer = new DataContractJsonSerializer(typeof(List<Todo>));
                var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(JSONFILENAME);

                StreamReader reader = new StreamReader(stream);

                Debug.WriteLine(await reader.ReadToEndAsync());
                todoList = (List<Todo>)serializer.ReadObject(stream);

                stream.Dispose();
            }
            catch (FileNotFoundException e)
            {
                todoList.Add(new Todo("Auto-generated TODO"));
                //throw;
            }

            this._createdTodo.Clear();
            foreach (Todo todo in todoList)
            {
                this._createdTodo.Add(todo);
            }

        }


        /*static*/
        private static TodoDataSource _todoDataSource = new TodoDataSource();
        public async static Task<TodoDataSource> getInstance()
        {
            await _todoDataSource.readJsonAsync();
            await _todoDataSource.writeJsonAsync();
            return _todoDataSource;
        }
        public async static Task writeToFile()
        {
            await _todoDataSource.writeJsonAsync();
        }
        public async static Task readToFile()
        {
            await _todoDataSource.readJsonAsync();
        }
        public static Todo GetTodoFromId(string searchId)
        {
            var matches = _todoDataSource.CreatedTodo.Where((item) => item.UniqueId.Equals(searchId));
            //Debug.WriteLine(((Todo)matches.First()).TodoTitle);
            if (matches.Count() >= 1) return matches.First();
            return new Todo("null");
        }
    }
}
