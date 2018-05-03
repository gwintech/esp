using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace ESP32_BLE
{
    public partial class Form1 : Form
    {
        public bool isConnected = false;
        BluetoothLEDevice bluetoothLeDevice;
        GattDeviceService currentSvc;
        GattCharacteristic selectedCharacteristic;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var watcher = new BluetoothLEAdvertisementWatcher();
            label1.Text = "";
            label2.Text = "";
            label3.Text = "";
            label4.Text = "";
            button1.Enabled = false;
            button2.Enabled = false;
            textBox1.MaxLength = 1;

            watcher.ScanningMode = BluetoothLEScanningMode.Active;

            // Only activate the watcher when we're recieving values >= -80
            watcher.SignalStrengthFilter.InRangeThresholdInDBm = -80;

            // Stop watching if the value drops below -90 (user walked away)
            watcher.SignalStrengthFilter.OutOfRangeThresholdInDBm = -90;

            // Register callback for when we see an advertisements
            watcher.Received += OnAdvertisementReceived;


            // Wait 5 seconds to make sure the device is really out of range
            watcher.SignalStrengthFilter.OutOfRangeTimeout = TimeSpan.FromMilliseconds(5000);
            watcher.SignalStrengthFilter.SamplingInterval = TimeSpan.FromMilliseconds(2000);

            // Starting watching for advertisements
            watcher.Start();
            getDevices();
            //ConnectDevice(53526636161810);
        }


        private void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher watcher, BluetoothLEAdvertisementReceivedEventArgs eventArgs)
        {
            // Tell the user we see an advertisement and print some properties
            /* Console.WriteLine(String.Format("Advertisement:"));
             Console.WriteLine(String.Format("  BT_ADDR: {0}", eventArgs.BluetoothAddress));
             Console.WriteLine(String.Format("  FR_NAME: {0}", eventArgs.Advertisement.ServiceUuids.FirstOrDefault()));
             List<Guid> test = eventArgs.Advertisement.ServiceUuids.ToList();*/
            //Guid s = test.FirstOrDefault();


            //eventArgs.Advertisement.ServiceUuids.FirstOrDefault();
            System.Threading.Thread.Sleep(1000); //try 5 second lay.
            //if (!isConnected)
                //ConnectDevice(eventArgs.BluetoothAddress);


            // Console.WriteLine();
        }

        async void getDevices() {
            var devices = await DeviceInformation.FindAllAsync(BluetoothLEDevice.GetDeviceSelector());
            List<string> dList = new List<string>();
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.Clear();
            listView1.Columns.Add("Device Name", 150);
            listView1.Columns.Add("Address", 150);
            foreach (DeviceInformation di in devices)
            {
                BluetoothLEDevice bleDevice = await BluetoothLEDevice.FromIdAsync(di.Id);
                
                string[] arr = new string[2];
                ListViewItem itm;
                //add items to ListView
                arr[0] = bleDevice.Name;
                arr[1] = bleDevice.BluetoothAddress.ToString();
                itm = new ListViewItem(arr);
                listView1.Items.Add(itm);
            }

            
           
        }
        async void ConnectDevice(ulong add)
        {

            

            // Note: BluetoothLEDevice.FromIdAsync must be called from a UI thread because it may prompt for consent.
            BluetoothLEDevice bluetoothLeDevice = await BluetoothLEDevice.FromBluetoothAddressAsync(add);
            
            //System.Threading.Thread.Sleep(5000);
            isConnected = true;
            // BluetoothLEDevice bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(ble.DeviceId);
            //Console.WriteLine("Device Added");
            //Console.ReadLine();
            //GattDeviceServicesResult result = await bluetoothLeDevice;
            //var prslt = await bluetoothLeDevice.DeviceInformation.Pairing.PairAsync();


            //System.Threading.Thread.Sleep(7000); //try 5 second lay.
            Guid serUuid = new Guid("4fafc201-1fb5-459e-8fcc-c5c9c331914b");
            Guid charUuid = new Guid("beb5483e-36e1-4688-b7f5-ea07361b26a8");
            //IReadOnlyList<GattDeviceService> svc = bluetoothLeDevice.GattServices;
            GattDeviceService service = bluetoothLeDevice.GetGattService(serUuid);

            //Console.ReadLine();
            //Console.WriteLine(String.Format("  Device Name: {0}", bluetoothLeDevice.DeviceInformation.Name));
            //Console.WriteLine("Getting Characterstics"); 
            //IReadOnlyList<GattCharacteristic> chars = service.GetAllCharacteristics();

            GattCharacteristic selectedCharacteristic = service.GetCharacteristics(charUuid).FirstOrDefault();
            //Console.WriteLine(chars.FirstOrDefault().Uuid);
            // Console.WriteLine(svc.FirstOrDefault().Uuid);
            //
            //IReadOnlyList<GattCharacteristic> chrs = svc.GetAllCharacteristics();
            /*
            if (result.Status == GattCommunicationStatus.Success)
            {
                var services = result.Services;

                // ...
            }*/

            //*****  WRITE DATA *******


            var writer = new DataWriter();
            // WriteByte used for simplicity. Other commmon functions - WriteInt16 and WriteSingle
            writer.WriteByte(0x78);

            GattCommunicationStatus result1 = await selectedCharacteristic.WriteValueAsync(writer.DetachBuffer());
            if (result1 == GattCommunicationStatus.Success)
            {
                Console.WriteLine("Written Successfully");
            }





            //*****  READING DATA  ********

            GattReadResult result = await selectedCharacteristic.ReadValueAsync();

            if (result.Status == GattCommunicationStatus.Success)
            {
                var reader = DataReader.FromBuffer(result.Value);
                byte[] input = new byte[reader.UnconsumedBufferLength];
                reader.ReadBytes(input);
                Console.WriteLine(Encoding.UTF8.GetString(input));

                // Utilize the data as needed
            }





            // ...
        }



        async void getServices(string add)
        {
            ulong a = ulong.Parse(add);
            bluetoothLeDevice = await BluetoothLEDevice.FromBluetoothAddressAsync(a);
            listView2.Clear();
            if (bluetoothLeDevice.DeviceInformation.Pairing.IsPaired)
            {
                IReadOnlyList<GattDeviceService> svc = bluetoothLeDevice.GattServices;
                
                listView2.Columns.Add("Service UUID", 150);
                listView2.Columns.Add("", 150);
                foreach (GattDeviceService sv in svc)
                {
                    string[] arr = new string[2];
                    ListViewItem itm;
                    //add items to ListView
                    arr[0] = sv.Uuid.ToString();
                    arr[1] = "";
                    itm = new ListViewItem(arr);
                    listView2.Items.Add(itm);

                }
            }
            else {
                label1.Text = "Device Not Paired!";
            }

            
        }

        private void getCharacterstics(string id)
        {
            Guid g = new Guid(id);

            currentSvc = bluetoothLeDevice.GetGattService(g);
            IReadOnlyList<GattCharacteristic> chrs = currentSvc.GetAllCharacteristics();
            listView3.Clear();
            listView3.Columns.Add("Characterstic UUID", 150);
            listView3.Columns.Add("Description", 150);
            foreach (GattCharacteristic c in chrs)
            {
                string[] arr = new string[2];
                ListViewItem itm;
                //add items to ListView
                arr[0] = c.Uuid.ToString();
                arr[1] = c.CharacteristicProperties.ToString();

                itm = new ListViewItem(arr);
                
                listView3.Items.Add(itm);
            }
            //do nothing

        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            if (listView1.SelectedItems.Count > 0)
                getServices(listView1.SelectedItems[0].SubItems[1].Text);
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            if (listView2.SelectedItems.Count == 0)
                label1.Text = "NO Charachterstics Found!!";
            else
            {
                getCharacterstics(listView2.SelectedItems[0].SubItems[0].Text);
                label1.Text = "";
            }
                
        }

        private void button1_Click(object sender, EventArgs e)
        {
            readChar();
        }

        async void readChar()
        {
            GattReadResult result = await selectedCharacteristic.ReadValueAsync();

            if (result.Status == GattCommunicationStatus.Success)
            {
                var reader = DataReader.FromBuffer(result.Value);
                byte[] input = new byte[reader.UnconsumedBufferLength];
                reader.ReadBytes(input);
                label2.Text =Encoding.UTF8.GetString(input);

                // Utilize the data as needed
            }
        }
        async void writeChar()
        {
            var writer = new DataWriter();
            // WriteByte used for simplicity. Other commmon functions - WriteInt16 and WriteSingle
            byte b = new byte();
            b = (byte)textBox1.Text.ToCharArray()[0];
            writer.WriteByte(b);

            GattCommunicationStatus result1 = await selectedCharacteristic.WriteValueAsync(writer.DetachBuffer());
            if (result1 == GattCommunicationStatus.Success)
            {
                label3.Text = "Written Successfully";
                button1.Enabled = true;
            }
        }

            private void listView3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView3.SelectedItems.Count>0)
            {
                button1.Enabled = true;
                Guid g = new Guid(listView3.SelectedItems[0].SubItems[0].Text);
                selectedCharacteristic = currentSvc.GetCharacteristics(g).FirstOrDefault();
                if (selectedCharacteristic.CharacteristicProperties.ToString().Contains("Write"))
                    button2.Enabled = true;
                else
                    button2.Enabled = false;
            }
            else
            {
                button2.Enabled = false;
                button1.Enabled = false;
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            writeChar();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(bluetoothLeDevice!=null)
                label4.Text = bluetoothLeDevice.Name + " " + bluetoothLeDevice.ConnectionStatus.ToString();
        }
    }
}
