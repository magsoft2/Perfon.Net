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
    /// Used for LiteDb Storage
    /// </summary>
    public class PerfCountersDoc
    {
        public PerfCountersDoc() {

            Values = new List<PerfCounterLiteDbDataBlock>();
        }

        public string CounterName { get; set; }

        public List<PerfCounterLiteDbDataBlock> Values { get; set; }

        public int _id { get; set; }

        public short blockId { get; set; }
    }


  
}
