using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Handle;
using Khendys.Controls;

namespace ChatServer
{
    public partial class Form_Server : Form
    {
        public Form_Server()
        {
            InitializeComponent();       
          
        }

        private static ExRichTextBox ERT;        

        /*
        Socket client;
        Socket server;
        IPEndPoint IP;
        */
        private void button_Start_Click(object sender, EventArgs e)
        {
            /*server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IP = new IPEndPoint(IPAddress.Parse(SocketHandle.SocketData.ServerIP), SocketHandle.SocketData.Port);
            server.Bind(IP);

            server.Listen(1);
            client = server.Accept();*/

            SocketHandle.InitialServer(this.textBox1.Text);
            this.button_Start.Enabled = false;




        }

        private void button1_Click(object sender, EventArgs e)
        {/*
            byte[] arr = new byte[2048];
            EndPoint end = (EndPoint)IP;
            Socket Client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Client.Bind(end);

            int n = Client.ReceiveFrom(arr,ref end);
           // textBox1.Text = Encoding.ASCII.GetString(arr, 0, n);
            Client.SendTo(arr, n, SocketFlags.None, end);*/
        }

        private void Form_Server_Load(object sender, EventArgs e)
        {
            ERT = new ExRichTextBox();
            ERT.Size = panel_RichTextBox.Size;
            ERT.Location = new Point(0, 0);
            panel_RichTextBox.Controls.Add(ERT);
        }

        public static void AddText(string Name,string _ID,string _Query,string Text)
        {
            if (ERT.InvokeRequired)
            {
                ERT.Invoke((MethodInvoker)delegate
                    {
                        string _Str = Name + "(" + _ID + ")" + _Query + " : " + Text + '\n';
                        ERT.AppendText(_Str);
                    });
                
            }
            else
            {
                string _Str = Name + "(" + _ID + ")" + _Query + " : " + Text + '\n';
                ERT.AppendText(_Str);
            }
        }

        

    }
}
