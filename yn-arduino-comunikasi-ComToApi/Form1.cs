using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Net;

namespace yn_arduino_comunikasi_ComToApi
{
    public partial class Form1 : Form
    {
        public string kl = "http://coba-coba.dev:81/skrispi-realtimedata/rest-api.php";
        public static System.IO.Ports.SerialPort port;
        delegate void SetTextCallback(string text);

        // This BackgroundWorker is used to demonstrate the 
        // preferred way of performing asynchronous operations.
        private BackgroundWorker hardWorker;

        private Thread readThread = null;

        public Form1()
        {
            InitializeComponent();

            hardWorker = new BackgroundWorker();
           
        }

        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.receiveText.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {

                string a = Convert.ToString(text);
                string[] b = a.Split('#');
                string c = Convert.ToString(b.Count());

                if (b.Count() == 5)
                {
                    this.receiveText.Text = text;
                    atur2(b);
                }
            }
        }
        public void atur2(string[] data)
        {
            WebClient objWebClient = new WebClient();

            NameValueCollection objNameValueCollection = new NameValueCollection();
            objNameValueCollection.Add("suhu", data[0]);
            objNameValueCollection.Add("lpg", data[1]);
            objNameValueCollection.Add("co", data[2]);
            objNameValueCollection.Add("api", data[3]);
            objNameValueCollection.Add("hasil", data[4]);

            byte[] bytes = objWebClient.UploadValues(kl, "POST", objNameValueCollection);
            //MessageBox.Show(Encoding.ASCII.GetString(bytes));
        }
        public void Read()
        {
            while (port.IsOpen)
            {
                try
                {
                    if (port.BytesToRead > 0)
                    {
                        string message = port.ReadLine();
                        this.SetText(message);
                    }
                }
                catch (TimeoutException) { }
            }
        }


        private void btnConnect_Click_1(object sender, EventArgs e)
        {
            System.ComponentModel.IContainer components =
            new System.ComponentModel.Container();
            port = new System.IO.Ports.SerialPort(components);
            port.PortName = comPort.SelectedItem.ToString();
            port.BaudRate = Int32.Parse(baudRate.SelectedItem.ToString());
            port.DtrEnable = true;
            port.ReadTimeout = 5000;
            port.WriteTimeout = 500;
            port.Open();

            readThread = new Thread(new ThreadStart(this.Read));
            readThread.Start();
            this.hardWorker.RunWorkerAsync();

            btnConnect.Text = "<Connected>";

            btnConnect.Enabled = false;
            comPort.Enabled = false;
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            foreach (string s in SerialPort.GetPortNames())
            {
                comPort.Items.Add(s);
            }
            if (comPort.Items.Count > 0)
                comPort.SelectedIndex = comPort.Items.Count - 1;
            else
                comPort.SelectedIndex = 0;

            baudRate.Items.Add("2400");
            baudRate.Items.Add("4800");
            baudRate.Items.Add("9600");
            baudRate.Items.Add("14400");
            baudRate.Items.Add("19200");
            baudRate.Items.Add("28800");
            baudRate.Items.Add("38400");
            baudRate.Items.Add("57600");
            baudRate.Items.Add("115200");

            baudRate.SelectedIndex = 2;
        }


        private void sendBtn_Click_1(object sender, EventArgs e)
        {
            kl = url.Text;
        }

        private void Form1_FormClosed_1(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (!(readThread == null))
                    readThread.Abort();
            }
            catch (NullReferenceException)
            {
            }

            try
            {
                port.Close();
            }
            catch (NullReferenceException)
            {
            }
        }
    }
}
