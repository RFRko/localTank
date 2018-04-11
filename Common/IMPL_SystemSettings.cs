using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanki
{
    public class SystemSettings : ISystemSettings
    {
        public int HostListeningPort { get; set; }
        public int RoomPortMin { get; set; }
        public int RoomPortMax { get; set; }
        public int ClientPortMin { get; set; }
        public int ClientPortMax { get; set; }
        public int MaxRoomNumber { get; set; }
    }
}
