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
using ChatServer;
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
            public int NameLength;
            public int Type;
            //0: check alive, client <-> server
            //1: TCP Text Data, client -> server 
            //2: TCP Text Data, server -> client 
            //3: TCP UDP Image Size, follow the UDP Image, client -> server 
            //4: TCP UDP Image Size, follow the UDP Image, server -> client
            //5: UDP Image, client -> server
            //6: UDP Image, server -> client
            //7: TCP send Name client -> server
            //20: TCP System message server -> client
          


        }

        public const int MAX_DATASIZE = 2048;
        public const int STRING_SIZE = 512;
      /*  public const int DGRAM_SIZE = sizeof(byte) * MAX_DATASIZE
                                    + sizeof(byte) * STRING_SIZE
                                    + sizeof(int)
                                    + sizeof(int)
                                    + sizeof(int);
        */
        public const int RECEIVE_LENGTH = 51200;

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

        private static Dgram ByteToDgram(byte[] _Arr,int _Len)
        {
            MemoryStream _Ms = new MemoryStream(_Arr, 0, _Len);
            XmlSerializer _Xs = new XmlSerializer(typeof(Dgram));
          //  MessageBox.Show(_Ms.ToArray().Length.ToString());
            
            using (FileStream fs = new FileStream("123.xml", FileMode.Create))
            {
                fs.Write(_Ms.ToArray(), 0, (int)_Ms.Length);
                fs.Close();
            }
            


            Dgram _Ret = (Dgram)_Xs.Deserialize(_Ms); 
            return _Ret;
        }

        public static void ReSendBitmap(Dgram N,Socket socket)
        {
            List<Dgram> _Sav = new List<Dgram>();
            int _N = Convert.ToInt32(ByteToStr(N.Data, N.DataLength));

           //recieve
            for(int i=0;i!=_N;++i)
            {
                byte[] _Arr = new byte[RECEIVE_LENGTH];
                byte[] _Len = new byte[4];
                socket.Receive(_Len, 4, SocketFlags.None);
                int recv = socket.Receive(_Arr, BitConverter.ToInt32(_Len, 0), SocketFlags.None);

                _Sav.Add(ByteToDgram(_Arr, recv));
            }
           
            //resend
            N.Type = 4;
            SendToAllClient(N, false);

            foreach (Dgram ite in _Sav)
            {
                ite.Type = 6;
                SendToAllClient(ite, true);
            }
         //   Bitmap bmp = RevertBitmap(_Sav);


           // Form_Server.pic.Image = bmp;


        }

        public static Bitmap RevertBitmap(List<Dgram> _List)
        {
            MemoryStream _Ms = new MemoryStream();

            foreach (Dgram ite in _List)
            {                
                _Ms.Write(ite.Data, 0, ite.DataLength);
            }

            Bitmap _Ret = new Bitmap(_Ms);

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
           // SocketData.Server.InitialUDPServer();
        }

        private static byte[] StrToByte(string _Str)
        {
            return Encoding.Unicode.GetBytes(_Str);
        }

        private static string ByteToStr(byte[] _Arr,int Len)
        {           
            return Encoding.Unicode.GetString(_Arr, 0, Len);
        }

        public static void SendToAllClient(Dgram _Dg,bool sendLen)
        {
            foreach (SocketData.ClientData i in SocketData.TCP_UDP_Client)
            {
                if (i.ReceiveRunning)
                {
                    i.TCPSend(_Dg);
                }
            }
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
                private void InitialUDPServer()
                {
                    UDPServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    UDPEndPoint = new IPEndPoint(IPAddress.Parse(ServerIP), 0);
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
                            IPEndPoint _EP = _Socket.RemoteEndPoint as IPEndPoint;
                            Form_Server.AddText(_EP.Address.ToString(), "-1", "Accept", "Successed");
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

                //*******************************
                //**********UDP recieve**********
                //*******************************

                public List<Dgram> UDP_Recieve(int N)
                {
                    List<Dgram> _Ret = new List<Dgram>();
                    InitialUDPServer();
                    EndPoint _EP = (EndPoint)UDPEndPoint;
                    int RecvSize;
                    for(int i=0;i!=N;++i)
                    {
                        byte[] _Arr = new byte[RECEIVE_LENGTH];
                        RecvSize = UDPServer.ReceiveFrom(_Arr, ref _EP);
                        MessageBox.Show(RecvSize.ToString());
                        Dgram _DG = SocketHandle.ByteToDgram(_Arr, RecvSize);
                        _Ret.Add(_DG);
                    }
                    UDPServer.Close();

                    return _Ret;
                }

                //********************************
                //**********\UDP recieve**********
                //********************************
              
            }

            public class ClientData
            {
                private Socket TCP_Client;
                private Socket UDP_Client;
                private IPEndPoint EP;
                
                public int ID;
                public string Name;
                
                private Thread Receive_Thread;
                

                public bool ReceiveRunning = true;
               
                public ClientData(Socket socket)
                {
                    TCP_Client = socket;
                    ID = socket.GetHashCode();
                    Receive_Thread = new Thread(this.Receive_Thread_Run);
                    Receive_Thread.IsBackground = true;                   
                    Receive_Thread.Start();

                    EP = (IPEndPoint)socket.RemoteEndPoint;
                }

                ~ClientData()
                {
                    TCP_Client.Close();
                    this.ReceiveRunning = false;    
                }

                public void TCPSend(Dgram _Dgram)
                {
                    byte[] _Arr = SocketHandle.DgramToByte(_Dgram);
                    
                    try
                    {                        
                        TCP_Client.Send(BitConverter.GetBytes(_Arr.Length), 4, SocketFlags.None);
                        TCP_Client.Send(_Arr, _Arr.Length, SocketFlags.None);
                    }
                    catch (Exception ex)
                    {
                        Form_Server.AddText(this.Name, this.ID.ToString(), "Except", ex.ToString());
                        this.ReceiveRunning = false;
                    }

                }
                
                private void UDP_Send(List<Dgram> _List,EndPoint _EP)
                {
                    UDP_Client = new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);

                    foreach(Dgram ite in _List)
                    {
                        byte[] _Arr = DgramToByte(ite);
                        UDP_Client.SendTo(_Arr, _EP);
                    }

                    UDP_Client.Close();
                    

                }

                private void Receive_Thread_Run()
                {
                    while (ReceiveRunning)
                    {
                        //send to all online member
                        Dgram _Temp = new Dgram();
                        byte[] _Arr = new byte[RECEIVE_LENGTH];
                        byte[] _Len = new byte[4];
                        int RecvSize;
                        try
                        {
                            TCP_Client.Receive(_Len, 4, SocketFlags.None);
                            RecvSize = TCP_Client.Receive(_Arr, BitConverter.ToInt32(_Len, 0), SocketFlags.None);
                        }
                        catch(Exception ex)
                        {
                            Form_Server.AddText(Name, ID.ToString(), "Except", ex.ToString());
                            break;
                        }

                        _Temp = SocketHandle.ByteToDgram(_Arr, RecvSize);
                        switch(_Temp.Type)
                        {
                            case 0:
                                break;
                            case 1:
                                string _Str = ByteToStr(_Temp.Data, _Temp.DataLength);
                                Form_Server.AddText(Name, ID.ToString(), "Recieve", _Str);
                                _Temp.Type = 2;
                                SocketHandle.SendToAllClient(_Temp, false);

                                break;
                            case 3:
                              //  MessageBox.Show(_Temp.Type.ToString());                    
                              // int _N = Convert.ToInt32(ByteToStr(_Temp.Data, _Temp.DataLength));
                                //MessageBox.Show();
                                SocketHandle.ReSendBitmap(_Temp, this.TCP_Client);
                                break;
                            case 5:
                                break;
                            case 7:
                                string _Str2 = ByteToStr(_Temp.Name, _Temp.NameLength);
                                this.Name = _Str2;
                                Form_Server.AddText(Name, ID.ToString(), "Name", "Recieve Name.");
                                _Temp.Type = 20;
                                SocketHandle.SendToAllClient(_Temp, false);
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
