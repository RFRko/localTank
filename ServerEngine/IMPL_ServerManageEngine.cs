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
        public ServerManageEngine() : base() { }
        public ServerManageEngine(IRoom inRoom) : base(inRoom)
        {
            this.ProcessMessage = ProcessMessage;
            this.ProcessMessages = null;
        }

        public override ProcessMessageHandler ProcessMessage { get; protected set; }
        public override ProcessMessagesHandler ProcessMessages { get; protected set; }

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
				var gamer =  WatingGamers.Find((s) => s.RemoteEndPoint == evntData.newAddresssee);
				if (gamer != null)
				{
					Sender.SendMessage(new Package()
					{
						Data = gamer.Passport,
						MesseggeType = MesseggeType.Passport
					}, gamer.RemoteEndPoint);

					SendRoomList(gamer.RemoteEndPoint);
				}
			}
		private void SendRoomList(IAddresssee addresssee)
			{
				var roomlist = new List<IRoomStat>();

				foreach (var i in Server.Rooms)
					roomlist.Add(i.RoomStat);

				Sender.SendMessage(new Package()
				{
					Data = roomlist,
					MesseggeType = MesseggeType.RoomList
				}, addresssee);
			}
		private void RoomList(IPackage package)
			{
				var client_id = package.Passport;
				var ipendpoint =  WatingGamers.Find((s) => s.Passport == client_id).RemoteEndPoint;
				SendRoomList((IAddresssee)ipendpoint);
			}
		private void RoomConnect(IPackage package)
			{
				var cd = (IConectionData)package.Data;
				var name = cd.PlayerName;
				var client_passport = package.Passport;
				var gamer = WatingGamers.Find((s) => s.Passport == client_passport);
				var room = ListRooms.Find((s) => s.RoomStat.Pasport == cd.Pasport);

				if (room != null)
				{
					if (room.RoomStat.Players_count < room.GameSetings.MaxPlayersCount)
					{
						room.AddGamer(new Gamer(gamer.RemoteEndPoint)
						{
							Name = name
						});
						WatingGamers.Remove(gamer);
					}
					else
					{
						Sender.SendMessage(new Package()
						{
							Data = "Room is full",
							MesseggeType = MesseggeType.RoomError
						}, gamer.RemoteEndPoint);
					}
				}
				else
				{
					Sender.SendMessage(new Package()
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

				var gamer = WatingGamers.Find((s) => s.Passport == client_passport);

				var new_room = new RoomAbs();

				new_room.AddGamer(gamer);

				WatingGamers.Remove(gamer);
			}
	}
}
