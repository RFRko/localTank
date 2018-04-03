using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanki
{
	[Serializable]
	public class ConectionData : IConectionData
	{
		public string PlayerName { get; set; }
		public Guid Pasport { get; set; }
	}
}
