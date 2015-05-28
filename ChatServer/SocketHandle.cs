using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ChatServer
{
    public class SocketHandle
    {
        public class Dgram
        {
            public const int MAX_DATASIZE = 2048;
            public int Size;
            public byte[] Data = new byte[MAX_DATASIZE];

        }
        public void SendPicture(Socket des,Bitmap bmp)
        {
            List<byte[]> _Data = DivideBitmap(bmp);


        }

        private List<byte[]> DivideBitmap(Bitmap bmp)
        {
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            byte[] _Arr = ms.ToArray();
            int index = 0;
            List<byte[]> _Ret = new List<byte[]>();
            while(true)
            {
                if (index + Dgram.MAX_DATASIZE < _Arr.Length)
                {
                    byte[] _Temp = new byte[Dgram.MAX_DATASIZE];
                    Array.Copy(_Arr, index, _Temp, 0, Dgram.MAX_DATASIZE);
                    _Ret.Add(_Temp);

                    index += Dgram.MAX_DATASIZE;
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

    }

    public class SocketData
    {
        public static Socket UDPServer;
        public static IPEndPoint TCPEndPoint;
        public static Socket TCPServer;
        public static List<Socket> TCPClient;
        public const int Port = 61361;  
        public const int MAX_LISTEN_SIZE = 10;
        
        private 
        
        public static void InitialServer()
        {
           

        }

        public static void InitialTCPServer()
        {
            TCPServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            TCPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), SocketData.Port);
            EndPoint _EndPoint = (EndPoint)TCPEndPoint;
            TCPServer.Bind(_EndPoint);
            TCPServer.Listen(MAX_LISTEN_SIZE);
        }

        private void Listen()
        {

        }

    }
   

}
