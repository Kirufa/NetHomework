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

namespace ChatServer
{
    public partial class Form_Server : Form
    {
        public Form_Server()
        {
            InitializeComponent();       
          
        }
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

        

    }
}
