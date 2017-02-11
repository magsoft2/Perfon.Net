using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace Perfon.Core.PerfCounterStorages
{
    /// <summary>
    /// </summary>
    public class PerfCountersDoc
    {
        public PerfCountersDoc() {

            Values = new List<PerfCounterValue3>();
        }

        public string CounterName { get; set; }

        public List<PerfCounterValue3> Values { get; set; }

        public int _id { get; set; }
    }


    public class PerfCounterValue3
    {
        public PerfCounterValue3(){}

        public PerfCounterValue3(DateTime timestamp, float value)
        {
            //v = new byte[5];

            var time = (uint)(timestamp.ToUniversalTime().TimeOfDay.TotalMilliseconds / 100);
            var val = (uint)(value * 10f);

            uint val20 = time;
            uint numberOfms20 = val;

            ulong packed = ((ulong)val20) | ((ulong)numberOfms20 << 20);

            v = BitConverter.GetBytes(packed);
            Array.Resize(ref v, 5);
        }

        public byte[] V 
        {
            get{ 
                return v;
            }
            set{ 
                v= value;
            }
        }

        [BsonIgnore]
        private byte[] v;


        [BsonIgnore]
        public PerfCounterValue GetValue(DateTime date)
        {
            byte[] newArray = new byte[8];
            v.CopyTo(newArray, 0);

            var packed = BitConverter.ToInt64(newArray, 0);
            var val = (uint)(packed & ((1 << 20) - 1));
            var bits20 = (uint)((packed >> 20) & ((1 << 20) - 1));

            return new PerfCounterValue(date.AddMilliseconds(val * 100), (float)bits20/10.0f);
        }
    }
}
