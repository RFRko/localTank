using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanki
{

    public class RegMsgQueueData : EventArgs
    {
        public IMessageQueueClient MsgQueueOwner { get; set; }
    }

    public class RegEngineData : EventArgs
    {
        public IEngineClient EngineOwner { get; set; }
    }

    public class NewAddressseeData : EventArgs
    {
        public IAddresssee newAddresssee { get; set; }
    }


    public class GameStateChangeData : EventArgs
    {
        public IEntity newGameState { get; set; }
    }

    public class EnforceDrawingData : EventArgs
    {
        public IEntity mustDrawTheEntity { get; set; }
    }
    public class GameStatusChangedData : EventArgs
	{
		public GameStatus newStatus { get; set; }
	}
}
