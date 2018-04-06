using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tanki 
{
	public class ClientEngine : EngineAbs,IClientEngine
    {
		public override ProcessMessageHandler ProcessMessage { get; protected set; }
		public override ProcessMessagesHandler ProcessMessages { get; protected set; }

        public IEnumerable<IRoomStat> RoomsStat { get { return _RoomsStat; } protected set { SetRoomsStat(_RoomsStat); } }
        public IMap Map { get; protected set; }
        public IEntity Entity { get; protected set; }

        private IGameEngineOwner Owner;
        private List<IRoomStat> _RoomsStat = new List<IRoomStat>();


        public event EventHandler<RoomStatChangeData> OnRoomsStatChanged;
        public event EventHandler<IMap> OnMapChanged;

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


        private void SetRoomsStat(List<IRoomStat> newVal)
        {
            _RoomsStat = newVal;
            OnRoomsStatChanged?.Invoke(this, new RoomStatChangeData() { RoomsStat = _RoomsStat });
        }


        private void ProcessMessageHandler(IPackage msg)
		{
            switch (msg.MesseggeType)
			{
				case MesseggeType.Map:
					{
						Map(msg);
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
				case MesseggeType.StatGame:
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

		private void Map(IPackage package)
		{
			var map = package.Data;
			//отправить map отрисовщику
		}
		private void SetRoomList(IPackage package)
		{
			//обновить roomlist
		}
		private void SetPassport(IPackage package)
		{
			//set passport
		}
		private void SetRoomIpEndpoint(IPackage package)
		{
			//set room_endpoint
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

		public void On_Entity_Update_Handler(IEntity entity)
		{
			//ловит уведомления от отрисовщика об изменнении Entity и отправляет его на сервер
			IPEndPoint room_IpEndpoint = null; //заменить, получить ipendpoint текущего room
			Guid my_passport = new Guid(); //заменить, получить passport клиента
			Owner.Sender.SendMessage(new Package()
			{
				Passport = my_passport,
				Data = entity,
				MesseggeType = MesseggeType.Entity
			}, room_IpEndpoint);
		}
		public void CreateGame(GameSetings gameSetings)
		{
			//отправить настройки и имя игрока серверу
		}
		public void ConnectGame(Guid room_guid)
		{
			//отправить id комнаты и имя игрока на сервер
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

        //событие о рум эндпоинт
        //событие о мап
        //событие о энтити
        //событие о старте игры
        //событие о конце игры
    }
}
