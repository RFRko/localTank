using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tanki
{
	#region ЗАГЛУШКИ
	//заглушка
	public interface IServer
	{
		IListener ServerListner { get; }
		IEnumerable<IRoom> Rooms { get; } //список игровых комнат
		IEnumerable<IRoom> _rooms { get; set; } //список начальных комнат
		void RUN();
	}
	//заглушка
	public interface IListener { }
	//заглушка
	public class Gamer : IGamer, IDisposable
	{
		public Gamer(IPEndPoint ep)
		{
			RemoteEndPoint = ep;
			Passport = GuidGenTimeBased.GenGuid(ep.Address.ToString().GetHashCode(), ep.Port);
		}
		public string Name { get; set; }

		public Guid Passport { get; private set; }
		public IPEndPoint RemoteEndPoint { get; private set; }

		public void SetId(string newID, Guid confirmpassport)
		{
			if (Passport != confirmpassport)
				Dispose();
			Name = newID;
		}

		public void Dispose()
		{
			RemoteEndPoint = null;
		}

	}
	//заглушка
	public  class RoomAbs : NetProcessorAbs
	{
		public RoomAbs() { }
		public RoomAbs(String id, IPEndPoint localEP) : base()
		{
			RoomId = id;
			//Reciever = new ReceiverUdpClientBased(localEP);
			//Sender = new SenderUdpClientBased(Reciever);
		}

		private List<IGamer> _gamers = new List<IGamer>();

		public string RoomId { get; set; }

		public IRoomStat RoomStat { get; set; }

		public IEnumerable<IGamer> Gamers { get { return _gamers; } }

		public IGameSetings GameSetings { get; set; }

		public virtual void AddGamer(IGamer newGamer)
		{
			_gamers.Add(newGamer);
		}

		public virtual new void RUN()
		{
			base.RUN();
			//if (MessageQueue == null) throw new Exception("MessageQueue object not valid");
			//if (Engine == null) throw new Exception("Engine object not valid");

			//MessageQueue.RUN();
			//Reciever.Run();
		}
		//------------------------------------------------------------
	}
	#endregion
	//===================================================================================
	//Нужно подключить ссылки на файлы а после все исправить, тут одни костыли

	
	public class ServerManageEngine : EngineAbs
	{
		public override ProcessMessageHandler ProcessMessage { get; protected set; }
		public override ProcessMessagesHandler ProcessMessages { get; protected set; }
		private ISender Sender { get; set; }
		private IServer Server { get; set; }
		private List<IRoom> ListRooms;
		private List<IGamer> WatingGamers;
		
		public ServerManageEngine() : base() { }
		public ServerManageEngine(IServer server, ISender sender, IRoom inRoom) : base(inRoom)
			{
				ProcessMessage = ProcessMessage;
				ProcessMessages = null;

				Server = server;
				Sender = sender;

				ListRooms = Server._rooms as List<IRoom>;
				WatingGamers = ListRooms[0].Gamers as List<IGamer>;
			}

		private void ProcessMessageHandler(Package msg)
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
				var gamer = WatingGamers.Find((s) => s.RemoteEndPoint == evntData.newAddresssee);
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
		private void SendRoomList(IPEndPoint addresssee)
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
				SendRoomList(ipendpoint);
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
		private void CreatRoom(Package package)
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
