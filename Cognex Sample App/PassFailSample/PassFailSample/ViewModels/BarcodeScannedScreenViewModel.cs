using System.Windows.Input;
using PassFailSample.Helpers;
using PassFailSample.Models;
using Xamarin.Forms;
using PassFailSample.IoC;
using Autofac;
using PassFailSample.Utilities.Navigation;
using PassFailSample.Utilities;
using System.Collections.Generic;

namespace PassFailSample.ViewModels
{
    public sealed class BarcodeScannedScreenViewModel : BaseViewModel
    {
        #region Properties

        private string _scannedBarcode;
        public string ScannedBarcode
        {
            get => this._scannedBarcode;
            set => this.SetProperty(ref this._scannedBarcode, value);
        }

        public List<IButtonListItem<Button>> AvailableButtons { get; set; }
        public ICommand EntryCompleteCommand { get; private set; }
        private DataStorage DataStore { get; set; }

        #endregion

        #region Constructor and Init/Deinit

        public BarcodeScannedScreenViewModel(IdleTimeoutTimer timer, Settings settings, DataStorage data) : base(timer, settings)
        {
            this.DataStore = data;
            this.EntryCompleteCommand = new Command(p =>
            {
                var temp = p as string;
                if (temp != null)
                {
                    this.CompletePage(temp != this.Settings.TextFailButton, temp);
                }
            });

            var ButtonList = this.Settings.ListAdditionalStatusOptions.Trim().Split(';');
            this.AvailableButtons = new List<IButtonListItem<Button>>();
            this.AvailableButtons.Add(new ButtonListItem<Button>(this.Settings.TextPassButton, this.EntryCompleteCommand, Color.FromHex(Settings.ColorPass)));
            this.AvailableButtons.Add(new ButtonListItem<Button>(this.Settings.TextFailButton, this.EntryCompleteCommand, Color.FromHex(Settings.ColorFail)));
            foreach (string element in ButtonList)
            {
                if (element != "")
                {
                    this.AvailableButtons.Add(new ButtonListItem<Button>(element, this.EntryCompleteCommand, Color.FromHex(Settings.ColorStandardButton)));
                }
            }
        }
        
        public override void Initialize()
        {
            base.Initialize();
            this.ScannedBarcode = string.IsNullOrEmpty(this.DataStore.Barcode) ? "Nothing was Scanned" : this.DataStore.Barcode;
        }

        public override void Deinitialize()
        {
            base.Deinitialize();
        }

        #endregion

        #region Methods

        private void CompletePage(bool stationPass, string status)
        {

            this.DataStore.SetStatus(status);
            if (stationPass)
            {
                this.DataStore.RecordBarcode();
                this.NavService.NavigateTo(typeof(ScanRequestedScreenViewModel)); // TODO change this after adding more input screens
            }
            else
            {
                // If this is the last page in the sequence, then record the barcode
                if (EnabledScreens.IsLastScreen(this))
                {
                    this.DataStore.RecordBarcode();
                }
                this.NavService.NavigateTo(EnabledScreens.GetNextScreen(this));
            }
        }

        #endregion

    }
}