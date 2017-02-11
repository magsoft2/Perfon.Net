using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace Perfon.Core.PerfCounterStorages
{
    /// <summary>
    /// Used for obtaining perf counter time series
    /// 
    /// </summary>
    public class PerfCounterValue2
    {
        //public PerfCounterValue()
        //{

        //}

        public PerfCounterValue2() { }
        public PerfCounterValue2(BsonDocument bs) {
            _id = bs["_id"].AsInt32;
            V = bs["V"].AsInt32;
        }

        public PerfCounterValue2(DateTime timestamp, float value):this()
        {
            //Timestamp = timestamp;
            //Value = value;
            _id = (int)(timestamp.ToUniversalTime().TimeOfDay.TotalMilliseconds / 100);
            V = (int)(value * 10f);
        }

        public int _id  { get; set; }
        public int V { get; set; }


        public float GetValue()
        {
            return V / 10f;
        }
        public DateTime GetFullTimeStamp(DateTime date)
        {
            return date.AddMilliseconds(_id * 100);
        }
    }
}
