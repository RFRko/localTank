using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Tanki
{
    public abstract class RoomAbs : NetProcessorAbs, IRoom
    {
        private RoomAbs() { }
        public RoomAbs(String id, IPEndPoint localEP, IRoomOwner owner) :base()
        {
            RoomId = id;
            Owner = owner;
            //Reciever = new ReceiverUdpClientBased(localEP);
            //Sender = new SenderUdpClientBased(Reciever);
        }

        private List<IGamer> _gamers = new List<IGamer>();

        public IRoomOwner Owner { get; protected set; }
        public string RoomId { get; set; }        

        public IEnumerable<IGamer> Gamers { get { return _gamers; } }

        public virtual void AddGamer(IGamer newGamer)
        {
            _gamers.Add(newGamer);
        }

        public virtual new void RUN()
        {
            base.RUN();
            //if (MessageQueue == null) throw new Exception("MessageQueue object not valid");
            //if (Engine == null) throw new Exception("Engine object not valid");

            //MessageQueue.RUN();
            //Reciever.Run();
        }

        public IEnumerable<IGamer> GetAddresssees() {return _gamers;}

        public IRoomStat getRoomStat()
        {
            return new RoomStat() { Id = this.RoomId, Players_count = Gamers.Count(), Creator_Id = "will be later" };
        }

        public IAddresssee this[string id] {
            get { var v = from g in _gamers where g.id == id select g;
                if (v.Count() > 1) throw new Exception("not unique") ;
                return v.First();
            } }
        public event EventHandler<NewAddressseeData> OnNewAddresssee;
    }



    public class ManagingRoom : RoomAbs, IManagerRoom
    {
        public ManagingRoom(string id, IPEndPoint localEP, IRoomOwner owner) : base(id, localEP, owner)
        {
            IReciever _Reciever = new ReceiverUdpClientBased(localEP);
            base.RegisterDependcy(_Reciever);

            Sender = new SenderUdpClientBased(Reciever);

            IEngine _Engine = (new ServerEngineFabric()).CreateEngine(SrvEngineType.srvManageEngine);
            base.RegisterDependcy(_Engine);

            IMessageQueue _MessageQueue = (new MessageQueueFabric()).CreateMessageQueue(MsgQueueType.mqOneByOneProcc);
            base.RegisterDependcy(_MessageQueue);

        }

        public IEnumerable<IRoomStat> getRoomsStat()
        {
            IManagerRoomOwner mO = Owner as IManagerRoomOwner;
            return mO.getRoomsStat();
        }

        public IRoomStat getRoomStat(String forRoomID)
        {
            IManagerRoomOwner mO = Owner as IManagerRoomOwner;
            return mO.getRoomStat(forRoomID);
        }

        public void MooveGamerToRoom(IGamer gamer, string TargetRoomId)
        {
            IManagerRoomOwner mO = Owner as IManagerRoomOwner;
            mO.MooveGamerToRoom(gamer,TargetRoomId);
        }
    }

    public class GameRoom : RoomAbs
    {
        public GameRoom(string id, IPEndPoint localEP, IRoomOwner owner) : base(id, localEP, owner)
        {
            Reciever = new ReceiverUdpClientBased(localEP);
            base.RegisterDependcy(Reciever);

            Sender = new SenderUdpClientBased(Reciever);

            Engine = (new ServerEngineFabric()).CreateEngine(SrvEngineType.srvGameEngine);
            base.RegisterDependcy(Engine);

            MessageQueue = (new MessageQueueFabric()).CreateMessageQueue(MsgQueueType.mqByTimerProcc);
            base.RegisterDependcy(MessageQueue);

        }
    }


    public class RoomFabric : IRoomFabric
    {
        public IRoom CreateRoom(String roomId, IPEndPoint localEP, RoomType roomType, IRoomOwner owner)
        {
            IRoom res = null;

            switch (roomType)
            {
                case RoomType.rtMngRoom:
                    res = new ManagingRoom(roomId, localEP, owner);
                    break;
                case RoomType.rtGameRoom:
                    res = new GameRoom(roomId, localEP, owner);
                    break;
            }
            return res;
        }
    }



}
