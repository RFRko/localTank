using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Tanki
{
    public class ReceiverUdpClientBased : IReciever,IUdpClient
    {
        private ReceiverUdpClientBased() { }
        public ReceiverUdpClientBased(IPEndPoint withLockalEP)
        {
            LockalEndPoint = withLockalEP;
            NetClient = new UdpClient(LockalEndPoint);
        }
        public IPEndPoint LockalEndPoint { get; private set; }

        public bool Alive { get; set; }

        public UdpClient NetClient { get; }

        public IRecieverClient Owner { get; private set; }

        public INetCommunicationObj GetNetCommunicationObject()
        {
            return this;
        }

        public void Run()
        {
            RecievingThr = new Thread(RecievingProc);
            RecievingThr.Name = "MAIN_RECIEVING_THREAD";
            RecievingThr.Start();
        }

        private bool alive;
        private Thread RecievingThr;

        private void RecievingProc()
        {
            Alive = true;
            //UdpClient Client = new UdpClient(LocalPort);
            IPEndPoint remoteIp = null;
            try
            {
                while (Alive)
                {

                    byte[] data = NetClient.Receive(ref remoteIp);
                    ISerializator obj = new BinSerializator();
                    IPackage p = obj.Deserialize(data);

                    Owner.MessageQueue.Enqueue(p);      //!!!!!!!!!!!!!!!!!!!!!!!
                    //return p;
                }
                //return null;
            }
            catch (ObjectDisposedException)
            {
                //допишу позже
            }
            finally
            {
                NetClient.Close();
            }

        }

        public void OnRegistered_EventHandler(object Sender, RegRecieverData evntData)
        {
            Owner = evntData.owner;
        }

        //private int localport;
        //public bool Alive
        //{
        //    get
        //    {
        //        return this.alive;
        //    }

        //    set
        //    {
        //        this.alive = value;
        //    }
        //}

        //public int LocalPort
        //{
        //    get
        //    {
        //        return this.localport;
        //    }

        //    set
        //    {
        //        this.localport = value;
        //    }
        //}

        //public IPackage Run()
        //{
        //    Alive = true;
        //    UdpClient Client = new UdpClient(LocalPort);
        //    IPEndPoint remoteIp = null;
        //    try
        //    {
        //        while (Alive)
        //        {
        //            byte[] data = Client.Receive(ref remoteIp);
        //            ISerializator obj = new BinSerializator();
        //            IPackage p = obj.Deserialize(data);
        //            return p;
        //        }
        //        return null;
        //    }
        //    catch (ObjectDisposedException)
        //    {
        //        if (!Alive)
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //    finally
        //    {
        //        Client.Close();
        //    }
        //}
    }

    public class RegRecieverData: EventArgs
    {
        public IRecieverClient owner { get; set; }
    }

}
