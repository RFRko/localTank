using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Tanki
{
    public abstract class RoomAbs : IRoom
    {
        private RoomAbs() { }
        public RoomAbs(String id)
        {
            RoomId = id;
            RoomReciever = new Receiver();
            Sender = new Sender();
        }

        private List<IGamer> _gamers = new List<IGamer>();

        public string RoomId { get; set; }        
        public IReceiver RoomReciever { get; protected set; }
        public IMessageQueue MessageQueue { get; protected set; }
        public IEngine Engine { get; protected set; }
        public ISender Sender { get; protected set; }

        public IEnumerable<IGamer> Gamers { get { return _gamers; } }

        public virtual void AddGamer(IGamer newGamer)
        {
            _gamers.Add(newGamer);
        }

        public virtual void RUN()
        {
            if (MessageQueue == null) throw new Exception("MessageQueue object not valid");
            if (Engine == null) throw new Exception("Engine object not valid");

            MessageQueue.RUN();
            RoomReciever.Run();
        }
    }



    public class ManagingRoom : RoomAbs
    {
        public ManagingRoom(string id) : base(id)
        {
            Engine = (new ServerEngineFabric()).CreateEngine(SrvEngineType.srvManageEngine, this);
            MessageQueue = (new MessageQueueFabric()).CreateMessageQueue(MsgQueueType.mqOneByOneProcc, Engine);
        }
    }

    public class GameRoom : RoomAbs
    {
        public GameRoom(string id) : base(id)
        {
            Engine = (new ServerEngineFabric()).CreateEngine(SrvEngineType.srvGameEngine, this);
            MessageQueue = (new MessageQueueFabric()).CreateMessageQueue(MsgQueueType.mqByTimerProcc, Engine);
        }
    }


    public class RoomFabric : IRoomFabric
    {
        public IRoom CreateRoom(String roomId, RoomType roomType)
        {
            IRoom res = null;

            switch (roomType)
            {
                case RoomType.rtMngRoom:
                    res = new ManagingRoom(roomId);
                    break;
                case RoomType.rtGameRoom:
                    res = new GameRoom(roomId);
                    break;
            }
            return res;
        }
    }



}
