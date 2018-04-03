using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanki
{
	[Serializable]
	public class GameSetings : IGameSetings
	{
		public int GameSpeed { get; set; }
		public int ObjectsSize { get; set; }
		public int MapSize { get; set; }
		public int MaxPlayersCount { get; set; }
		public GameType GameType { get; set; }
	}
}
