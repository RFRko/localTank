using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tanki 
{
	public class ClientEngine : EngineAbs, IClientEngine
    {
		public ClientEngine()
		{
			ProcessMessage += ProcessMessageHandler;
			ProcessMessages = null;
			_Entity = new Tank();
		}

		private IGameClient client;
		private static object Map_locker = new object();
		private static object Entity_locker = new object();

		private IEnumerable<IRoomStat> _RoomsStat = null;
		private IMap _Map = null;
		private ITank _Entity = null;
		private string _ErrorText = null;
		private Size _MapSize;


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
			get { lock (Entity_locker) { return _Entity; } }

			set
			{
				lock (Entity_locker) { _Entity = value; }

				//мы решили отправлять Entity только по таймеру





				//var room_IpEndpoint = (IPEndPoint)client["Room"];
				//var my_passport = client.Passport;

				//Owner.Sender.SendMessage(new Package()
				//{
				//	Sender_Passport = my_passport,
				//	Data = value,
				//	MesseggeType = MesseggeType.Entity
				//}, room_IpEndpoint);
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


		public void OnEntityHandler(object Sender, ITank evntData)
		{
			Entity = evntData;
		}
		private void ProcessMessageHandler(IPackage package)
		{
            client = Owner as IGameClient;
            switch (package.MesseggeType)
			{
				case MesseggeType.Map:
					{
						Map = package.Data as IMap;
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
				case MesseggeType.StartGame:
					{
						client.RUN_GAME();
						break;
					}
				case MesseggeType.EndGame:
					{
						client.END_GAME();
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


		public event EventHandler<RoomStatChangeData> OnRoomsStatChanged; //событие обновления IEnumerable<IRoomStat>
		public event EventHandler<GameStateChangeData> OnMapChanged; //событие обновления IMap
		public event EventHandler<ErrorData> OnError; //сообщение об ошибке
		public event EventHandler<RoomConnect> OnRoomConnect; // подключение к комнате




		public void OnViewCommandHandler(object Sender, object evntData) { } //на всякий случай
		public override ProcessMessageHandler ProcessMessage { get; protected set; } // не нужен, требует EngineAbs 
		public override ProcessMessagesHandler ProcessMessages { get; protected set; } // не нужен, требует EngineAbs
		public override void OnNewAddresssee_Handler(object Sender, NewAddressseeData evntData) { } // не нужен, требует EngineAbs
	}
}
