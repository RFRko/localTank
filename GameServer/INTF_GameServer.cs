using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Net.Sockets;
using System.Threading;

namespace Tanki
{
    public interface IListener
    {
        IListeningClient Client { get; }
        Socket ipv4_listener { get; }
        Socket ipv6_listener { get; }

        void RUN();
        event EventHandler<NewConnectionData> OnNewConnection;
        void NotifyListenerRegisterHandler(Object Sender, NotifyListenerRegData evntData);
    }

    public interface IListeningClient
    {
        void RegisterListener(IListener listener);
        event EventHandler<NotifyListenerRegData> NotifyListenerRegister;
        void OnNewConnectionHandler(Object Sender, NewConnectionData evntData);
    }

    public interface IServer:IListeningClient
    {
        IListener ServerListner { get; }
        IEnumerable<IRoom> Rooms { get; }
        void RUN();

    }
}
