using System;
using System.Collections.Generic;
using PassFailSample.Helpers;
using PassFailSample.ViewModels;
using Autofac;
using PassFailSample.IoC;

namespace PassFailSample.Models
{
    public class EnabledScreens
    {

        private static List<Type> ScreensList { get; set; }

        private static void CheckOrInitScreensList(Settings settings)
        {
            if (ScreensList == null)
            {
                ScreensList = new List<Type>();
                ScreensList.Add(typeof(HomeScreenViewModel));
                if (settings.BoolUserLoginScreenEnabled)
                {
                    ScreensList.Add(typeof(UserLoginScreenViewModel));
                }
                ScreensList.Add(typeof(ScanRequestedScreenViewModel));
                if (settings.BoolBarcodeScannedScreenEnabled)
                {
                    ScreensList.Add(typeof(BarcodeScannedScreenViewModel));
                }
                if (settings.BoolFailureFeedbackScreenEnabled)
                {
                    ScreensList.Add(typeof(FailureFeedbackScreenViewModel));
                }
                if (settings.BoolCustomInputScreenEnabled)
                {
                    ScreensList.Add(typeof(CustomInputScreenViewModel));
                }
            }
        }

        public static bool IsLastScreen(BaseViewModel CurrentViewModel)
        {
            CheckOrInitScreensList(CurrentViewModel.Settings);
            var nextScreenIndex = ScreensList.IndexOf(CurrentViewModel.GetType()) + 1;
            return (nextScreenIndex >= ScreensList.Count); // Last Page?
        }

        public static Type GetNextScreen(BaseViewModel CurrentViewModel)
        {
            CheckOrInitScreensList(CurrentViewModel.Settings);

            // Identify what the current Screen is and find it in the list
            var nextScreenIndex = ScreensList.IndexOf(CurrentViewModel.GetType()) +1;
            if (nextScreenIndex < ScreensList.Count)
            {
                return ScreensList[nextScreenIndex];
            }
            else  // If the current view model is for the last enabled screen, then return to Scan Requested Screen
            {
                return typeof(ScanRequestedScreenViewModel);
            }
        }

    }
}