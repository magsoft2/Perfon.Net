using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Perfon.Core.PerfCounterStorages;
using Perfon.WebApi;

namespace Perfon.WebApi
{
    /// <summary>
    /// Web Api Controller for retrieving perf counters, available by default for PerfLib clients
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "GET")]
    [AuthPerfMonitorLibActions]
    public class PerfCountersController : ApiController
    {
        readonly string keyList = "PerfListCounters";
            

        public IPerfomanceCountersStorage Db
        {
            get
            {
                //How to deal with DI here??
                return (this.ControllerContext.Configuration.Properties["PerfMonitorLib"] as PerfMonitorForWebApi).Storage;
            }
        }


        /// <summary>
        /// Get counters list
        /// </summary>
        /// <returns></returns>
        [Route("api/perfcounters")]        
        public async Task<IHttpActionResult> Get()
        {
            var res = MemoryCache.Default.Get(keyList) as IEnumerable<string>;

            if (res == null)
            {
                // Not the best solution. Use Lazy here??
                res = await Db.GetCountersList();
                MemoryCache.Default.Add(keyList, res, new DateTimeOffset(DateTime.Now.AddSeconds(10)));
            }

            return Ok(res);
        }

        /// <summary>
        /// Get perf counter values history track
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Route("api/perfcounters")] ///{*name}
        public async Task<IHttpActionResult> Get([FromUri]string name, [FromUri]DateTime? date = null, [FromUri]int? skip = null)
        {
            string key = name+date.GetHashCode();
            var res = MemoryCache.Default.Get(key) as IEnumerable<PerfCounterValue>;

            if (res == null)
            {
                // Not the best solution. Use Lazy here??
                res = await Db.QueryCounterValues(name, date);
                MemoryCache.Default.Add(key, res, new DateTimeOffset(DateTime.Now.AddSeconds(2)));
            }

            if (res != null && skip.HasValue)
            {
                res = res.Skip(skip.Value);
            }

            return Ok(res);
        }

    }
}