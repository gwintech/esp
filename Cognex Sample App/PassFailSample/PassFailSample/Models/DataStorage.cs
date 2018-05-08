using PassFailSample.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PassFailSample.Models
{
    public class DataStorage
    {
        #region Properties

        public List<string> ListTextAdders { get; private set; }
        public string StatusReason { get; private set; }
        public string Status { get; private set; }
        public string Barcode { get; private set; }
        public string UserID { get; private set; }
        private DateTime _ScanTime { get; set; }
        public Settings Settings { get; private set; }
        #endregion

        #region Constructor and Init/Deinit
        public DataStorage(Settings settings)
        {
            this.Barcode = string.Empty;
            this.UserID = string.Empty;
            this.Settings = settings;
        }
        
        // TODO: Do we need this? maybe create a new csv file for each new instance of this class? This doesn't get called anywhere, call on app startup after autofac init?
        public bool Initialize()
        {
            return DependencyService.Get<ISaveAndLoad>().Initialize();
        }

        // TODO: Do we need this? Close all references when app closes
        public bool Deinitialize()
        {
            return true;
        }
        #endregion

        #region Methods

        public void SetUserID(string user)
        {
            this.UserID = user;
        }
        public void SetBarcode(string barcode)
        {
            this.Barcode = barcode;
            this._ScanTime = DateTime.Now;
        }
        public void SetStatus(string status)
        {
            this.Status = status;
        }
        public void SetStatusReason(string statusReason)
        {
            this.StatusReason = statusReason;
        }
        public void SetTextField(string text)
        {
            this.ListTextAdders.Add(text);
        }
        private void ClearStoredBarcodeData()
        {
            this.Barcode = string.Empty;
            this.Status = string.Empty;
            this.StatusReason = string.Empty;
            this.ListTextAdders.Clear();
        }

        public async Task<bool> RecordBarcode()
        {
            var time = this._ScanTime;
            if (this.Settings.BoolUseLoggedEntryTime)
            {
                time = DateTime.Now;
            }
            // TODO: add in the extra text fields
            if (await DependencyService.Get<ISaveAndLoad>().AppendText($"{this.UserID}, {this.Barcode}, {time}, {this.Status}, {this.StatusReason},\r\n"))
            {
                ClearStoredBarcodeData();
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}