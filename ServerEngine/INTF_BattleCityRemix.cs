using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanki
{

///<summary>
/// убрал заглушки, добавил в проект ссылку на Common
/// 
/// вместо этого добавил в Common  интерфейс IEngine
/// добавил абстрактный класс ServerEngineAbs
/// переименовал ServerEngine в ServerGameEngine и унаследовал от абстркатного - реализацию не трогал (только переименовал IProtocol в IPackage)
/// 
/// ДЛЯ ЧЕГО: нужен еще один вид серверного движка, который будет принимать просто IPackage, a не IEnumerable<IPackage> (для функций обмена управляющими сообщениями между клиент-сервер)
/// 
/// </summary>
/// 

    public interface IServerEngine:IEngine
    {
        
    }

 //   public interface IServerEngine: IEngine
	//{
	//	//ProcessMessageHandler ProcessMessage { get; } перенесено в Common - см.пояснения там.
	//	IPackage GenerateMap();
 //       IPackage Reload(int id);
 //       IPackage Move(int id);
 //       IPackage Fire(int id);
 //       IPackage Death(int id);
 //       // тут логично принимать какое-то условие для определения победы из IRoom
 //       IPackage CheckWin();
	//	IEnumerable<IPackage> Send();
	//}

///<summary>
/// ниже объявил то что нужно для отдельной реализации серверного движка обработки единичных управляющих сообщений
/// </summary>

    public enum SrvEngineType
    {
        srvGameEngine,
        srvManageEngine
    }

    public interface IServerEngineFabric
    {
        IServerEngine CreateEngine(SrvEngineType engineType);
        IServerEngine CreateEngine(SrvEngineType engineType, IRoom inRoom);

    }



}
