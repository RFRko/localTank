using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tanki
{
	public partial class GameForm : Form
	{
		private IClientEngine ClientEngine;

		public GameForm(IClientEngine clientEngine, Size size)
		{
			InitializeComponent();
			this.Size = size;
			ClientEngine = clientEngine;
			clientEngine.OnMapChanged += OnMapChangeHandler;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
		}
		private void OnMapChangeHandler(object Sender, GameStateChangeData data)
		{
				var map = data.newMap;
				var myTank = map.Tanks.First((i) => { return i.Tank_ID == ClientEngine.GetPassport(); });
		}
	}
}
