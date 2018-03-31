using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tanki
{
	//заглушка
	public interface IRoom
	{
		IEnumerable<IGamer> gamerList { get; }
	}

	public delegate void ProcessMessageHandler(IEnumerable<IPackage> list);
	public interface IServerEngine
	{
		ProcessMessageHandler ProcessMessage { get; }
		IMap Send();
	}
}
