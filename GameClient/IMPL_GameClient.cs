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
    public class GameClient:NetProcessorAbs, IGameClient
    {
        private Dictionary<string, IAddresssee> adresee_list;       // приватный Dictionary<String, IAddresssee>  для хранения перечня адрессатов
        private TcpClient tcp;
        private IEntity entity;
        private Guid passport;



        //взять этот за основу НУЖЕН НОВЫЙ КОНСТРУКТОР!!!!
        public GameClient(IPEndPoint localEP, IRoomOwner owner) : base(null, localEP, owner)
        {
            this.adresee_list = new Dictionary<string, IAddresssee>();
            tcp = new TcpClient();


            IReciever _Reciever = new ReceiverUdpClientBased(localEP);
            base.RegisterDependcy(_Reciever);
            

            Sender = new SenderUdpClientBased(Reciever);
            
            // Нужно будет прописать создание клиентского Engine
            //IEngine _Engine = (new ServerEngineFabric()).CreateEngine(SrvEngineType.srvManageEngine);
            //base.RegisterDependcy(_Engine);

            IMessageQueue _MessageQueue = (new MessageQueueFabric()).CreateMessageQueue(MsgQueueType.mqOneByOneProcc);
            base.RegisterDependcy(_MessageQueue);

        }

        public void AddAddressee(string Id, IAddresssee addresssee)
        {
            this.adresee_list.Add(Id, addresssee);
        }

        public IAddresssee this[string id]
        {
            get
            {
                if(this.adresee_list[id] != null)
                {
                    return this.adresee_list[id];
                }
                else
                {
                    return null;
                }
            }
        }





        public IEntity ClientGameState
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler<EnforceDrawingData> EnforceDrawing;



        public void OnClientGameStateChangedHandler(object Sender, GameStateChangeData evntData)
        {
            throw new NotImplementedException();
        }

        public void RUN(IPEndPoint ServerEndPoint)
        {
            tcp = new TcpClient(ServerEndPoint);
            base.RUN();
            tcp.Connect(ServerEndPoint.Address, ServerEndPoint.Port);
        }

        public void RUN_GAME()
        {
            throw new NotImplementedException();
        }

        public Guid Passport
        {
            get
            {
                return this.passport;
            }
            set
            {
                this.passport = value;
            }
        }



        // должен быть приватный TCPClient  для коннекта к хосту


        //должен быть приватный Timer - на callBack которого будет вызываться метод переодической отправки клинтского состояния игры на сервер.

        //должен будет быть приватный метод  'void ProceedQueue(Object state)' который будет передаваться time-ру как callback 
        // этот метод должен с периодиностью таймера отправлять клиентское состояние игры на сервер
    }
}
