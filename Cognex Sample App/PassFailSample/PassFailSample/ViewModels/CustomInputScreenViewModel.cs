using System;
using System.Windows.Input;
using PassFailSample.Helpers;
using PassFailSample.Models;
using Xamarin.Forms;
using PassFailSample.Utilities.Navigation;

namespace PassFailSample.ViewModels
{
    public sealed class CustomInputScreenViewModel : BaseViewModel
    {
        #region Properties

        public ICommand EntryCompletedCommand { get ; private set; }
        private DataStorage DataStore { get; set; }

        private string _failureReasonString;
        public string FailureReasonString
        {
            get => this._failureReasonString;
            set => this.SetProperty(ref this._failureReasonString, value);
        }

        #endregion

        #region Constructor and Init/Deinit

        public CustomInputScreenViewModel(DataStorage data, IdleTimeoutTimer timer, Settings settings) : base(timer, settings)
        {
            this.DataStore = data;
            this.EntryCompletedCommand = new Command(p =>
            {
                // TODO Change this once adding multiple text entry screens
                this.DataStore.SetStatusReason(this.FailureReasonString);
                this.DataStore.RecordBarcode();
                this.NavService.NavigateTo(typeof(ScanRequestedScreenViewModel));
            });
        }

        public override void Initialize()
        {
            base.Initialize();
            this.FailureReasonString = string.Empty;
        }

        public override void Deinitialize()
        {
            base.Deinitialize();
        }

        #endregion
    }
}