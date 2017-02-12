using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Perfon.Interfaces.PerfCounterStorage;

namespace Perfon.Core.PerfCounterStorages.LiteDbStorage
{
    /// <summary>
    /// Used for LiteDb Storage.
    /// Pack\restore perf data block into byte array
    /// </summary>
    public class PerfCounterLiteDbDataBlock
    {
        /// <summary>
        /// Used only by LiteDb
        /// </summary>
        public PerfCounterLiteDbDataBlock() { }

        /// <summary>
        /// Use this ctor for creating PerfCounterValue3 which should be passed to LiteDb
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="value"></param>
        public PerfCounterLiteDbDataBlock(DateTime timestamp, float value)
        {
            //v = new byte[5];

            var time = (uint)(timestamp.TimeOfDay.TotalMilliseconds / 100);
            var val = (uint)(value * 10f);

            uint val20 = time;
            uint numberOfms20 = val;

            ulong packed = ((ulong)val20) | ((ulong)numberOfms20 << 20);

            v = BitConverter.GetBytes(packed);
            Array.Resize(ref v, 5);
        }

        public byte[] V
        {
            get
            {
                return v;
            }
            set
            {
                v = value;
            }
        }

        [BsonIgnore]
        private byte[] v;


        /// <summary>
        /// Get perf counter data block after restoring this class from LiteDb
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [BsonIgnore]
        public IPerfCounterValue GetValue(DateTime date)
        {
            byte[] newArray = new byte[8];
            v.CopyTo(newArray, 0);

            var packed = BitConverter.ToInt64(newArray, 0);
            var val = (uint)(packed & ((1 << 20) - 1));
            var bits20 = (uint)((packed >> 20) & ((1 << 20) - 1));

            return new PerfCounterValue(date.AddMilliseconds(val * 100).ToUniversalTime(), (float)bits20 / 10.0f);
        }
    }
}
