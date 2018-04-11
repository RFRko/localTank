using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tanki
{

    public abstract class ListeningClientAbs : IListeningClient
    {
        public event EventHandler<NotifyListenerRegData> NotifyListenerRegister;
        public void RegisterListener(IListener listener)
        {
            listener.OnNewConnection += OnNewConnectionHandler;
            NotifyListenerRegister += listener.NotifyListenerRegisterHandler;

            NotifyListenerRegister?.Invoke(this, new NotifyListenerRegData() { Client = this });
        }

        public abstract void OnNewConnectionHandler(object Sender, NewConnectionData evntData);

    }

    public class Listener : IListener
    {
        public Listener(IIpEPprovider ipEPprovider, Int32 Port)
        {
            IPHostEntry HostEntry = Dns.GetHostEntry(Dns.GetHostName());

            //IPAddress ipv4Addr = HostEntry.AddressList[2];
            //IPAddress ipv6Addr = HostEntry.AddressList[0];

            //IPEndPoint ipv4EP = new IPEndPoint(ipv4Addr, Port);
            //IPEndPoint ipv6EP = new IPEndPoint(ipv6Addr, Port);

            IPEndPoint ipv4EP = ipEPprovider.CreateIPEndPoint(AddressFamily.InterNetwork, Port);
            IPEndPoint ipv6EP = ipEPprovider.CreateIPEndPoint(AddressFamily.InterNetworkV6, Port);

            if (ipv4EP != null)
            {
                ipv4_listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                ipv4_listener.Bind(ipv4EP);
            }

            if (ipv6EP != null)
            {
                ipv6_listener = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                ipv6_listener.Bind(ipv6EP);
            }


        }
        public Socket ipv4_listener { get; protected set; }
        public Socket ipv6_listener { get; protected set; }

        protected ManualResetEvent Listening = new ManualResetEvent(false);

        public IListeningClient Client { get; protected set; }
        public event EventHandler<NewConnectionData> OnNewConnection;

        public void NotifyListenerRegisterHandler(Object Sender, NotifyListenerRegData evntData)
        {
            Client = evntData.Client;
        }

        public void RUN()
        {
            Int32 startedListeningThreads = 0;

            if (Client == null) throw new Exception("lisnening Clien of type IListeningClient not registered");

            ParameterizedThreadStart StartWithParam = new ParameterizedThreadStart(StartListening);
            Thread sThr = new Thread(StartWithParam);

            if (ipv4_listener != null)
            {
                sThr.Name = "LISTENING_" + ipv4_listener.LocalEndPoint.ToString();
                sThr.Start(ipv4_listener);
                startedListeningThreads++;
            }

            StartWithParam = new ParameterizedThreadStart(StartListening);
            sThr = new Thread(StartWithParam);

            if (ipv6_listener != null)
            {
                sThr.Name = "LISTENING_" + ipv6_listener.LocalEndPoint.ToString();
                sThr.Start(ipv6_listener);
                startedListeningThreads++;
            }


            if (startedListeningThreads == 0) throw new Exception("No listening threads started");
        }


        private void StartListening(Object listenerSoket)
        {
            Byte[] buffer = new Byte[1024];

            Socket ListenerSoket = listenerSoket as Socket;
            try
            {
                ListenerSoket.Listen(100); // на 100 подключений

                while (true)
                {
                    Listening.Reset(); //Ставим событие в несигнальное состояние
                    ListenerSoket.BeginAccept(OnListenCallBack, ListenerSoket);  //ожидание начинается в другом потоке
                    Listening.WaitOne(); // начинаем ожидать.. пока в потоке где выполняется прослушивание собыетие не перейдет в сигнальное состояние Listening.Set()
                }

            }
            catch (Exception ex) {}

            ListenerSoket.Shutdown(SocketShutdown.Both);
            ListenerSoket.Close();
        }

        void OnListenCallBack(IAsyncResult ar)
        {
            Listening.Set(); //устанавливаем в сигнальное состояние, чтобы Listening.Wait - мог пойти дальше..
            Socket listeningSocket = (Socket)ar.AsyncState;
            Socket remoteClientSocket = listeningSocket.EndAccept(ar);

            this.OnNewConnection?.Invoke(this, new NewConnectionData() { RemoteClientSocket = remoteClientSocket});
        }

    }


    public class NewConnectionData: EventArgs
    {
        public Socket RemoteClientSocket { get; set; }
    }

    public class NotifyListenerRegData: EventArgs
    {
        public IListeningClient Client { get; set; }
    }
}
