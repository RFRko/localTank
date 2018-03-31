using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tanki;

namespace Tanki
{
    public class Sender : ISender
    {
        private string remoteaddress;
        private int remoteport;
        private IPackage pack;

        public IPackage Pack
        {
            get
            {
                return this.pack;
            }

            set
            {
                this.pack = value;
            }
        }

        public string RemoteAdress
        {
            get
            {
                return this.remoteaddress;
            }

            set
            {
                this.remoteaddress = value;
            }
        }

        public int RemotePort
        {
            get
            {
                return this.remoteport;
            }

            set
            {
                this.remoteport = value;
            }
        }

        public void SendMessage()
        {
            UdpClient sender = new UdpClient(); // создаем клиента для отпраки сообщений на хост
            try
            {
                while(true)
                {
                    Serializator obj = new Serializator();
                    byte[] data = obj.Serialize(Pack);
                    sender.Send(data, data.Length, RemoteAdress, RemotePort);   // отправка пакета
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sender.Close();                 //закрываем соединение
            }
        }

        Sender(string ra, int rp, IPackage p)
        {
            this.RemoteAdress = ra;
            this.RemotePort = rp;
            this.Pack = p;
        }
    }
}
