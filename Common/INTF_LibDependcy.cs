using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Tanki
{
    /// <summary>
    /// Добавлен для включения интерфейсов, которые не являются общими (характерны для конкретных библиотек),
    /// Но не могут быть в них объявлены из-за возникновения циклических ссылок при dependecy injection
    /// например ServerEngine должен иметь dependency Room, но Room в библиотеке Server, которой нужна dependency ServerEngine
    /// </summary>
    /// 

    public interface IGamer: IAddresssee
    {
        String Name { get;}
        Guid Passport { get; }
        IPEndPoint RemoteEndPoint { get; }
        //Socket Socket { get; set; }
        void SetId(String Name, Guid confirmpassport);
    }


    public interface IAddressseeHolderBase
    {
        event EventHandler<NewAddressseeData> OnNewAddresssee;
    }

    public interface IAddressseeHolder<T, Tid>: IAddressseeHolderBase where T:IAddresssee
    {
        IEnumerable<T> GetAddresssees();
        IAddresssee this[Tid id] { get; }
    }


    public interface IAddressseeHolderClient
    {

        void OnNewAddresssee_Handler(Object Sender, NewAddressseeData evntData);
    }


	/// <summary>
	/// Нужна для:
	/// -IServer (библиотека GameServer)
	/// -ServerGameEngine (библиотека ServerEngine)
	/// </summary>
	public interface IRoom : INetProcessor, IAddressseeHolder<IGamer, String>
	{
		IRoomStat RoomStat { get; }
		IGameSetings GameSetings { get; set; }
		IEnumerable<IGamer> Gamers { get; }
		void AddGamer(IGamer newGamer);

		void RUN();
	}


	public enum RoomType
    {
        rtMngRoom,
        rtGameRoom
    }

    public interface IRoomFabric
    {
        IRoom CreateRoom(String roomId, IPEndPoint localEP, RoomType roomType);
    }


}
