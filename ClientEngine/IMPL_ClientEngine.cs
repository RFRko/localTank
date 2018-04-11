using System;
using System.Collections.Generic;
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
			client = Owner as IGameClient;
		}


		private IGameClient client;
		private object Map_locker;
		private object Entity_locker;

		public IEnumerable<IRoomStat> RoomsStat
		{
			get { return RoomsStat;  }

			protected set
			{
				RoomsStat = value;
				OnRoomsStatChanged?.Invoke(this, new RoomStatChangeData() { newRoomsStat = value });
			}
		}
		public IMap Map
		{
			get { lock (Map_locker) { return Map; } }

			protected set
			{
				lock (Map_locker) { Map = value; }
				OnMapChanged?.Invoke(this, new GameStateChangeData() { newMap = Map });
			}
		}
		public IEntity Entity
		{
			get { lock (Entity_locker) { return Entity; } }

			protected set
			{
				lock (Entity_locker) { Entity = value; }

				var room_IpEndpoint = (IPEndPoint)client["Room"];
				var my_passport = client.Passport;

				Owner.Sender.SendMessage(new Package()
				{
					Sender_Passport = my_passport,
					Data = value,
					MesseggeType = MesseggeType.Entity
				}, room_IpEndpoint);
			}
		}
		public string RoomError 
		{
			get { return RoomError; }
			protected set
			{
				RoomError = value;
				OnError?.Invoke(this, new ErrorData() { errorText = value });
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
		public void GetRoomList()
		{
			Owner.Sender.SendMessage(new Package()
			{
				Sender_Passport = client.Passport,
				MesseggeType = MesseggeType.GetRoomList
			}, client["Host"]);
		}


		public void OnEntityHandler(object Sender, IEntity evntData)
		{
			Entity = evntData;
		}
		private void ProcessMessageHandler(IPackage package)
		{
            switch (package.MesseggeType)
			{
				case MesseggeType.Map:
					{
						Map = package.Data as IMap;
						break;
					}
				case MesseggeType.RoomList:
					{
						RoomsStat = package.Data as IEnumerable<IRoomStat>;
						break;
					}
				case MesseggeType.Passport:
					{
						client.Passport = (Guid)package.Data;
						break;
					}
				case MesseggeType.RoomEndpoint:
					{
						client.AddAddressee("Room", package.Data as IAddresssee);
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
						RoomError = package.Data as string;
						break;
					}
				default: throw new Exception("Undefine MessaggeType");
			}
		}


		public event EventHandler<RoomStatChangeData> OnRoomsStatChanged; //событие обновления IEnumerable<IRoomStat>
		public event EventHandler<GameStateChangeData> OnMapChanged; //событие обновления IMap
		public event EventHandler<ErrorData> OnError; //сообщение об ошибке




		public void OnViewCommandHandler(object Sender, object evntData) { } //на всякий случай
		public override ProcessMessageHandler ProcessMessage { get; protected set; } // не нужен, требует EngineAbs 
		public override ProcessMessagesHandler ProcessMessages { get; protected set; } // не нужен, требует EngineAbs
		public override void OnNewAddresssee_Handler(object Sender, NewAddressseeData evntData) { } // не нужен, требует EngineAbs
	}
}
