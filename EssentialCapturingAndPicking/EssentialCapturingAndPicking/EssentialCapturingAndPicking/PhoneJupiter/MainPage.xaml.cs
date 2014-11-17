using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace LazyToDo
{
    public sealed partial class MainPage : Page
    {
        private static readonly IEnumerable<string> SupportedVideoFileTypes = new List<string> { ".mp4", ".wmv", ".avi" };
        private static readonly IEnumerable<string> SupportedImageFileTypes = new List<string> { ".jpeg", ".jpg", ".png" };

     

        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            animate.Begin();
            // Attach event which will return the picked files
            var app = Application.Current as App;
            if (app != null)
            {
                app.FilesPicked += OnFilesPicked;
            }   
        }


        private async void OnFilesPicked(IReadOnlyList<StorageFile> files)
        {
            Image.Source = null;
            MediaElement.Source = null;

            if (files.Count > 0)
            {
                // Check if video or image and pick first file to show
                var imageFile = files.FirstOrDefault(f => SupportedImageFileTypes.Contains(f.FileType.ToLower()));
                if (imageFile != null)
                {
                    var bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(await imageFile.OpenReadAsync());
                    Image.Source = bitmapImage;
                }

                var videoFile = files.FirstOrDefault(f => SupportedVideoFileTypes.Contains(f.FileType.ToLower()));
                if (videoFile != null)
                {
                    MediaElement.SetSource(await videoFile.OpenReadAsync(), videoFile.ContentType);
                }
            }
        }

        private void BtnFileOpenPickerVideoClick(object sender, RoutedEventArgs e)
        {
            TriggerPicker(SupportedVideoFileTypes);
        }

        private void BtnFileOpenPickerPhotosClick(object sender, RoutedEventArgs e)
        {
            TriggerPicker(SupportedImageFileTypes);
        }

        private void BtnFileOpenPickerPhotosAndVideoClick(object sender, RoutedEventArgs e)
        {
            TriggerPicker(SupportedImageFileTypes.Union(SupportedVideoFileTypes), true);
        }

        private static void TriggerPicker(IEnumerable<string> fileTypeFilers, bool shouldPickMultiple = false)
        {
            var fop = new FileOpenPicker();
            foreach (var fileType in fileTypeFilers)
            {
                fop.FileTypeFilter.Add(fileType);
            }
            if (shouldPickMultiple)
            {
                fop.PickMultipleFilesAndContinue();
            }
            else
            {
                fop.PickSingleFileAndContinue();
            }
        }
    }
}