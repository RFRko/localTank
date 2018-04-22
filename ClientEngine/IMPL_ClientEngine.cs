using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tanki 
{
	public class ClientEngine : EngineAbs, IClientEngine
    {
		public ClientEngine()
		{
			ProcessMessage += ProcessMessageHandler;
			ProcessMessages = null;
			//_Entity = new Tank();
			First_Map = true;
			_ifReadyToSendEntity = new ManualResetEvent(false);
			_ifReadyToSetEntity = new ManualResetEvent(false);
		}

		private IGameClient client;
		private object Map_locker = new object();
		private object Entity_locker = new object();
		private bool First_Map;

		private IEnumerable<IRoomStat> _RoomsStat = null;
		private IMap _Map = null;
		private volatile ITank _Entity = null;
		private string _ErrorText = null;
		private Size _MapSize;
		//private bool st_g = false;
		//private int count = 0;
		//Stopwatch stopWatch = new Stopwatch();

		private Int32 timerSpeed = 40;
        private Timer _timer = null;
		private ManualResetEvent _ifReadyToSendEntity;
		private ManualResetEvent _ifReadyToSetEntity;
		private CancellationToken _timerCancelator = new CancellationToken();
		private CancellationTokenSource _CancelationSource = new CancellationTokenSource();


        public IEnumerable<IRoomStat> RoomsStat
		{
			get { return _RoomsStat;  }

			protected set
			{
                _RoomsStat = value;
                OnRoomsStatChanged?.BeginInvoke(this, new RoomStatChangeData() { newRoomsStat = value }, null, null);
			}
		}
		public IMap Map
		{
			get { lock (Map_locker) { return _Map; } }

			protected set
			{
				lock (Map_locker) { _Map = value; }
				OnMapChanged?.BeginInvoke(this, new GameStateChangeData() { newMap = value }, null, null);
			}
		}
		public ITank Entity
		{
			get { lock(Entity_locker) return _Entity; } 

			set
			{
				//if (start)
				//{                

                    _ifReadyToSetEntity.WaitOne();
                    _ifReadyToSendEntity.Reset();
                        _Entity = value;
                    _ifReadyToSendEntity.Set();

                    //lock (Entity_locker) _Entity = value;
                    //var room_IpEndpoint = client["Room"];
                    //var my_passport = client.Passport;

                    //Owner.Sender.SendMessage(new Package()
                    //{
                    //	Sender_Passport = my_passport,
                    //	Data = value,
                    //	MesseggeType = MesseggeType.Entity
                    //}, room_IpEndpoint);
                //}
			}
		}
		public string ErrorText 
		{
			get { return _ErrorText; }
			protected set
			{
				_ErrorText = value;
				OnError?.Invoke(this, new ErrorData() { errorText = value });
			}
		}
		public Size Map_size
		{
			get { return _MapSize; }
			protected set
			{
				_MapSize = value;
				OnRoomConnect?.BeginInvoke(this, new RoomConnect() { MapSize = value }, null, null);
			}
		}
		public int MaxHealthPoints { get; private set; }


		public void CreateGame(GameSetings gameSetings, string player_name)
		{
			var connectionData = new ConectionData()
			{
				GameSetings = gameSetings,
				PlayerName = player_name
			};

			Owner.Sender.SendMessage(new Package()
			{
				Sender_Passport = client.Passport,
				Data = connectionData,
				MesseggeType = MesseggeType.CreateRoom
			}, client["Host"]);
		}
		public void JOINGame(Guid room_guid, string player_name)
		{
			var connectionData = new ConectionData()
			{
				RoomPasport = room_guid,
				PlayerName = player_name
			};

			Owner.Sender.SendMessage(new Package()
			{
				Sender_Passport = client.Passport,
				Data = connectionData,
				MesseggeType = MesseggeType.RoomID
			}, client["Host"]);
		}
		public void StopGame()
		{
			_CancelationSource.Cancel();
			_timer.Dispose();
			Owner.Sender.SendMessage(new Package()
			{
				Sender_Passport = client.Passport,
				MesseggeType = MesseggeType.RequestLogOff
			}, client["Room"]);
		}
		public void exit()
		{
			StopGame();
			Owner.Sender.SendMessage(new Package()
			{
				Sender_Passport = client.Passport,
				MesseggeType = MesseggeType.RequestLogOff
			}, client["Host"]);
		}

		public Guid GetPassport()
		{
			return client.Passport;
		}
		public void GetRoomList()
		{
			Owner.Sender.SendMessage(new Package()
			{
				Sender_Passport = client.Passport,
				MesseggeType = MesseggeType.GetRoomList
			}, client["Host"]);
		}


		private void SendByTimerCallback(Object data)
		{         

			_ifReadyToSendEntity.WaitOne();
			_ifReadyToSetEntity.Reset();

			var ct = (CancellationToken)data ;
			if (ct.IsCancellationRequested)
			{
				_timer.Change(Timeout.Infinite, Timeout.Infinite);
				return;
			}


			if (_Entity == null)
            {
                _ifReadyToSetEntity.Set();
                return;
            }

            if (_Entity.Position == Rectangle.Empty)
            {
				_ifReadyToSetEntity.Set();
				return;
			}

            var room_IpEndpoint = client["Room"];
			var my_passport = client.Passport;


			Owner.Sender.SendMessage(new Package()
			{
				Sender_Passport = my_passport,
				Data = _Entity,
				MesseggeType = MesseggeType.Entity
			}, room_IpEndpoint);

		_ifReadyToSetEntity.Set();
		}
		private void ProcessMessageHandler(IPackage package)
		{
            client = Owner as IGameClient;
            switch (package.MesseggeType)
			{
				case MesseggeType.Map:
					{
						//if (st_g)  count++;
						//if(stopWatch.Elapsed.Seconds == 10)
						//{
						//	//тут ставь точку и смотри count
						//	stopWatch.Stop();
						//}
						Map = package.Data as IMap;
						if(First_Map)
						{
							_Entity = Map.Tanks.First(i => i.Tank_ID == client.Passport);
							First_Map = false;
							MaxHealthPoints = _Map.Tanks.ElementAt(0).HelthPoints;
						}
						break;
					}
				case MesseggeType.RoomList:
					{
                        RoomsStat = (package.Data as IRoomsStat).RoomsStat;
                        break;
					}
				case MesseggeType.Passport:
					{
                        IinitialConectionData initConnData = package.Data as IinitialConectionData;
                        client.Passport = initConnData.passport;

                        client.AddAddressee("Host", initConnData.manageRoomEndpoint);
						break;
					}
				case MesseggeType.RoomInfo:
					{
						var roomInfo = package.Data as RoomInfo;
						client.AddAddressee("Room", roomInfo.roomEndpoint as IAddresssee);
						Map_size = roomInfo.mapSize;
						break;
					}
				case MesseggeType.TankDeath:
					{
						var tank = package.Data as ITank;
						if (tank.Tank_ID == client.Passport)
						{
							//StopGame();
							onDeath?.BeginInvoke(this,
							new ErrorData(), null, null);
						}
						OnTankDeath?.BeginInvoke(this, new DestroyableTank()
						{ tankToDestroy = tank }, null, null);
						break;
					}
				case MesseggeType.StartGame:
					{
						//st_g = true;
						//stopWatch.Start();
						_ifReadyToSendEntity.Set();
						_timerCancelator = _CancelationSource.Token;
						_timer = new Timer(SendByTimerCallback, _timerCancelator, 0, timerSpeed);
						Thread.Sleep(500);
						onGameStart?.BeginInvoke(this, new ErrorData()
						{ errorText = "" }, null, null);

						break;
					}
				case MesseggeType.EndGame:
					{
						//StopGame();
						onEndGame?.BeginInvoke(this,
							new ErrorData() { errorText = (package?.Data as ITank).Name }, null, null);
						break;
					}
				case MesseggeType.Error:
					{
						ErrorText = package.Data as string;
						break;
					}
				default: throw new Exception("Undefine MessaggeType");
			}
		}

		public event EventHandler<ErrorData> onGameStart;
		public event EventHandler<ErrorData> onEndGame;
		public event EventHandler<ErrorData> onDeath;
		public event EventHandler<RoomStatChangeData> OnRoomsStatChanged; //событие обновления IEnumerable<IRoomStat>
		public event EventHandler<GameStateChangeData> OnMapChanged; //событие обновления IMap
		public event EventHandler<ErrorData> OnError; //сообщение об ошибке
		public event EventHandler<RoomConnect> OnRoomConnect; // подключение к комнате
		public event EventHandler<DestroyableTank> OnTankDeath; // событие уничтожения танка



		public void OnEntityHandler(object Sender, ITank evntData) { Entity = evntData; } //не используется
		public void OnViewCommandHandler(object Sender, object evntData) { } //на всякий случай
		public override ProcessMessageHandler ProcessMessage { get; protected set; } // не нужен, требует EngineAbs 
		public override ProcessMessagesHandler ProcessMessages { get; protected set; } // не нужен, требует EngineAbs
		public override void OnNewAddresssee_Handler(object Sender, NewAddressseeData evntData) { } // не нужен, требует EngineAbs

        public override void OnNetProcStarted_EventHandler(object Sender, NetProcStartedEvntData evntData)
        {
			//nothing to do required yet
		} // не нужен, требует EngineAbs
		public override void OnAddressseeHolderFull_Handler(object Sender, AddressseeHolderFullData evntData)
        {
			//nothing to do required yet
		} // не нужен, требует EngineAbs
		public override void OnBeforNetProcStarted_EventHandler(object Sender, NetProcBeforStartedEvntData evntData)
        {
			//nothing to do required yet
		} // не нужен, требует EngineAbs
	}
}
