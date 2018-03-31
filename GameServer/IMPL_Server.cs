using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanki
{
    class GameServer: ListeningClientAbs,  IServer
    {
        private GameServer() { }

        private GameServer(IListener listener)
        {
            ServerListner = listener;
            RegisterListener(ServerListner);
        }

        private List<IRoom> _rooms = new List<IRoom>();

        public IListener ServerListner { get; private set; }
        public IEnumerable<IRoom> Rooms { get { return _rooms; } }

        public override void OnNewConnectionHandler(object Sender, NewConnectionData evntData)
        {
            throw new NotImplementedException();
        }

        public void RUN()
        {
            IRoom managerRoom = (new RoomFabric()).CreateRoom("", RoomType.rtMngRoom);
            _rooms.Add(managerRoom);
            managerRoom.RUN();

            ServerListner.RUN();
        }
    }
}
