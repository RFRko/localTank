using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanki
{
	[Serializable]
	public class Package : IPackage
	{
		public Guid Sender_Passport { get; set; }
		public object Data { get; set; }
		public MesseggeType MesseggeType { get; set; }
	}
}
