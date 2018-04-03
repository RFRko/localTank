using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Tanki
{
    public class Gamer : IGamer,IDisposable
    {
        public Gamer(IPEndPoint ep)
        {
            RemoteEndPoint = ep;
            Passport = GuidGenTimeBased.GenGuid(ep.Address.ToString().GetHashCode(), ep.Port);
        }
        public string Name { get; set; }

        public Guid Passport { get; private set; }
        public IPEndPoint RemoteEndPoint { get; private set; }

        public void SetId(string newID, Guid confirmpassport)
        {
            if (Passport != confirmpassport)
                Dispose();
            Name = newID;
        }

        public void Dispose()
        {
            RemoteEndPoint  = null;
        }

    }
}
