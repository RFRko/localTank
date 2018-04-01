using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanki
{
    public class GuidGenTimeBased
    {
        private static void TimeBasedPartGen(ref Byte[] guid_source)
        {
            if (guid_source.Length != 16) throw new Exception("GUID source bytes array must be 16Bytes length..");

            DateTime now = DateTime.Now;

            Int32 YYYYMMDD = Int32.Parse(now.Year.ToString() +
                                        (now.Month < 10 ? "0" : "") + now.Month.ToString() +
                                        (now.Day < 10 ? "0" : "") + now.Day.ToString());
            Int16 HHMM = Int16.Parse(DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString());

            Int16 _msec = (Int16)(DateTime.Now.Second * 1000 + DateTime.Now.Millisecond);

            Int16 MSEC = (Int16)(DateTime.Now.Second * 1000 + DateTime.Now.Millisecond);//Int16.Parse((DateTime.Now.Second * 1000 + DateTime.Now.Millisecond).ToString());


            Int32 targ_offset = 0;

            Byte[] guid_source_part = BitConverter.GetBytes(YYYYMMDD);
            Buffer.BlockCopy(guid_source_part, 0, guid_source, targ_offset, guid_source_part.Length);
            targ_offset += guid_source_part.Length;

            guid_source_part = BitConverter.GetBytes(HHMM);
            Buffer.BlockCopy(guid_source_part, 0, guid_source, targ_offset, guid_source_part.Length);
            targ_offset += guid_source_part.Length;

            guid_source_part = BitConverter.GetBytes(MSEC);
            Buffer.BlockCopy(guid_source_part, 0, guid_source, targ_offset, guid_source_part.Length);
            targ_offset += guid_source_part.Length;

        }

        public static Guid GenGuid(Int32 node_lword, Int32 node_rword)
        {

            Byte[] guid_source = new Byte[16];

            TimeBasedPartGen(ref guid_source);
            SetGuidNode(ref guid_source, node_lword, node_rword);

            return new Guid(guid_source);
        }
        public void SetGuidNode(ref Guid GUIDOBJ, Int32 node_lword, Int32 node_rword)
        {

            Byte[] guid_content = GUIDOBJ.ToByteArray();

            SetGuidNode(ref guid_content, node_lword, node_rword);
            GUIDOBJ = new Guid(guid_content);
        }

        private static void SetGuidNode(ref Byte[] guid_content, Int32 node_lword, Int32 node_rword)
        {
            Int32 nodePos = 8;

            Int32 targ_offset = nodePos;

            Byte[] node_word = BitConverter.GetBytes(node_lword);
            Buffer.BlockCopy(node_word, 0, guid_content, targ_offset, node_word.Length);
            targ_offset += node_word.Length;

            node_word = BitConverter.GetBytes(node_rword);
            Buffer.BlockCopy(node_word, 0, guid_content, targ_offset, node_word.Length);
            targ_offset += node_word.Length;
        }

    }
}
