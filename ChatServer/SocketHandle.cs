using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml.Serialization;
namespace Handle
{
    public class SocketHandle
    {
        [Serializable]
        public class Dgram
        {           
            public byte[] Data = new byte[MAX_DATASIZE];
            public byte[] Name = new byte[STRING_SIZE];
            public int DataLength;
            public int StringLength;
            public int Type;
            //0: check alive, client <-> server
            //1: TCP Text Data, client -> server 
            //2: TCP Text Data, server -> client 
            //3: TCP UDP Image Size, follow the UDP Image, client -> server 
            //4: TCP UDP Image Size, follow the UDP Image, server -> client
            //5: UDP Image, client -> server
            //6: UDP Image, server -> client
          


        }

        public const int MAX_DATASIZE = 2048;
        public const int STRING_SIZE = 512;
      /*  public const int DGRAM_SIZE = sizeof(byte) * MAX_DATASIZE
                                    + sizeof(byte) * STRING_SIZE
                                    + sizeof(int)
                                    + sizeof(int)
                                    + sizeof(int);
        */
        public const int RECEIVE_LENGTH = 10240;

        public static void SendPicture(Socket des, Bitmap bmp)
        {
            List<byte[]> _Data = DivideBitmap(bmp);
        }

        private static byte[] DgramToByte(Dgram dgram)
        {
            MemoryStream _Ms = new MemoryStream();
            XmlSerializer _Xs = new XmlSerializer(typeof(Dgram));
            _Xs.Serialize(_Ms, dgram);
            return _Ms.ToArray();
        }

        private static Dgram ByteToDgram(byte[] _Arr)
        {
            MemoryStream _Ms = new MemoryStream(_Arr);
            XmlSerializer _Xs = new XmlSerializer(typeof(Dgram));
          //  MessageBox.Show(_Ms.ToArray().Length.ToString());

         /*   using (FileStream fs = new FileStream("123.xml", FileMode.Create))
            {
                fs.Write(_Ms.ToArray(), 0, (int)_Ms.Length);
                fs.Close();
            }
            */


            Dgram _Ret = (Dgram)_Xs.Deserialize(_Ms); 
            return _Ret;
        }

        private static List<byte[]> DivideBitmap(Bitmap bmp)
        {
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            byte[] _Arr = ms.ToArray();
            int index = 0;
            List<byte[]> _Ret = new List<byte[]>();
            while (true)
            {
                if (index + MAX_DATASIZE < _Arr.Length)
                {
                    byte[] _Temp = new byte[MAX_DATASIZE];
                    Array.Copy(_Arr, index, _Temp, 0, MAX_DATASIZE);
                    _Ret.Add(_Temp);

                    index += MAX_DATASIZE;
                }
                else
                {
                    int Len = _Arr.Length - index;

                    byte[] _Temp = new byte[Len];
                    Array.Copy(_Arr, index, _Temp, 0, Len);
                    _Ret.Add(_Temp);

                    break;
                }

            }

            return _Ret;
        }

        public static void InitialServer(string _IP)
        {
            SocketData.Server = new SocketData.ServerData();
            SocketData.ServerData.ServerIP = _IP;
            SocketData.Server.InitialTCPServer();
            SocketData.Server.InitialUDPServer();
        }

        private static byte[] StrToByte(string _Str)
        {
            return Encoding.Unicode.GetBytes(_Str);
        }

        private static string ByteToStr(byte[] _Arr,int Len)
        {
           
            return Encoding.Unicode.GetString(_Arr, 0, Len);
        }

        private void SendToAllClient(byte[] Data,int Leng,int Tyte)
        {
            Dgram _Dg = new Dgram();
            SocketHandle.SocketData.TCP_UDP_Client.

            

        }

        public class SocketData
        {
            //TCP
            public static ServerData Server;
            public static List<ClientData> TCP_UDP_Client = new List<ClientData>();           

            //other
            public const int Port = 61361;
            public const int MAX_LISTEN_SIZE = 10;
            public const int THREAD_WAIT_TIME = 50;   // msec
            
           
           
            public class ServerData
            {
                //Sokcet
                private Socket TCPServer;
                private Socket UDPServer;
                
                //IPEndPoint
                private IPEndPoint TCPEndPoint;
                private IPEndPoint UDPEndPoint;

                //background Thread
                private Thread Accept_Thread;

                //other
                private int ListenCount = MAX_LISTEN_SIZE;
                public static string ServerIP = "192.168.0.103";
                private bool AcceptRunning = true;


                //*****************************
                //**********initional**********
                //*****************************
                public void InitialUDPServer()
                {
                    UDPServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    UDPEndPoint = new IPEndPoint(IPAddress.Parse(ServerIP), Port);
                    UDPServer.Bind(UDPEndPoint);
                }            

                public void InitialTCPServer()
                {
                    TCPServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    TCPEndPoint = new IPEndPoint(IPAddress.Parse(ServerIP), SocketData.Port);
                    EndPoint _EndPoint = (EndPoint)TCPEndPoint;
                    TCPServer.Bind(_EndPoint);
                    TCPServer.Listen(MAX_LISTEN_SIZE);
                    TCPAcceptInitial();
                }
                private void TCPAcceptInitial()
                {
                    Accept_Thread = new Thread(this.Accept_Thread_Run);
                    Accept_Thread.IsBackground = true;
                    Accept_Thread.Start();

                }
                //******************************
                //**********\initional**********
                //******************************

                //*************************************
                //**********background thread**********
                //*************************************
                private void Accept_Thread_Run()
                {
                    while (AcceptRunning)
                    {
                        if (ListenCount > 0)
                        {
                            Socket _Socket = TCPServer.Accept();
                            MessageBox.Show("Accept");
                            ClientData _Client = new ClientData(_Socket);
                            TCP_UDP_Client.Add(_Client);
                            ListenCount--;
                        }
                        Thread.Sleep(THREAD_WAIT_TIME);
                    }
                }
                //*************************************
                //**********background thread**********
                //*************************************

               
            }

            public class ClientData
            {
                private Socket TCP_Client;
                private Socket UDP_Client;
                private IPEndPoint EP;
                public int ID;               
                private Thread Receive_Thread;

                private bool ReceiveRunning = true;
               
                public ClientData(Socket socket)
                {
                    TCP_Client = socket;
                    ID = socket.GetHashCode();
                    Receive_Thread = new Thread(this.Receive_Thread_Run);
                    Receive_Thread.IsBackground = true;                   
                    Receive_Thread.Start();
                }

                ~ClientData()
                {
                    TCP_Client.Close();
                    this.ReceiveRunning = false;    
                }

                private void Receive_Thread_Run()
                {
                    while (ReceiveRunning)
                    {
                        //send to all online member
                        Dgram _Temp = new Dgram();
                        byte[] _Arr = new byte[RECEIVE_LENGTH];
                        int RecvSize;
                        try
                        {
                            RecvSize = TCP_Client.Receive(_Arr);
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                            break;
                        }

                        _Temp = SocketHandle.ByteToDgram(_Arr);

                        switch(_Temp.Type)
                        {
                            case 0:
                                break;
                            case 1:
                                string _Str = ByteToStr(_Temp.Data, _Temp.DataLength);
                                MessageBox.Show(_Str);


                                break;
                            case 3:
                                break;
                            case 5:
                                break;
                        
                            default:
                                break;
                        }
                    }
                }

              

            }


            
        }
    }




}
