using Common;
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
            IPAddress roomAddr = ((IPEndPoint)ServerListner.ipv6_listener.LocalEndPoint).Address;
            Int32 roomPort = 50001;
            IPEndPoint roomEP = new IPEndPoint(roomAddr, roomPort);

            IRoom managerRoom = (new RoomFabric()).CreateRoom("", roomEP, RoomType.rtMngRoom,this);
            _rooms.Add(managerRoom);
            managerRoom.RUN();

            ServerListner.RUN();
        }

        public IEnumerable<IRoomStat> getRoomsStat()
        {
            var rSts = from r in Rooms select new RoomStat(){ Pasport = r.Passport, Players_count = r.Gamers.Count(), Creator_Pasport = r.CreatorPassport};
            return rSts;
        }

        public IPEndPoint MooveGamerToRoom(IGamer gamer, Guid TargetRoomId)
        {
            var selRoom = from r in Rooms where r.Passport == TargetRoomId select r;
            selRoom.First().AddGamer(gamer);
            return selRoom.First().Reciever.LockalEndPoint;
        }

        public IRoomStat getRoomStat(String RoomID)
        {
            var selRooms = from r in Rooms where r.RoomId == RoomID select r;
            IRoom selRoom = selRooms.First();

            return new RoomStat() {  Pasport = selRoom.Passport, Players_count = selRoom.Gamers.Count(), Creator_Pasport = selRoom.CreatorPassport };
        }

        public IRoom GetRoomByGuid(Guid roomGuid)
        {
            IRoom foundRoom = null;

            var r = (from R in Rooms where R.Passport == roomGuid select  R);            
            if (r.Count() > 1) throw new Exception("Rooms ID not unique");

            foundRoom = r.FirstOrDefault();

            return foundRoom;
        }
    }
}
