using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using System.Net;

namespace C_Networking_Tool
{
    // Main Frame / Main Thread
    public partial class Form1 : Form
    {

        CancellationTokenSource CancelToken = null; //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public Form1()
        {
            Thread mainThread = Thread.CurrentThread;
            mainThread.Name = "Main Thread";

            InitializeComponent();

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }



        //Validating IP-----------------------------------------------------------
        public void IsValidIPAddress(string IpAddress)
        {
            try
            {
                IPAddress IP;
                if (IpAddress.Count(c => c == '.') == 3)
                {
                    bool flag = IPAddress.TryParse(IpAddress, out IP);
                    if (flag)
                    {
                        iptext.Clear();
                        consoleOutput.Clear();
                        consoleOutput.Text = "Starting....\n";
                        
                        //button control
                        iptext.Enabled = false;
                        button1.Enabled = false;
                        btn_cancel.Enabled = true;

                        //Creating new Thread ----------------------------------------------------
                        Thread RunScanMethod = new Thread(new ParameterizedThreadStart(RunScan));
                        RunScanMethod.Start(IP);
                        //------------------------------------------------------------------------

                    }
                    else
                    {
                        
                        iptext.Clear();
                        consoleOutput.Clear();
                        consoleOutput.Text = "You entered a not valid IP. Try again.";
                    }
                }
                else
                {
                    iptext.Clear();
                    consoleOutput.Clear();
                    consoleOutput.Text = "You entered a not valid IP. Try again.";
                }
            }
            catch (Exception) { }
        }

        //------------------------------------------------------------------------
        //Ports
        private static int[] Ports = new int[]
        {
            20, //FTP Data Transfer
            21, //FTP Command Control
            22, //FTPS/SSH
            23, //Telnet
            25, //SMTP
            26, //SMTP
            53, //DNS
            80, //HTTP
            110, //POP3
            143, //IMAP
            443, //HTTPs
            587, //SMTP SSL
            993, //IMAP SSL
            995, //POP3 SSL
            2077, // WebDAV/WEBDisk
            2078, // Webdav/webdisk ssl
            2082, // Cpanel
            2083, // Cpanel ssl
            2086, // WHM -webhost manager
            2087, // whm - ssl
            2095, //webmail
            2096, //webmail ssl
            3306, // mysql



        };

        //Port Scan Method / New Thread
        private void RunScan(object IP)
        {
            CancelToken = new CancellationTokenSource();
            var token = CancelToken.Token;

            string IPX = IP.ToString();
            string result = "Running...";
            consoleOutput.Invoke(new MethodInvoker(delegate () { consoleOutput.Text = result; }));
            foreach (int s in Ports)
            {
                if (token.IsCancellationRequested)
                {
                    //token.ThrowIfCancellationRequested();
                    result = null;
                    return;
                }
                using (TcpClient Scan = new TcpClient())
                {
                    try 
                    {
                        Scan.Connect(IPX, s);
                        result += Environment.NewLine + ($"[{s}]" + " | Open\n");
                        consoleOutput.Invoke(new MethodInvoker(delegate () { consoleOutput.Text = result; }));

                    }
                    catch 
                    {
                        result += Environment.NewLine + ($"[{s}]" + " | Closed\n");
                        consoleOutput.Invoke(new MethodInvoker(delegate () { consoleOutput.Text = result; }));
                    }
                }

            }
            result += Environment.NewLine + "Port Scan finished. \n";
            consoleOutput.Invoke(new MethodInvoker(delegate () { consoleOutput.Text = result; }));
            button1.Invoke(new MethodInvoker(delegate () { button1.Enabled = true; }));
            iptext.Invoke(new MethodInvoker(delegate () { iptext.Enabled = true; }));
            btn_cancel.Invoke(new MethodInvoker(delegate () { btn_cancel.Enabled = false; }));


        }

        // Scan Button Click
        private void button1_MouseClick(object sender, MouseEventArgs e)
        {

            if (String.IsNullOrEmpty(consoleOutput.Text))
            {
                consoleOutput.Text = "Please Enter IP first\n";
            }
            
            
            consoleOutput.Text = "Starting....\n";
            string IP = iptext.Text;
            IsValidIPAddress(IP);

        }

        private void consoleOutput_TextChanged(object sender, EventArgs e)
        {
        
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        //Cancel Button Click
        private void btn_cancel_Click(object sender, EventArgs e)
        {
            CancelToken.Cancel();
            Thread.Sleep(3000);
            iptext.Clear();
            consoleOutput.Invoke(new MethodInvoker(delegate () { consoleOutput.Text = "Cancelled"; }));
            btn_cancel.Enabled = false;
            button1.Enabled = true;
            iptext.Enabled = true;
        }

        // IP Text Enter
        private void iptext_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                string IP = iptext.Text;
                IsValidIPAddress(IP);

            }
        }
    }
}




