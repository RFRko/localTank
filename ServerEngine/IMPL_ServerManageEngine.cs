using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tanki
{
	public class ServerManageEngine : EngineAbs
	{
		public override ProcessMessageHandler ProcessMessage { get; protected set; }
		public override ProcessMessagesHandler ProcessMessages { get; protected set; }
		private IManagerRoom ManagerRoom;

		public ServerManageEngine() : base() { }
		public ServerManageEngine(IRoom inRoom) : base(inRoom)
		{
			ProcessMessage += ProcessMessageHandler;
			ProcessMessages = null;

            if (Owner != null) // owner реально присваивается в registerdependecy родительского объекта - поэтому тут он может быть еще пустой если явно не передавался через конструктор
                ManagerRoom = Owner as IManagerRoom; 
		}

		private void ProcessMessageHandler(IPackage msg)
		{
			switch (msg.MesseggeType)
			{
				case MesseggeType.GetRoomList:
					{
						RoomList(msg);
						break;
					}
				case MesseggeType.RoomID:
					{
						RoomConnect(msg);
						break;
					}
				case MesseggeType.CreateRoom:
					{
						CreatRoom(msg);
						break;
					}
				default: return;
			}
		}

		public override void OnNewAddresssee_Handler(object sender, NewAddressseeData evntData)
		{
			var gamer = evntData.newAddresssee as IGamer;
			if (gamer != null)
			{
				Owner.Sender.SendMessage(new Package()
				{
					Data = gamer.Passport,
					MesseggeType = MesseggeType.Passport
				}, gamer.RemoteEndPoint);

				SendRoomList(gamer.RemoteEndPoint);
			}
			else throw new Exception("Empty new gamer");
		}
		private void SendRoomList(IPEndPoint addresssee)
		{
            ManagerRoom = Owner as IManagerRoom;

            IRoomsStat roomsData = new RoomsListData(ManagerRoom.getRoomsStat()); // должен быть сериализуемый объект

            Owner.Sender.SendMessage(new Package()
			{
				Data = roomsData,
				MesseggeType = MesseggeType.RoomList
			}, addresssee);
		}
		private void RoomList(IPackage package)
		{
			var client_id = package.Sender_Passport;
			IGamer gamer = ManagerRoom.GetGamerByGuid(client_id);
			SendRoomList(gamer.RemoteEndPoint);
		}
		private void RoomConnect(IPackage package)
		{
			var cd = (IConectionData)package.Data;
			var name = cd.PlayerName;
			var client_passport = package.Sender_Passport;
			IGamer gamer = ManagerRoom.GetGamerByGuid(client_passport);
			gamer.SetId(name, client_passport);
			var room_passport = cd.RoomPasport;
			var room = (Owner as IManagerRoomOwner).GetRoomByGuid(room_passport);
			var map_Size = room.GameSetings.MapSize;
			if (room != null)
			{
				if (room.Gamers.Count() < room.GameSetings.MaxPlayersCount)
				{
					IPEndPoint room_ipendpoint = ManagerRoom.MooveGamerToRoom(gamer, room_passport);
					var roominfo = new RoomInfo()
					{
						roomEndpoint = room_ipendpoint,
						mapSize = map_Size
					};

					Owner.Sender.SendMessage(new Package()
					{
						Data = roominfo,
						MesseggeType = MesseggeType.RoomInfo
					}, gamer.RemoteEndPoint);
				}
				else
				{
					Owner.Sender.SendMessage(new Package()
					{
						Data = "Room is full",
						MesseggeType = MesseggeType.Error
					}, gamer.RemoteEndPoint);
				}
			}
			else
			{
				Owner.Sender.SendMessage(new Package()
				{
					Data = "Room is not exist",
					MesseggeType = MesseggeType.Error
				}, gamer.RemoteEndPoint);
			}
		}
		private void CreatRoom(IPackage package)
		{
			var conectionData = (IConectionData)package.Data;
			var client_passport = package.Sender_Passport;
			var newGameSettings = conectionData.GameSetings;
			var player_name = conectionData.PlayerName;
      
            // получить Gamer по id из списка ожидающих
            IGamer gamer = ManagerRoom.GetGamerByGuid(client_passport);
            gamer.SetId(player_name, client_passport);
            
            // создать комнату
            IRoom newGameRoom = ManagerRoom.AddRoom(newGameSettings, client_passport);
            newGameRoom.CreatorPassport = gamer.Passport;

			// добавить в нее игрока
			var room_endpoint = ManagerRoom.MooveGamerToRoom(gamer, newGameRoom.Passport);

			var roominfo = new RoomInfo()
			{
				roomEndpoint = room_endpoint,
				mapSize = newGameSettings.MapSize
			};

			Owner.Sender.SendMessage(new Package()
            {
              Data = roominfo,
              MesseggeType = MesseggeType.RoomInfo
			}, gamer.RemoteEndPoint);
        }
	}
}
