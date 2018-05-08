using System.Windows.Input;
using Xamarin.Forms;
using PassFailSample.Helpers;
using PassFailSample.Models;
using PassFailSample.IoC;
using System.Collections.Generic;
using PassFailSample.Utilities;

namespace PassFailSample.ViewModels
{
    public sealed class FailureFeedbackScreenViewModel : BaseViewModel
    {
        #region Properties


        // The overall list that will keep track of which view models can be navigated
        // to and displayed in the "master" portion of master/detail
        public List<IButtonListItem<Button>> AvailableButtons { get; set; }
        private DataStorage DataStore { get; set; }
        public ICommand FailureFeedbackSelectCommand { get; private set; }
        const string OtherLabel = "OTHER";

        #endregion

        #region Constructor and Init/Deinit
        public FailureFeedbackScreenViewModel(DataStorage data, IdleTimeoutTimer timer, Settings settings) : base(timer, settings)
        {
            this.DataStore = data;
            this.FailureFeedbackSelectCommand = new Command(p =>
            {
                var temp = p as string;
                if (temp != null)
                {
                    if (temp == OtherLabel)
                    {
                        // TODO Change this once adding multiple text entry screens
                        this.NavService.NavigateTo(EnabledScreens.GetNextScreen(this));
                    }
                    else
                    {
                        this.GetFailureReason(temp);
                    }
                }
            });

            var ButtonList = this.Settings.ListFailureReasons.Trim().Split(';');
            this.AvailableButtons = new List<IButtonListItem<Button>>();
            foreach (string element in ButtonList)
            {
                this.AvailableButtons.Add(new ButtonListItem<Button>(element, this.FailureFeedbackSelectCommand, Color.FromHex(Settings.ColorStandardButton)));
            }
            if (Settings.BoolCustomInputScreenEnabled)
            {
                this.AvailableButtons.Add(new ButtonListItem<Button>(OtherLabel, this.FailureFeedbackSelectCommand, Color.FromHex(Settings.ColorStandardButton)));
            }

        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Deinitialize()
        {
            base.Deinitialize();
        }
        #endregion

        #region Methods
        private void GetFailureReason(string failReason)
        {
            this.DataStore.SetStatusReason(failReason);
            this.DataStore.RecordBarcode();
            //// TODO Change this once adding multiple text entry screens
            //using (var scope = IoCContainer.Container.BeginLifetimeScope())
            //{
            this.NavService.NavigateTo(typeof(ScanRequestedScreenViewModel));
            //}
            // If this is the last page in the sequence, then record the barcode
        }
        #endregion
    }
}