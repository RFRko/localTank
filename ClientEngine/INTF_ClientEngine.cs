using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tanki
{
    public interface IClientEngine
    {
        IEnumerable<IRoomStat> RoomsStat { get; }
        IMap Map { get; }
        ITank Entity { get; set; }
		string ErrorText { get; }
		Size Map_size { get; }
		void CreateGame(GameSetings gameSetings, string player_name);
		void JOINGame(Guid room_guid, string player_name);
		Guid GetPassport();
		void GetRoomList();


		event EventHandler<RoomStatChangeData> OnRoomsStatChanged;
        event EventHandler<GameStateChangeData> OnMapChanged;
		event EventHandler<ErrorData> OnError;
		void OnEntityHandler(Object Sender, ITank evntData);
        void OnViewCommandHandler(Object Sender, Object evntData);
    }
}

