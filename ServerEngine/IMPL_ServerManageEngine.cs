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
		private IManagerRoomOwner ManagerRoom;

		public ServerManageEngine() : base() { }
		public ServerManageEngine(IRoom inRoom) : base(inRoom)
		{
			ProcessMessage += ProcessMessageHandler;
			ProcessMessages = null;

			ManagerRoom = Owner as IManagerRoomOwner;
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
			var gamer = evntData as IGamer;
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
			Owner.Sender.SendMessage(new Package()
			{
				Data = ManagerRoom.getRoomsStat(),
				MesseggeType = MesseggeType.RoomList
			}, addresssee);
		}
		private void RoomList(IPackage package)
		{
			var client_id = package.Passport;
			IGamer gamer = null; // нужен метод -  IGamer Gamer_by_Guid(Guid pasport) поиск игрока по guid в списке ожидающих
			SendRoomList(gamer.RemoteEndPoint);
		}
		private void RoomConnect(IPackage package)
		{
			var cd = (IConectionData)package.Data;
			var name = cd.PlayerName;
			var client_passport = package.Passport;
			IGamer gamer = null; // нужен метод:  IGamer Gamer_by_Guid(Guid pasport); поиск игрока по guid в списке ожидающих
			gamer.SetId(name, client_passport);
			var room_passport = cd.Pasport;
			var room = Owner as IRoom; // нужен метод:  IRoom Room_by_Guid(Guid pasport); поиск комнаты по guid

			if (room != null)
			{
				if (room.Gamers.Count() < room.GameSetings.MaxPlayersCount)
				{
					IPEndPoint room_ipendpoint = ManagerRoom.MooveGamerToRoom(gamer, cd.Pasport);

					Owner.Sender.SendMessage(new Package()
					{
						Data = room_ipendpoint,
						MesseggeType = MesseggeType.RoomEndpoint
					}, gamer.RemoteEndPoint);
				}
				else
				{
					Owner.Sender.SendMessage(new Package()
					{
						Data = "Room is full",
						MesseggeType = MesseggeType.RoomError
					}, gamer.RemoteEndPoint);
				}
			}
			else
			{
				Owner.Sender.SendMessage(new Package()
				{
					Data = "Room is not exist",
					MesseggeType = MesseggeType.RoomError
				}, gamer.RemoteEndPoint);
			}
		}
		private void CreatRoom(IPackage package)
		{
			var newGame = (IGameSetings)package.Data;
			var client_passport = package.Passport;
			// получить Gamer по id из списка ожидающих
			// создать комнату
			// добавить в нее игрока
			// отправить ipendpoint комнаты игроку
		}
	}
}
