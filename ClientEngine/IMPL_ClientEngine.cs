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
		public override ProcessMessageHandler ProcessMessage { get; protected set; }
		public override ProcessMessagesHandler ProcessMessages { get; protected set; }

        public IEnumerable<IRoomStat> RoomsStat
		{
			get { return RoomsStat; }
			protected set
			{
				OnRoomsStatChanged?.Invoke(this, new RoomStatChangeData() { RoomsStat = value });
			}
		}
        public IMap Map { get; protected set; }
        public IEntity Entity
		{
			get { return Entity; }
			protected set
			{
				lock (locker)
				{
					Entity = value;
				}
				IPEndPoint room_IpEndpoint = Owner.adresee_list["Room"];
				Guid my_passport = Owner.Passport;
				Owner.Sender.SendMessage(new Package()
				{
					Sender_Passport = my_passport,
					Data = value,
					MesseggeType = MesseggeType.Entity
				}, room_IpEndpoint);
			}
		}
		public string RoomError { get { return _roomError; } protected set { SetRoomError(value);  } }

		//private IGameEngineOwner Owner;
		//private List<IRoomStat> _RoomsStat = new List<IRoomStat>();
		//private IEntity _entity;
		//private string _roomError = "";

		private object locker;


        public event EventHandler<RoomStatChangeData> OnRoomsStatChanged;
        public event EventHandler<IMap> OnMapChanged;
		public event EventHandler<string> OnRoomError;

		public ClientEngine()
		{
			ProcessMessage += ProcessMessageHandler;
			ProcessMessages = null;
		}
        public ClientEngine(IGameEngineOwner owner)
        {
            ProcessMessage += ProcessMessageHandler;
            ProcessMessages = null;
            Owner = owner;
        }


		private void SetEntity(IEntity entity)
		{
			lock (locker)
			{
				_entity = entity;
			}
			IPEndPoint room_IpEndpoint = Owner.adresee_list["Room"];
			Guid my_passport = Owner.Passport;
			Owner.Sender.SendMessage(new Package()
			{
				Sender_Passport = my_passport,
				Data = entity,
				MesseggeType = MesseggeType.Entity
			}, room_IpEndpoint);
		}
        private void SetRoomsStat(List<IRoomStat> newVal)
        {
			lock (locker)
			{
				_RoomsStat = newVal;
			}
            OnRoomsStatChanged?.Invoke(this, new RoomStatChangeData() { RoomsStat = _RoomsStat });
        }
		private void SetRoomError(string text)
		{
			lock (locker)
			{
				_roomError = text;
			}
			OnRoomsStatChanged?.Invoke(this, new RoomStatChangeData() { RoomsStat = _RoomsStat });
		}

        private void ProcessMessageHandler(IPackage msg)
		{
            switch (msg.MesseggeType)
			{
				case MesseggeType.Map:
					{
						SetMap(msg);
						break;
					}
				case MesseggeType.RoomList:
					{
						SetRoomList(msg);
						break;
					}
				case MesseggeType.Passport:
					{
						SetPassport(msg);
						break;
					}
				case MesseggeType.RoomEndpoint:
					{
						SetRoomIpEndpoint(msg);
						break;
					}
				case MesseggeType.StartGame:
					{
						GameStart(msg);
						break;
					}
				case MesseggeType.EndGame:
					{
						EndGame(msg);
						break;
					}
				default: throw new Exception("Undefine MessaggeType");
			}
		}

		private void SetMap(IPackage package)
		{
			var map = package.Data as IMap;
            OnMapChanged?.Invoke(this, map);
        }
        private void SetRoomList(IPackage package)
		{
			SetRoomsStat(package.Data as List<IRoomStat>);
		}
		private void SetPassport(IPackage package)
		{
			Owner.Passport = (Guid)package.Data;
		}
		private void SetRoomIpEndpoint(IPackage package)
		{
			Owner.adresee_list.Add("Room", package.Data as IAddresssee);
		}
		private void GameStart(IPackage package)
		{
			//запустить игровое поле
			//запустить отправку ientyti на сервер
		}
		private void EndGame(IPackage package)
		{
			//остановить отрисовку
			//перейти в окно со списком комнат
		}
		private void SetRoomError(IPackage package)
		{
			_roomError = package.Data;
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
				Sender_Passport = Owner.Passport,
				Data = connectionData,
				MesseggeType = MesseggeType.CreateRoom
			}, Owner.adresee_list["Host"]);
		}
		public void JOINGame(Guid room_guid, string player_name)
		{
			var connectionData = new ConectionData()
			{
				Pasport = room_guid,
				PlayerName = player_name
			};

			Owner.Sender.SendMessage(new Package()
			{
				Sender_Passport = Owner.Passport,
				Data = connectionData,
				MesseggeType = MesseggeType.CreateRoom
			}, Owner.adresee_list["Host"]);
		}

        public override void OnNewAddresssee_Handler(object Sender, NewAddressseeData evntData)
        {
            throw new NotImplementedException();
        }

        public void OnViewCommandHandler(object Sender, object evntData)
        {
            throw new NotImplementedException();
        }
        public void OnEntityHandler(object Sender, IEntity evntData)
        {
            throw new NotImplementedException();
        }
    }
}
