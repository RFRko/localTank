using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Tanki
{
    public class Receiver : IReceiver
    {
        private bool alive;
        private int localport;
        public bool Alive
        {
            get
            {
                return this.alive;
            }

            set
            {
                this.alive = value;
            }
        }

        public int LocalPort
        {
            get
            {
                return this.localport;
            }

            set
            {
                this.localport = value;
            }
        }

        public IPackage Run()
        {
            Alive = true;
            UdpClient Client = new UdpClient(LocalPort);
            IPEndPoint remoteIp = null;
            try
            {
                while (Alive)
                {
                    byte[] data = Client.Receive(ref remoteIp);
                    Serializator obj = new Serializator();
                    IPackage p = obj.Deserialize(data);
                    return p;
                }
                return null;
            }
            catch (ObjectDisposedException)
            {
                if (!Alive)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                Client.Close();
            }
        }
    }
}
