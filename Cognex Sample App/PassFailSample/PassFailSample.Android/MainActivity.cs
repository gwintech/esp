using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using PassFailSample.Models;
using PassFailSample.IoC;
using Xamarin.Forms;
using PassFailSample.Droid.Models;
using System.IO;
using PassFailSample.Utilities.Navigation;
using Autofac;

namespace PassFailSample.Droid
{
    [Activity(Label = "PassFailSample", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = false,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

            global::Xamarin.Forms.Forms.Init(this, bundle);
            DisplayCrashReport();

            LoadApplication(new App());

            // Register All Dependency Services
            DependencyService.Register<ISaveAndLoad>();
            DependencyService.Register<IBarcodeScanner>();
            DependencyService.Get<BarcodeScanner>().Initialize(this);
            
            Android.Support.V7.Widget.Toolbar toolbar = this.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
        }

        public override void OnBackPressed()
        {
            IoCContainer.Container.Resolve<NavigationService>().PopAsync();
            // This overrides the default pop screen with the customizeable Navigation PopAsync method
        }

        //public override bool OnOptionsItemSelected(Android.Views.IMenuItem item)
        //{
        //    // check if the current item id is equals to the back button id
        //    if (item.ItemId == 16908332) // xam forms nav bar back button id
        //    {
        //        IoCContainer.Container.Resolve<NavigationService>().PopAsync();
        //    }
        //    return true;
        //}

        /// <summary>
        /// Error handling... Displays the last crash report if in debug mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="unobservedTaskExceptionEventArgs"></param>

        private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            var newExc = new Exception("TaskSchedulerOnUnobservedTaskException",
                unobservedTaskExceptionEventArgs.Exception);
            LogUnhandledException(newExc);
        }

        private static void CurrentDomainOnUnhandledException(object sender,
            UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var newExc = new Exception("CurrentDomainOnUnhandledException",
                unhandledExceptionEventArgs.ExceptionObject as Exception);
            LogUnhandledException(newExc);
        }

        internal static void LogUnhandledException(Exception exception)
        {
            try
            {
                const string errorFileName = "Fatal.log";
                var libraryPath =
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder
                        .Personal); // iOS: Environment.SpecialFolder.Resources
                var errorFilePath = Path.Combine(libraryPath, errorFileName);
                var errorMessage = String.Format("Time: {0}\r\nError: Unhandled Exception\r\n{1}", DateTime.Now,
                    exception.ToString());
                File.WriteAllText(errorFilePath, errorMessage);

                // Log to Android Device Logging.
                Android.Util.Log.Error("Crash Report", errorMessage);
            }
            catch
            {
                // just suppress any error logging exceptions
            }
        }

        /// <summary>
        // If there is an unhandled exception, the exception information is diplayed 
        // on screen the next time the app is started (only in debug configuration)
        /// </summary>
        [Conditional("DEBUG")]
        private void DisplayCrashReport()
        {
            const string errorFilename = "Fatal.log";
            var libraryPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var errorFilePath = Path.Combine(libraryPath, errorFilename);

            if (!File.Exists(errorFilePath))
            {
                return;
            }

            var errorText = File.ReadAllText(errorFilePath);
            new AlertDialog.Builder(this)
                .SetPositiveButton("Clear", (sender, args) =>
                {
                    File.Delete(errorFilePath);
                })
                .SetNegativeButton("Close", (sender, args) =>
                {
                    // User pressed Close.
                })
                .SetMessage(errorText)
                .SetTitle("Crash Report")
                .Show();
        }

    }
}