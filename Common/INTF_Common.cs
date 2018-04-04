using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

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
		Guid Pasport { get; set; }
		int Players_count { get; set; }
		Guid Creator_Pasport { get; set; }
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
		Rectangle Position { get; set; }
		Direction Direction { get; set; }
        EntityAction Command { get; set; }
        bool Can_Shoot { get; set; }
		bool Is_Alive { get; set; }
		bool Can_Be_Destroyed { get; set; }
        int Speed { get; set; }
		int Size { get; set; }
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
		Guid Tank_ID { get; set; }
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
		Guid Parent_Id { get; set; }
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


    public interface IAddresssee
    {
        IPEndPoint RemoteEndPoint { get; }
    }

    /// <summary>
    /// Cущность отправляющая информацию от клиента хосту
    /// </summary>
    public interface ISender
    {
        //string RemoteAdress { get; set; }   // ip хоста
        //int RemotePort { get; set; }        // порт хоста
        //IPackage Pack { get; set; }         // пакет на отправку
        void SendMessage(IPackage msg, IPEndPoint Target);
        void SendMessage(IPackage msg, IEnumerable<IPEndPoint> Targets);
        void SendMessage(IPackage msg, IAddresssee Target);
        void SendMessage(IPackage msg, IEnumerable<IAddresssee> Targets);

    }




    /// <summary>
    /// Cущность принимающая информацию клиентом от хоста
    /// </summary>
    public interface IReciever: INetCommunicationObj
    {
        IRecieverClient Owner { get; }
        bool Alive { get; set; }                            // работает ли поток на прием
        //int LocalPort { get; set; }                       // прослушивающий порт
        IPEndPoint LockalEndPoint { get; }
        INetCommunicationObj GetNetCommunicationObject();
        void Run();                                         //must running in separate thread
        void OnRegistered_EventHandler(Object Sender, RegRecieverData evntData);

    }

    public interface IRecieverClient
    {
        IMessageQueue MessageQueue { get; }
        void RegisterDependcy(IReciever regReciever);
        event EventHandler<RegRecieverData> OnRegisterReciever;
    }




    public interface INetCommunicationObj { }
    public interface ISocket: INetCommunicationObj
    {
        Socket Socket { get; }
    }
    public interface IUdpClient : INetCommunicationObj
    {
        UdpClient NetClient { get; }
    }


    #region MessageQueue Interfaces
    /// <summary> Интерфейс описывает очередь сообщений клиента/сервера </summary>
    public interface IMessageQueue : IDisposable
    {
        IMessageQueueClient Owner { get; }
        void Enqueue(IPackage msg);
        void RUN();
        void OnRegistered_EventHandler(Object Sender, RegMsgQueueData evntData);

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

    public interface IMessageQueueClient
    {
        IReciever Reciever { get; }
        IEngine Engine { get; }
        void RegisterDependcy(IMessageQueue regMsqQueue);
        event EventHandler<RegMsgQueueData> OnRegisterMessageQueue;
    }

    #endregion MessageQueue Interfaces


    #region IEngine
    /// <summary> Общий Интерфейс для движков (серверного и клиентского
    /// должен передаватся как dependency MessageQueue (поэтому определяется сдесь для исключения циклических ссылок библиотек)
    /// реализации будут в разных библиотеках, т.к. это разные реализации для разных приложений

    public delegate void ProcessMessageHandler(IPackage message);
    public delegate void ProcessMessagesHandler(IEnumerable<IPackage> messages);

    public interface IEngine:IAddressseeHolderClient
    {
        IEngineClient Owner { get; }
        ProcessMessageHandler ProcessMessage { get; }
        ProcessMessagesHandler ProcessMessages { get; }
        void OnRegistered_EventHandler(Object Sender, RegEngineData evntData);
    }


    public interface IEngineClient
    {
        IMessageQueue MessageQueue { get; }
        ISender Sender { get; }

        void RegisterDependcy(IEngine regEngine);
        event EventHandler<RegEngineData> OnRegisterEngine;
    }

    #endregion


    ///<summary> Реализует общую абстракцию сетевой обработки - 
    /// Итнерфейс, имеющий Enumerable of IReciver, IMessageQueue, IEngine, ISender
    /// может использоваться для GameServer и GameClient
    /// </summary>
    #region INetProcessor
    public interface INetProcessor:IMessageQueueClient, IEngineClient,IRecieverClient
    {
        //IMessageQueueClient - предоставляет IEnumerable of IReciever (использующие IMessageQueue), IEngine (нужный для IMessageQueue), 
        //                      а также механизм регистрации dependency IMessageQueue
        //IEngineClient - предоставляет IMessageQueue (использующий IEngine), и ISender (Нужный для IEngine)
        //                      а также механизм регистрации dependency IEngine
        void RUN();
    }
	#endregion INetProcessor



	/// <summary> Пакет данных - играет роль сообщения между клинтом/сервером.
	/// Используется в IMesegeQueue, ISender, IReceiver</summary>
	/// Реализующий клас обязан иметь атрибут [Serializable]
	public interface IPackage
	{
		Guid Passport { get; set; }
		object Data { get; set; }
		MesseggeType MesseggeType { get; set; }
	}


	/// <summary> Сереализатор. Используется в ISender, IReceiver. </summary>
	public interface ISerializator
	{
		byte[] Serialize(object obj);
		IPackage Deserialize(byte[] bytes);
	}

	public interface IGameSetings
	{
		int GameSpeed { get; set; }
		int ObjectsSize { get; set; }
		int MapSize { get; set; }
		int MaxPlayersCount { get; set; }
		GameType GameType { get; set; }
	}

	public interface IConectionData
	{
		string PlayerName { get; set; }
		Guid Pasport { get; set; }
	}
}
