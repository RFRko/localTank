using System;
using System.Collections.Generic;
using System.Linq;
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
        public Listener() { }
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

            if (Client == null) throw new Exception("lisnening Clien of type IListeningClient not registered");

            ParameterizedThreadStart StartWithParam = new ParameterizedThreadStart(StartListening);
            Thread sThr = new Thread(StartWithParam);
            sThr.Name = "LISTENING_" + ipv4_listener.LocalEndPoint.ToString();
            sThr.Start(ipv4_listener);

            StartWithParam = new ParameterizedThreadStart(StartListening);
            sThr = new Thread(StartWithParam);
            sThr.Name = "LISTENING_" + ipv6_listener.LocalEndPoint.ToString();
            sThr.Start(ipv6_listener);

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
