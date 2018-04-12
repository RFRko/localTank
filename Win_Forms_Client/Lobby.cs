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

            onRoomListRecieved = onRoomListRecievedProc;

            InitializeComponent();
        }

        // подписан на событие, обновляет список комнат
        private void SetRoomList(object sender, RoomStatChangeData data)
        {
               this.Invoke(onRoomListRecieved,data.newRoomsStat);            
        }

        private Action<IEnumerable<IRoomStat>> onRoomListRecieved;
        private void onRoomListRecievedProc(IEnumerable<IRoomStat> RoomList)
        {
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = RoomList;
                dataGridView1.Refresh();
        }


		// обновить
		private void button3_Click(object sender, EventArgs e)
		{
			clientEngine.GetRoomList();
		}

		//подписан на событие, ловит ошибки присланные сервером
		private void ErrorHandler(object sender, ErrorData e)
		{
			var caption = "Ошибка";
			var message = e.errorText;
			var buttons = MessageBoxButtons.OK;
			MessageBox.Show(message, caption, buttons);

			clientEngine.GetRoomList();
		}

		// создать игру
		private void button1_Click(object sender, EventArgs e)
		{
			var name = textBox1.Text;
			if (!string.IsNullOrEmpty(name)) {
				label3.Text = "";
				var gameOptionsForm = new GameOptionsForm();
				gameOptionsForm.Show();
				if (gameOptionsForm.ok)
				{
					clientEngine.CreateGame(gameOptionsForm.gameSetings, name);

					var gameForm = new GameForm(clientEngine, gameOptionsForm.gameSetings.MapSize);
					gameForm.Show();
					Hide();
				}
				else return;
			}
			else label3.Text = "Укажите ваше имя";
		}

		// подключится к комнате
		private void button2_Click(object sender, EventArgs e)
		{
			var name = textBox1.Text;
			if (!string.IsNullOrEmpty(name))
			{
				label3.Text = "";
				var room_index = dataGridView1.SelectedRows[0].Index;
				var room_guid = clientEngine.RoomsStat.ElementAt(room_index).Pasport;

				clientEngine.JOINGame(room_guid, name);

				var gameForm = new GameForm(clientEngine, clientEngine.Map_size);
				gameForm.Show();
				Hide();
			}
			else label3.Text = "Укажите ваше имя";
		}
	}
}
