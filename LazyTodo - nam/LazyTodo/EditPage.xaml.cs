using LazyTodo.Common;
using LazyTodo.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace LazyTodo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private static readonly IEnumerable<string> SupportedVideoFileTypes = new List<string> { ".mp4", ".wmv", ".avi" };
        private static readonly IEnumerable<string> SupportedImageFileTypes = new List<string> { ".jpeg", ".jpg", ".png" };

        private string itemId;
        private Todo item;

        public EditPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            //EditPageStoryBoard.Begin();

            var app = Application.Current as App;
            if (app != null)
            {
                app.FilesOpenPicked += OnFilesOpenPicked;
            } 
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
            
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            itemId = (string)e.NavigationParameter;
            item = TodoDataSource.GetTodoFromId(itemId);

            this.DefaultViewModel["ItemToEdit"] = item;

            Debug.WriteLine(item.Attachments.Count);

            MyDatePicker.Date = item.TodoDateTime.Date;
            MyTimePicker.Time = item.TodoDateTime.TimeOfDay;

        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            DateTimeOffset date = MyDatePicker.Date;
            TimeSpan time = MyTimePicker.Time;
            DateTime dateTime = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds);

            item.TodoTitle = TitleBox.Text;
            item.TodoDateTime = dateTime;
            item.Description = DescriptionBox.Text;
            
            //Debug.WriteLine(dateTime);

            await TodoDataSource.writeToFile();

            this.Frame.GoBack();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }
        private void AddMediaButton_Click(object sender, RoutedEventArgs e)
        {
            TriggerPicker(SupportedImageFileTypes.Union(SupportedVideoFileTypes));
        }

        private async void UpdateMediaImage()
        {
            ObservableCollection<BitmapImage> BitmapImageAttached = new ObservableCollection<BitmapImage>();
            foreach (MyMedia myMedia in item.Attachments)
            {
                BitmapImage bitmapImage = new BitmapImage();
                if (myMedia.myMediaType == MyMedia.MyMediaType.Image)
                {
                    //StorageFile file = await MyMedia.getFileFromUriAsync(/*myMedia.fileUri*/);
                    //var stream = await file.OpenReadAsync();
                    //bitmapImage.SetSource(stream);
                    //stream.Dispose();
                }
                else
                {
                    bitmapImage.UriSource = new Uri(this.BaseUri, "Assets/DarkGray.png");
                }
            }
            this.defaultViewModel["AttachedImages"] = BitmapImageAttached;
        }


        /** file picker + camera **/
        private void OnFilesOpenPicked(IReadOnlyList<StorageFile> files)
        {
            foreach (StorageFile file in files)
            {
                if (file != null)
                {
                    item.AddMedia(file, this.BaseUri);
                    UpdateMediaImage();
                }
            }

        }
        private static void TriggerPicker(IEnumerable<string> fileTypeFilters)
        {
            var fileOpenPicker = new FileOpenPicker();

            foreach (var fileType in fileTypeFilters)
            {
                fileOpenPicker.FileTypeFilter.Add(fileType);
            }
            fileOpenPicker.PickMultipleFilesAndContinue();
        }

    }
}
