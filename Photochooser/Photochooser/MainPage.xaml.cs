using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Photochooser.Resources;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;

namespace Photochooser
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void Select_Existing_image_button_click(object sender, RoutedEventArgs e)
        {
            //creating a new instance of photo chooser task
            PhotoChooserTask choosetask = new PhotoChooserTask();

            choosetask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed); 
               
            //camera button visibilty
            choosetask.ShowCamera = true;
            
            //setting the height and width
            choosetask.PixelHeight = 300;
            choosetask.PixelWidth = 300;
            
            //launch the task
            choosetask.Show();
        }


        //Task completed event handler
        private void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            //checking whether image was selected or not
            if (e.TaskResult == TaskResult.OK)
            {
                BitmapImage image = new BitmapImage();
                image.SetSource(e.ChosenPhoto);
                //imagecontrol.Source = image;
                profilecontrol.ImageSource = image;
                 
            }
        }


        //Capture a new image
        private void Capture_a_new_image(object sender, RoutedEventArgs e)
        {
            CameraCaptureTask cameratask = new CameraCaptureTask();
            cameratask.Completed += new EventHandler<PhotoResult>(photoCameraCapture_Completed); 
            cameratask.Show();
        }


        //Task complete event handler
        private void photoCameraCapture_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                BitmapImage image = new BitmapImage();
                image.SetSource(e.ChosenPhoto);
                imagecontrol.Source = image;
            }
        }

    }
}