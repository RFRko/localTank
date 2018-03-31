using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Tanki
{
	/// <summary> Бинарный сериализатор. Используется в ISender, IReceiver. </summary>
	public class BinSerializator : ISerializator
	{
		/// <summary> Де-Сериализатор </summary>
		/// <param name="bytes"> Масив байт требующий Де-Сериализации</param>
		/// <returns> IPackage </returns>
		public IPackage Deserialize(byte[] bytes)
		{
			MemoryStream memStream = new MemoryStream();
			BinaryFormatter binForm = new BinaryFormatter();
			memStream.Write(bytes, 0, bytes.Length);
			memStream.Seek(0, SeekOrigin.Begin);
			Package obj = (Package)binForm.Deserialize(memStream);

			return obj;
		}



		/// <summary> Сериализатор </summary>
		/// <param name="obj"> Объект требующий Сериализации</param>
		/// <returns> Возвращает массив байт </returns>
		public byte[] Serialize(object obj)
		{
			if (obj == null)
				return null;
			BinaryFormatter bf = new BinaryFormatter();
			using (MemoryStream ms = new MemoryStream())
			{
				bf.Serialize(ms, obj);
				return ms.ToArray();
			}
		}
	}
}
