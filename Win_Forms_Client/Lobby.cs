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
        IClientEngine clientEngine;

        public Lobby(IClientEngine ClientEngine)
        {
            clientEngine = ClientEngine;

            clientEngine.OnRoomsStatChanged += SetRoomList;
            clientEngine.OnError += ErrorHandler;
			clientEngine.OnRoomConnect += RoomConnect;


			onRoomListRecieved = onRoomListRecievedProc;
			onRoomConnect = onRoomConnectProc;

            InitializeComponent();
			Name_tb.Text = "Vasya";
		}
		
        private void SetRoomList(object sender, RoomStatChangeData data)
        {
               this.Invoke(onRoomListRecieved,data.newRoomsStat);            
        }

        private Action<IEnumerable<IRoomStat>> onRoomListRecieved;
        private void onRoomListRecievedProc(IEnumerable<IRoomStat> RoomList)
        {
			DGV_RoomList.DataSource = null;
            DGV_RoomList.DataSource = RoomList;
			DGV_RoomList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DGV_RoomList.Refresh();
		}

		private void RoomConnect(object sender, RoomConnect data)
		{
			this.Invoke(onRoomConnect, data.MapSize);
		}

		private Action<Size> onRoomConnect;
		private void onRoomConnectProc(Size MapSize)
		{
			var gameForm = new GameForm(clientEngine, MapSize);
			gameForm.Show();
			Hide();
		}

		private void ErrorHandler(object sender, ErrorData e)
		{
			var caption = "Ошибка";
			var message = e.errorText;
			var buttons = MessageBoxButtons.OK;
			MessageBox.Show(message, caption, buttons);

			clientEngine.GetRoomList();
		}
		
		private void Create_Room_btn_Click(object sender, EventArgs e)
		{
			var name = Name_tb.Text;
			if (!string.IsNullOrEmpty(name))
			{
				label3.Text = "";
				var gameOptionsForm = new GameOptionsForm();
				gameOptionsForm.ShowDialog();
				if (gameOptionsForm.ok)
					clientEngine.CreateGame(gameOptionsForm.gameSetings, name);
				else return;
			}
			else label3.Text = "Укажите ваше имя";
		}

		private void Conect_btn_Click(object sender, EventArgs e)
		{
			var name = Name_tb.Text;
			if (!string.IsNullOrEmpty(name))
			{
				label3.Text = "";
				var room_index = DGV_RoomList.SelectedRows[0].Index;
				var room_guid = clientEngine.RoomsStat.ElementAt(room_index).Pasport;

				clientEngine.JOINGame(room_guid, name);
			}
			else label3.Text = "Укажите ваше имя";
		}

		private void Refresh_btn_Click(object sender, EventArgs e)
		{
			clientEngine.GetRoomList();
		}
	}
}
