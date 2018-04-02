using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tanki
{
    public class GameServer: ListeningClientAbs,  IServer
    {
        private GameServer() { }

        public GameServer(IListener listener)
        {
            ServerListner = listener;
            RegisterListener(ServerListner);
        }

        private List<IRoom> _rooms = new List<IRoom>();

        public IListener ServerListner { get; private set; }
        public IEnumerable<IRoom> Rooms { get { return _rooms; } }

        public override void OnNewConnectionHandler(object Sender, NewConnectionData evntData)
        {
            var remoteEP = (IPEndPoint)evntData.RemoteClientSocket.RemoteEndPoint;
            Gamer newGamer = new Gamer(remoteEP);           
            _rooms[0].AddGamer(newGamer);

            // у Room будет событие OnNewGamer
            // для Управляющей комнаты по этому событию будет отправка клиенту IPackage.Data = Gamer.GUID
        }

        public void RUN()
        {
            //

            IPAddress roomAddr = ((IPEndPoint)ServerListner.ipv6_listener.LocalEndPoint).Address;
            Int32 roomPort = 50001;
            IPEndPoint roomEP = new IPEndPoint(roomAddr, roomPort);

            IRoom managerRoom = (new RoomFabric()).CreateRoom("", roomEP, RoomType.rtMngRoom);
            _rooms.Add(managerRoom);
            managerRoom.RUN();

            ServerListner.RUN();
        }
    }
}
