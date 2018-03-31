using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanki
{
	[Serializable]
	class Package : IPackage
	{
		public string Sender_id { get; set; }
		public object Data { get; set; }
	}
}
