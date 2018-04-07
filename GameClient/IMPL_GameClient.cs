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
    public class GameClient:NetProcessorAbs, IClient, IGameClient
    {
        private Dictionary<string, IAddresssee> adresee_list;       // приватный Dictionary<String, IAddresssee>  для хранения перечня адрессатов
        private TcpClient tcp;                                      // должен быть приватный TCPClient  для коннекта к хосту
        private IEntity clientGameState;
        private int miliseconds;
        private Guid Passport;
        private TimerCallback tm;                                   //должен быть приватный Timer - на callBack которого будет вызываться метод переодической отправки клинтского состояния игры на сервер.
        private IPackage package;
        private IPEndPoint endpoint;

        public event EventHandler<EnforceDrawingData> EnforceDrawing;

        //взять этот за основу НУЖЕН НОВЫЙ КОНСТРУКТОР!!!!
        public GameClient(IPEndPoint localEP, IRoomOwner owner) 
        {
            this.adresee_list = new Dictionary<string, IAddresssee>();
            this.tcp = new TcpClient();
            this.package = new Package();
            IReciever _Reciever = new ReceiverUdpClientBased(localEP);
            base.RegisterDependcy(_Reciever);
            base.Sender = new SenderUdpClientBased(Reciever);


            // Нужно будет прописать создание клиентского Engine
            //IEngine _Engine = (new ServerEngineFabric()).CreateEngine(SrvEngineType.srvManageEngine);
            //base.RegisterDependcy(_Engine);

            //entity = _engine.Entity;

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
                return this.clientGameState;
            }

            set
            {
                this.clientGameState = value;
            }
        }


        public int MiliSeconds
        {
            get
            {
                return this.miliseconds;
            }
            set
            {
                this.miliseconds = value;
            }
        }

        Guid IGameClient.Passport { get; set ; }

        public void RUN(IPEndPoint ServerEndPoint)                  // запускает базовый NetProcessorAbs.RUN (очередь\reciver), коннектится к cерверу
        {
            base.RUN();
        }

        

        public void RUN_GAME()                                     // запускает таймер переодической отправки клиентского состоянения игры на сервер
        {
            int num = 0;
            this.tm = new TimerCallback(ProceedQueue);
            Timer timer = new Timer(tm, num, 0, this.MiliSeconds);
        }

        private void ProceedQueue(object state)          //должен будет быть приватный метод  'void ProceedQueue(Object state)' который будет передаваться time-ру как callback 
        {                                                           // этот метод должен с периодиностью таймера отправлять клиентское состояние игры на сервер    

            var e = Engine as IClientEngine;

            package = new Package()
            {
                Sender_Passport = Passport,
                Data = e.Entity,
                MesseggeType = MesseggeType.Entity
            };

            this.clientGameState = (IEntity)state;
            // отправка данных
            //this.package.Data = clientGameState;
            Sender.SendMessage(this.package, adresee_list["Room"]);
            
        }

        public void Connect(IPEndPoint ServerEndPoint)
        {
            tcp.Connect(ServerEndPoint.Address, ServerEndPoint.Port);
        }

        public void END_GAME()
        {
            throw new NotImplementedException();
        }

        public void OnClientGameStateChangedHandler(object Sender, GameStateChangeData evntData)
        {
            throw new NotImplementedException();
        }
    }
}
