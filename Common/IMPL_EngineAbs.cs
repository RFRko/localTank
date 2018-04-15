using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanki
{
    public abstract class EngineAbs : IEngine
    {
        public EngineAbs() {}
        public EngineAbs(IEngineClient forClient) { Owner = forClient; }

        public abstract ProcessMessageHandler ProcessMessage { get; protected set; }
        public abstract ProcessMessagesHandler ProcessMessages { get; protected set; }
        public IEngineClient Owner { get; protected set; }

        
        public abstract void OnNetProcStarted_EventHandler(object Sender, NetProcStartedEvntData evntData);
		public abstract void OnNewAddresssee_Handler(object Sender, NewAddressseeData evntData);
		public abstract void OnAddressseeHolderFull_Handler(object Sender, AddressseeHolderFullData evntData);
        public abstract void OnBeforNetProcStarted_EventHandler(object Sender, NetProcBeforStartedEvntData evntData);


        public virtual void OnRegistered_EventHandler(object Sender, RegEngineData evntData)
        {
            Owner = evntData.EngineOwner;

            var addrHolder = Owner as IAddressseeHolderBase;

			if (addrHolder != null)
			{
				addrHolder.OnNewAddresssee += OnNewAddresssee_Handler;
                addrHolder.OnAddressseeHolderFull += OnAddressseeHolderFull_Handler;
            }

            Owner.OnNetProcessorBeforStarted += OnBeforNetProcStarted_EventHandler;
            Owner.OnNetProcessorStarted += OnNetProcStarted_EventHandler;


        }

    }

}
