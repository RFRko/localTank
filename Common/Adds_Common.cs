﻿namespace Tanki
{
	/// <summary> Варианты направления движения</summary>
	public enum Direction {
		/// <summary> Направление движения влево</summary>
		Left,
		/// <summary> Направление движения вправо</summary>
		Right,
		/// <summary> Направление движения вверх </summary>
		Up,
		/// <summary> Направление движения вниз </summary>
		Down
	}



	/// <summary> Названия команд </summary>
	public enum Team {
		/// <summary> Красная команда </summary>
		Red,
		/// <summary> Зеленая команда </summary>
		Green
	}
	/// <summary>
	/// Тип действия, выполняемое сущностью
	/// </summary>
    public enum EntityAction
    {
		/// <summary>Действие отсутствует </summary>
		None,
		/// <summary>Сущность передвигается </summary>
		Move,
		/// <summary>Сущность стреляет </summary>
		Fire
	}
	/// <summary> Тип игры </summary>
	public enum GameType
	{
		/// <summary> Игра до последнего оставшегося в живых игрока </summary>
		LastAlive,
		/// <summary> Игра на время </summary>
		FragPerTime,
		/// <summary> Защита флага </summary>
		FlagDefence,
		/// <summary>Игра до определенного количества убийств </summary>
		FragCount,
        /// <summary>для не игровой комнаты </summary>
        NotGame
    }
    /// <summary> Тип системного сообщения для отправки </summary>
    public enum MesseggeType
	{
		/// <summary> Обмен обЪектами</summary>
		Map,
		/// <summary> Запрос списка комнат </summary>
		GetRoomList,
		/// <summary> Отправка клиенту списка комнат </summary>
		RoomList,
		/// <summary> Отправка клиенту сгенерированого id </summary>
		Passport,
		/// <summary> Отравка серверу id выбранной комнаты </summary>
		RoomID,
		/// <summary> Отравка клиенту ipEndpoint выбранной/созданной комнаты </summary>
		RoomInfo,
		/// <summary> Отправка клиенту сообщения об ошибке </summary>
		Error,
		/// <summary> Отправка серверу сообщения о создании новой комнаты </summary>
		CreateRoom,
		/// <summary> Отправка клиентам сообщения о начале игры </summary>
		StartGame,
		/// <summary> Отправка клиентам сообщения о конце игры </summary>
		EndGame,
		/// <summary> Oбъект отправляемый клиентом на сервер</summary>
		Entity,
		/// <summary> Событие о уничтожении танка</summary>
		TankDeath,
		RequestLogOff
	}
	/// <summary>
	/// Состояние игры
	/// </summary>
	public enum GameStatus
	{
		/// <summary> Ожидание начала игры</summary>
		WaitForStart,
		/// <summary> Игра запущена</summary>
		Start,
		/// <summary> Игра завершена</summary>
		EndGame
	}
	public enum BlockType
	{
		Brick,
		Brick2,
		Concrete,
		Tree,
		Health
	}
}
