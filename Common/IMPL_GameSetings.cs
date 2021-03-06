﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanki
{
	[Serializable]
	public class GameSetings : IGameSetings
	{
		public int GameSpeed { get; set; }
		public int _ObjectsSize;
		public int ObjectsSize
		{
			get { return _ObjectsSize; }
			set { _ObjectsSize = value; Bullet_size = value / 4; }
		}
		public int Bullet_size { get; set; }
		public Size MapSize { get; set; }
		public int MaxPlayersCount { get; set; }
		public GameType GameType { get; set; }
	}
}
