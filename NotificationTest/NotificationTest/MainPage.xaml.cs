using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace NotificationTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            ToastTemplateType toastTemplate = ToastTemplateType.ToastText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            Int16 dueTimeInSeconds = 10;
            DateTime dueTime = DateTime.Now.AddSeconds(dueTimeInSeconds);

            ScheduledToastNotification scheduledToast = new ScheduledToastNotification(toastXml, dueTime);

            scheduledToast.Id = "Future_Toast";

            ToastNotificationManager.CreateToastNotifier().AddToSchedule(scheduledToast);

            //ToastTemplateType toastTemplate = ToastTemplateType.ToastText02;
            //XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            // TO DO: Fill in the template with your notification content here. 
            

            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode("Hyehey"));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode("Hello World!"));
          
            Int16 startTimeInSeconds = 5;
            DateTime startTime = DateTime.Now.AddSeconds(startTimeInSeconds);

            ScheduledToastNotification recurringToast = new ScheduledToastNotification(toastXml, startTime, new TimeSpan(0,0,0,10),5); 
            recurringToast.Id = "Recurring_Toast";

            ToastNotificationManager.CreateToastNotifier().AddToSchedule(recurringToast);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }
    }
}
