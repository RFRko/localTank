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
		int MaxHealthPoints { get; }
		void CreateGame(GameSetings gameSetings, string player_name);
		void JOINGame(Guid room_guid, string player_name);
		Guid GetPassport();
		void GetRoomList();
		void StopGame();
		void exit();

		event EventHandler<ErrorData> onEndGame;
		event EventHandler<ErrorData> onGameStart;
		event EventHandler<ErrorData> onDeath;
		event EventHandler<RoomStatChangeData> OnRoomsStatChanged;
        event EventHandler<GameStateChangeData> OnMapChanged;
		event EventHandler<ErrorData> OnError;
		event EventHandler<RoomConnect> OnRoomConnect;
		event EventHandler<DestroyableTank> OnTankDeath;
		void OnEntityHandler(Object Sender, ITank evntData);
        void OnViewCommandHandler(Object Sender, Object evntData);
    }
}

