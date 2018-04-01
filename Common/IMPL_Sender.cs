using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tanki;

namespace Tanki
{
    public class SenderUdpClientBased : ISender,IUdpClient
    {
        //Sender(string ra, int rp, IPackage p)
        //{
        //    this.RemoteAdress = ra;
        //    this.RemotePort = rp;
        //    this.Pack = p;
        //}

        //public Sender() { }

        public SenderUdpClientBased(INetCommunicationObj withCommunicationObj)
        {
            IUdpClient cl = withCommunicationObj as IUdpClient;
            if (cl == null) throw new Exception("Invalid constructor parameter..");
            _lockal_udp_client = cl.NetClient;
        }

        public SenderUdpClientBased(UdpClient withLocalClient)
        {
            _lockal_udp_client = withLocalClient;
        }

        private UdpClient _lockal_udp_client;

        public UdpClient NetClient { get; private set; }

        #region propose2del
        //private string remoteaddress;
        //private int remoteport;
        //private IPackage pack;

        //public IPackage Pack
        //{
        //    get
        //    {
        //        return this.pack;
        //    }

        //    set
        //    {
        //        this.pack = value;
        //    }
        //}

        //public string RemoteAdress
        //{
        //    get
        //    {
        //        return this.remoteaddress;
        //    }

        //    set
        //    {
        //        this.remoteaddress = value;
        //    }
        //}

        //public int RemotePort
        //{
        //    get
        //    {
        //        return this.remoteport;
        //    }

        //    set
        //    {
        //        this.remoteport = value;
        //    }
        //}

        //public void SendMessage()
        //{
        //    UdpClient sender = new UdpClient(); // создаем клиента для отпраки сообщений на хост
        //    try
        //    {
        //        while(true)
        //        {
        //            ISerializator obj = new BinSerializator();
        //            byte[] data = obj.Serialize(Pack);
        //            sender.Send(data, data.Length, RemoteAdress, RemotePort);   // отправка пакета
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageBox.Show(ex.Message);
        //    }
        //    finally
        //    {
        //        sender.Close();                 //закрываем соединение
        //    }
        //}

        #endregion  propose2del

        private Byte[] get_msg_bytes(IPackage msg)
        {
            ISerializator srls = new BinSerializator();
            return srls.Serialize(msg);
        }

        public void SendMessage(IPackage msg, IPEndPoint Target)
        {
            Byte[] msg_bytes = get_msg_bytes(msg);
            _lockal_udp_client.Send(msg_bytes,msg_bytes.Length, Target);
        }

        public void SendMessage(IPackage msg, IEnumerable<IPEndPoint> Targets)
        {
            Byte[] msg_bytes = get_msg_bytes(msg);
            Parallel.ForEach(Targets,
                (currentTagret, loopState, index) => { _lockal_udp_client.Send(msg_bytes, msg_bytes.Length, currentTagret); }
             );
        }

        public void SendMessage(IPackage msg, IAddresssee Target)
        {
            Byte[] msg_bytes = get_msg_bytes(msg);
            _lockal_udp_client.Send(msg_bytes, msg_bytes.Length, Target.RemoteEndPoint);

        }

        public void SendMessage(IPackage msg, IEnumerable<IAddresssee> Targets)
        {
            Byte[] msg_bytes = get_msg_bytes(msg);
            Parallel.ForEach(Targets,
                (currentTagret, loopState, index) => { _lockal_udp_client.Send(msg_bytes, msg_bytes.Length, currentTagret.RemoteEndPoint); }
             );
        }

    }
}
