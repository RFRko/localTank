using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tanki
{
    public interface IClientEngine
    {
        IEnumerable<IRoomStat> RoomsStat { get; }
        IMap Map { get; }
        IEntity Entity { get; }
		string RoomError { get; }
		void CreateGame(GameSetings gameSetings, string player_name);
		void JOINGame(Guid room_guid, string player_name);
		void GetRoomList();


		event EventHandler<RoomStatChangeData> OnRoomsStatChanged;
        event EventHandler<GameStateChangeData> OnMapChanged;
		event EventHandler<ErrorData> OnError;
		void OnEntityHandler(Object Sender, IEntity evntData);
        void OnViewCommandHandler(Object Sender, Object evntData);

    }
}

