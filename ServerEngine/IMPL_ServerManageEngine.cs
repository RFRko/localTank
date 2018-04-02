using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanki
{
    public class ServerManageEngine:EngineAbs
    {
        public ServerManageEngine():base() { }
        public ServerManageEngine(IRoom inRoom) : base(inRoom)
        {
            this.ProcessMessage = ProcessMessage;
            this.ProcessMessages = null;
        }

        public override ProcessMessageHandler ProcessMessage { get; protected set; }
        public override ProcessMessagesHandler ProcessMessages { get; protected set; }

        public override void OnNewAddresssee_Handler(object Sender, NewAddressseeData evntData)
        {
            var v = evntData.newAddresssee;


            //
        }

        private void ProcessMessageHandler(IPackage msg)
        {
            

            // нужно реализовать обработку управляющих  сообщений клиет-сервер
        }
    }
}
