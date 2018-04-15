using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanki
{

    public abstract class NetProcessorAbs : INetProcessor
    {
        public NetProcessorAbs() { }
        public NetProcessorAbs(IMessageQueue msgQueue, IEngine engine, IReciever reciever)
        {
            RegisterDependcy(msgQueue);
            RegisterDependcy(engine);
            RegisterDependcy(reciever);
        }

        public IReciever Reciever { get; protected set; }
        public IEngine Engine { get; protected set; }
        public IMessageQueue MessageQueue { get; protected set; }
        public ISender Sender { get; protected set; }

        public event EventHandler<RegMsgQueueData> OnRegisterMessageQueue;
        public event EventHandler<RegEngineData> OnRegisterEngine;
        public event EventHandler<RegRecieverData> OnRegisterReciever;
        public event EventHandler<NetProcStartedEvntData> OnNetProcessorStarted;

        public void RegisterDependcy(IMessageQueue regMsqQueue)
        {
            OnRegisterMessageQueue += regMsqQueue.OnRegistered_EventHandler;
            OnRegisterMessageQueue?.Invoke(this, new RegMsgQueueData() { MsgQueueOwner = this });
            OnRegisterMessageQueue -= regMsqQueue.OnRegistered_EventHandler;
            if (regMsqQueue.Owner == this)
                MessageQueue = regMsqQueue;
        }

        public void RegisterDependcy(IEngine regEngine)
        {
            OnRegisterEngine += regEngine.OnRegistered_EventHandler;
            OnRegisterEngine?.Invoke(this,  new RegEngineData { EngineOwner = this });
            OnRegisterEngine -= regEngine.OnRegistered_EventHandler;
            if (regEngine.Owner == this)
            {
                Engine = regEngine;
                OnNetProcessorStarted += Engine.OnNetProcStarted_EventHandler;
            }
        }

        public void RegisterDependcy(IReciever regReciever)
        {
            OnRegisterReciever += regReciever.OnRegistered_EventHandler;
            OnRegisterReciever?.Invoke(this, new RegRecieverData() { owner = this });
            OnRegisterReciever -= regReciever.OnRegistered_EventHandler;
            if (regReciever.Owner == this)
                Reciever = regReciever;
        }

        public void RUN()
        {
            if (MessageQueue == null || MessageQueue.Owner != this) throw new Exception("MessageQueue is NULL or not registered");
            if (Engine == null || Engine.Owner != this) throw new Exception("Engine is NULL or not registered");

            this.MessageQueue.RUN();
            this.Reciever.Run();

            OnNetProcessorStarted?.BeginInvoke(this, new NetProcStartedEvntData() { Started = true }, null, null);
        }
    }
}
