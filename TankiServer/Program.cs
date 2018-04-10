using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanki
{
    class Program
    {
        static void Main(string[] args)
        {

            IListener listener = new Listener(11001);
            IServer Srv = new GameServer(listener);
            Srv.RUN();




        }
    }
}
