﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanki
{

    public class RegMsgQueueData : EventArgs
    {
        public IMessageQueueClient MsgQueueOwner { get; set; }
    }

    public class RegEngineData : EventArgs
    {
        public IEngineClient EngineOwner { get; set; }
    }

}
