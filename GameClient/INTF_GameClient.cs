using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tanki;

namespace Tanki
{
    ///// <summary>
    ///// Cущность отправляющая информацию от клиента хосту
    ///// </summary>
    //public interface ISender
    //{
    //    string RemoteAdress { get; set; }   // ip хоста
    //    int RemotePort { get; set; }        // порт хоста
    //    IPackage Pack { get; set; }         // пакет на отправку
    //    void SendMessage();
    //}


    ///// <summary>
    ///// Cущность принимающая информацию клиентом от хоста
    ///// </summary>
    //public interface IReceiver
    //{
    //    bool Alive { get; set; }   // работает ли поток на прием
    //    int LocalPort { get; set; }        // прослушивающий порт
    //    IPackage Run();
    //}

    public interface IGameClient
    {
        void AddAddressee(String Id, IAddresssee addresssee);
        IAddresssee this[String id] { get; } //свойство идексатор для возврата Адресата по текстовому имени\ид.  
                                             //Адресат это объект с IPEndPoint комнаты (может быть как минимум два аддерсата - управляющая комната, текущая игровая комната
        void RUN(IPEndPoint ServerEndPoint); // запускает базовый NetProcessorAbs.RUN (очередь\reciver), коннектится к cерверу
        void RUN_GAME(); // запускает таймер переодической отправки клиентского состоянения игры на сервер
        IClientGameState ClientGameState { get;set }
        void OnClientGameStateChangedHandler(Object Sender, GameStateChangeData evntData);

    }

}
