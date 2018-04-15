using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanki
{
    public class RoomIsFullException: Exception
    {
        public RoomIsFullException() : base("Room is Full..") { }
    }
}
