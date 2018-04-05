namespace Tanki
{
	/// <summary> Варианты направления движения (Left/Right/Up/Down)</summary>
	public enum Direction { Left, Right, Up, Down }



	/// <summary> Названия команд (Red/Green)</summary>
	public enum Team { Red, Green }

    public enum EntityAction
    {
		None,
        Move,
        Fire
    }

	public enum GameType
	{
		LastAlive,
		Time,
		FlagDefence
	}

	public enum MesseggeType
	{
		Map, //обмен обЪектами
		GetRoomList, //запрос списка комнат
		RoomList, //отправка клиенту списка комнат
		Passport, //отправка клиенту сгенерированый id
		RoomID, //отравка серверу id выбранной комнаты
		Connect, //подключение к серверу
		RoomError, //отправка клиенту сообщение об ошибке
		CreateRoom, //отправка сервереру сообщение о создании новой комнаты
		StatGame, //оправка клиентам сообщения о начале игры
		EndGame, //оправка клиентам сообщения о конце игры
		Kill, //отправка серерверу сообщения об убействе
		Setings, //настройки
		RoomEndpoint // Ipendpoint созданной/подключенной комнаты
	}
}
