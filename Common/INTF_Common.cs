using System;
using System.Collections.Generic;
using System.Drawing;

namespace Tanki
{
	/// <summary>
	/// Интерфейс описующий информацию об игре (IGameRoom).
	/// Является частью интерфейса IGameRoom.
	/// Предназначен для обмена между клиентом/серверером
	/// Реализующий клас обязан иметь атрибут [Serializable]
	/// </summary>
	public interface IRoomStat
	{
		string Id { get; set; }
		int Players_count { get; set; }
		string Creator_Id { get; set; }
	}


	/// <summary>
	/// Интерфейс описующий информацию об игровом поле.
	/// Предназначен для обмена между клиентом/серверером
	/// Реализующий клас обязан иметь атрибут [Serializable]
	/// </summary>
	public interface IMap
	{
		IEnumerable<ITank> Tanks { get; set; }
		IEnumerable<IBullet> Bullets { get; set; }
		IEnumerable<IBlock> Blocks { get; set; }
	}



	/// <summary>
	/// Интерфейс описующий информацию об объекте.
	/// Является родителем для ITank, IBullet, IBlock.
	/// Предназначен для обмена между клиентом/серверером
	/// Реализующий клас обязан иметь атрибут [Serializable]
	/// </summary>
	public interface IEntity
	{
		Point Position { get; set; }
		Direction Direction { get; set; }
		bool Can_Shoot { get; set; }
		bool Is_Alive { get; set; }
		bool Can_Be_Destroyed { get; set; }
        int Speed { get; set; }
	}



	/// <summary>
	/// Интерфейс описующий информацию о Танке(Игроке).
	/// Является наследником IEntity.
	/// Является частью IPlayer(подругому IGamer).
	/// Используется в интерфейсах IServerEngine и IClientEngine.
	/// Реализующий клас обязан иметь атрибут [Serializable]
	/// </summary>
	public interface ITank : IEntity
	{
		int Lives { get; set; }
		Team Team { get; set; }
	}



	/// <summary>
	/// Интерфейс описующий информацию о Пуле.
	/// Является наследником IEntity.
	/// Используется в интерфейсах IServerEngine и IClientEngine.
	/// Реализующий клас обязан иметь атрибут [Serializable]
	/// </summary>
	public interface IBullet : IEntity
	{
		string Parent_Id { get; set; }
	}



	/// <summary>
	/// Интерфейс описующий информацию о Преградах (Пеньках).
	/// Является наследником IEntity.
	/// Используется в интерфейсах IServerEngine и IClientEngine.
	/// Реализующий клас обязан иметь атрибут [Serializable]
	/// </summary>
	public interface IBlock : IEntity
    {

    }



    /// <summary>
    /// Cущность отправляющая информацию от клиента хосту
    /// </summary>
    public interface ISender
    {
        string RemoteAdress { get; set; }   // ip хоста
        int RemotePort { get; set; }        // порт хоста
        IPackage Pack { get; set; }         // пакет на отправку
        void SendMessage();
    }


    /// <summary>
    /// Cущность принимающая информацию клиентом от хоста
    /// </summary>
    public interface IReceiver
    {
        bool Alive { get; set; }   // работает ли поток на прием
        int LocalPort { get; set; }        // прослушивающий порт
        IPackage Run();
    }

    #region MessageQueue Interfaces
    /// <summary> Интерфейс описывает очередь сообщений клиента/сервера </summary>
    public interface IMessageQueue : IDisposable
    {
        void Enqueue(IPackage msg);
        void RUN();
    }

    public enum MsgQueueType
    {
        mqOneByOneProcc,
        mqByTimerProcc
    }

    public interface IMessageQueueFabric
    {
        IMessageQueue CreateMessageQueue(MsgQueueType queueType, IEngine withEngine);
    }

    #endregion MessageQueue Interfaces


    #region IEngine
    /// <summary> Общий Интерфейс для движков (серверного и клиентского
    /// должен передаватся как dependency MessageQueue (поэтому определяется сдесь для исключения циклических ссылок библиотек)
    /// реализации будут в разных библиотеках, т.к. это разные реализации для разных приложений

    public delegate void ProcessMessageHandler(IPackage message);
    public delegate void ProcessMessagesHandler(IEnumerable<IPackage> messages);

    public interface IEngine
    {
        ProcessMessageHandler ProcessMessage { get; }
        ProcessMessagesHandler ProcessMessages { get; }
    }

    #endregion




    /// <summary> Пакет данных - играет роль сообщения между клинтом/сервером.
    /// Используется в IMesegeQueue, ISender, IReceiver</summary>
    /// Реализующий клас обязан иметь атрибут [Serializable]
    public interface IPackage
	{
		string Sender_id { get; set; }
		object Data { get; set; }
	}


	/// <summary> Сереализатор. Используется в ISender, IReceiver. </summary>
	public interface ISerializator
	{
		byte[] Serialize(object obj);
		IPackage Deserialize(byte[] bytes);
	}
}
