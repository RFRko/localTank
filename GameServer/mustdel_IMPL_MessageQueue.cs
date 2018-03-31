using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Threading;

namespace Tanki
{
    class IMPL_MessageQueue
    {
    }


    //public abstract class MessageQueueAbs : IMessageQueue
    //{
    //    private MessageQueueAbs() { }
    //    public MessageQueueAbs(IServerEngine withEngine) { _serverEngine = withEngine; }

    //    protected IServerEngine _serverEngine;
    //    public abstract void RUN();
    //    public abstract void Enqueue(IProtocol msg);
    //    public abstract void Dispose();

    //}



    //public class MessageQueue_ProcessedOneByOne : MessageQueueAbs
    //{ 
    //    public MessageQueue_ProcessedOneByOne(IServerEngine withEngine, Object data = null) : base(withEngine) { }

    //    private Object _locker = new Object();
    //    private Object _locker_stopping = new Object();
    //    private AutoResetEvent _ifReady = new AutoResetEvent(false);
    //    private AutoResetEvent _proceedMsg = new AutoResetEvent(false);
    //    private AutoResetEvent _timer = new AutoResetEvent(false);

    //    //private MAK_MSG_proceed_method<T> _msg_proceed_method;

    //    private Queue<IProtocol> _msg_queue = new Queue<IProtocol>();
    //    private Thread _proceedingThread = null;
    //    private Boolean _enforceCancel = false;

    //    //public MAK_MSG_proceed_method<T> MsgProceedMethod { get { return _msg_proceed_method; } }
    //    public Thread ProceedingThread { get { return _proceedingThread; } }
    //    public Boolean EnforceCancel { get { return _enforceCancel; } set { _enforceCancel = value; } }

    //    public override void Enqueue(IProtocol newMsg)
    //    {
    //        lock (_locker)
    //        {
    //            _msg_queue.Enqueue(newMsg);

    //            var s = _proceedingThread.ThreadState;
    //            _ifReady.Set();
    //        }
    //    }

    //    public override void RUN()
    //    {
    //        _proceedingThread = new Thread(ProceedQueue);
    //        _proceedingThread.Name = "SERVER_MSG_PROCEEDING";
    //        _proceedingThread.Start();
    //    }

    //    private void ProceedQueue()
    //    {
    //        while (true)
    //        {
    //            IProtocol msg = null;

    //            lock (_locker)
    //            {
    //                if (_enforceCancel)
    //                    return;

    //                if (_msg_queue.Count > 0)
    //                    msg = _msg_queue.Dequeue();
    //            }

    //            if (msg != null)
    //            {
    //                //_serverEngine.ProcessMessage(msg); - НУЖНА ЕЩЕ РЕАЛИЗАЦИЯ ProcessMessage  c параметром 'просто единичный IProtocol'
    //            }
    //            else
    //                _ifReady.WaitOne();

    //        }

    //        //_proceedMsg.Set();
    //    }

    //    public override void Dispose()
    //    {
    //        lock (_locker_stopping)
    //        {
    //            _enforceCancel = true;
    //            _proceedingThread.Join();

    //            _ifReady.Close();
    //            _ifReady.Dispose();
    //            _proceedMsg.Close();
    //            _proceedMsg.Dispose();
    //            _msg_queue = null;
    //        }
    //    }
    //}



    //public class MessageQueue_ProcessedByTimer : MessageQueueAbs
    //{
    //    public MessageQueue_ProcessedByTimer(IServerEngine withEngine, Object data = null) : base(withEngine) { }

    //    private Object _locker = new Object();
    //    private Object _locker_stopping = new Object();
    //    private AutoResetEvent _ifReady = new AutoResetEvent(false);
    //    private AutoResetEvent _ifEnqueReady = new AutoResetEvent(false);
    //    private AutoResetEvent _ifDequeReady = new AutoResetEvent(false);

    //    //private AutoResetEvent _proceedMsg = new AutoResetEvent(false);
    //    private AutoResetEvent _finish_timer = new AutoResetEvent(false);

    //    //private MAK_MSG_proceed_method<T> _msg_proceed_method;
    //    private Queue<IProtocol> _msg_queue = new Queue<IProtocol>();
    //    private Thread _proceedingThread = null;
    //    private Boolean _enforceCancel = false;

    //    //public MAK_MSG_proceed_method<T> MsgProceedMethod { get { return _msg_proceed_method; } }
    //    //public Thread ProceedingThread { get { return _proceedingThread; } }
    //    public Boolean EnforceCancel { get { return _enforceCancel; } set { _enforceCancel = value; } }

    //    private Timer _timer;

    //    public override void Enqueue(IProtocol newMsg)
    //    {
    //        lock (_locker)
    //        {
    //            _ifEnqueReady.WaitOne();
    //            _ifDequeReady.Reset();
    //            lock(_msg_queue)
    //            {
    //                _ifReady.Set();
    //            }
    //            _ifDequeReady.Set();
    //            //var s = _proceedingThread.ThreadState;                
    //        }
    //    }

    //    public override void RUN()
    //    {
    //        //_proceedingThread = new Thread(ProceedQueue);
    //        //_proceedingThread.Name = "SERVER_MSG_PROCEEDING";
    //        //_proceedingThread.Start();
    //        _ifEnqueReady.Set();
    //        _timer = new Timer(ProceedQueue,_ifReady, 0, 1000);
    //        _finish_timer.WaitOne();            
    //    }

    //    private void ProceedQueue(Object state)
    //    {
    //        IProtocol msg = null;
    //        List<IProtocol> recieved_packages_batch = new List<IProtocol>();

    //        lock (_locker)
    //        {
    //            if (_enforceCancel)
    //            {
    //                _finish_timer.Set();
    //                return;
    //            }

    //            _ifReady.WaitOne();

    //            _ifDequeReady.WaitOne();
    //            _ifEnqueReady.Reset();
    //            while (_msg_queue.Count>0)
    //            {
    //                msg = _msg_queue.Dequeue();
    //                recieved_packages_batch.Add(msg);
    //            }
    //            _ifEnqueReady.Set();
    //        }

    //        if (recieved_packages_batch != null)
    //        {
    //            _serverEngine.ProcessMessage(recieved_packages_batch);
    //        }
    //        else
    //            _ifReady.WaitOne();

    //    }


    //    public override void Dispose()
    //    {
    //        lock (_locker_stopping)
    //        {
    //            _enforceCancel = true;
    //            //_proceedingThread.Join();
    //            _timer.Dispose();

    //            _ifReady.Close();
    //            _ifReady.Dispose();

    //            _ifEnqueReady.Close();
    //            _ifEnqueReady.Dispose();

    //            _ifDequeReady.Close();
    //            _ifDequeReady.Dispose();

    //            _msg_queue = null;
    //        }
    //    }
    //}


    //public class MessageQueueFabric : IMessageQueueFabric
    //{
    //    public IMessageQueue CreateMessageQueue(MsgQueueType queueType,IServerEngine withEngine)
    //    {
    //        IMessageQueue instance = null;

    //        switch (queueType)
    //        {
    //            case MsgQueueType.mqByTimerProcc:
    //                instance = new MessageQueue_ProcessedByTimer(withEngine);
    //                break;
    //            case MsgQueueType.mqOneByOneProcc:
    //            default:
    //                instance = new MessageQueue_ProcessedOneByOne(withEngine);
    //                break;
    //        }

    //        return instance;
    //    }
    //}




}
