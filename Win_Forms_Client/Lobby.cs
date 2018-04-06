using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tanki
{
	public partial class Lobby : Form
	{
		Client client;

		public Lobby(Client client)
		{
			this.client = client;
			InitializeComponent();
		}

		public void Refresh()
		{
			dataGridView1.DataSource = null;
			dataGridView1.DataSource = client.RoomList;
			dataGridView1.Refresh();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			Refresh();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var options_form = new GameOptionsForm();
			options_form.ShowDialog();
			var result = options_form.gameSetings;
			if (result != null){

				//client.Sender.SendMessage(new Package()
				//{
				//	Data = result,
				//	MesseggeType = MesseggeType.Settings
				//}, client.adresee_list["host"]);

				var gameForm = new GameForm();
				gameForm.Show();
				Hide();
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			//подключится
		}
	}


	public class Client
	{
		public Dictionary<string, IPEndPoint> adresee_list;
		public TcpClient tcp;
		public IEntity entity;
		public Guid passport;

		public List<RoomStat> RoomList { get; set; }
		public IEntity ClientGameState { get; set; }

		public IReciever Reciever { get; protected set; }
		public IEngine Engine { get; protected set; }
		public IMessageQueue MessageQueue { get; protected set; }
		public ISender Sender { get; protected set; }

		public Client(IPEndPoint iPEndPoint, ushort lockal_port)
		{
			adresee_list = new Dictionary<string, IPEndPoint>();
			adresee_list.Add("host", iPEndPoint);
			tcp = new TcpClient();
			Engine = new ClientEngine(this);
			MessageQueue = new MessageQueue_ProcessedOneByOne(Engine);
		}

		public bool RUN()
		{
			try
			{
				tcp.Connect(adresee_list["host"]);
				return true;
			}
			catch { return false; }
		}
		public void RUN_GAME()
		{
			throw new NotImplementedException();
		}
	}
	public class ClientEngine: IEngine
	{
		private Client parent;

		public IEngineClient Owner => throw new NotImplementedException();
		public ProcessMessageHandler ProcessMessage => throw new NotImplementedException();
		public ProcessMessagesHandler ProcessMessages => throw new NotImplementedException();

		public ClientEngine(Client parent)
		{
			this.parent = parent;
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

		//ловит уведомления от отрисовщика об изменнении Entity и отправляет его на сервер
		public void On_Entity_Update_Handler(IEntity entity)
		{
			IPEndPoint room_IpEndpoint = null; //заменить, получить ipendpoint текущего room
			Guid my_passport = new Guid(); //заменить, получить passport клиента
			parent.Sender.SendMessage(new Package()
			{
				Passport = my_passport,
				Data = entity,
				MesseggeType = MesseggeType.Entity
			}, room_IpEndpoint);
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

		public void OnRegistered_EventHandler(object Sender, RegEngineData evntData)
		{
			throw new NotImplementedException();
		}

		public void OnNewAddresssee_Handler(object Sender, NewAddressseeData evntData)
		{
			throw new NotImplementedException();
		}
	}
}
