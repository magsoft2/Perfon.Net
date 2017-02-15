using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Perfon.Core;
using Perfon.Core.PerfCounterStorages;
using Perfon.Interfaces.PerfCounterStorage;
using Perfon.Mvc;

namespace Perfon.Mvc.Controllers
{
	public class PerfCountersController : Controller
	{
		readonly string keyList = "PerfListCounters";


		private IPerfomanceCountersStorage Db
		{
			get
			{
				//How to deal with DI here??
				return (this.HttpContext.Application[EnumKeyNames.PerfMonitorLib.ToString()] as PerfMonitorForMvc).Storage;
			}
		}


		/// <summary>
		/// Get counters list
		/// </summary>
		/// <returns></returns>
		public async Task<ActionResult> Get()
		{
			var requestQueryString = HttpContext.Request.QueryString;

			var name = requestQueryString["name"];

			if (string.IsNullOrEmpty(name))
			{

				var res = MemoryCache.Default.Get(keyList) as IEnumerable<string>;

				if (res == null || res.Count() <= 0)
				{
					// Not the best solution. Use Lazy here??
					res = await Db.GetCountersList();
					MemoryCache.Default.Add(keyList, res, new DateTimeOffset(DateTime.Now.AddSeconds(60)));
				}

				return Json(res, JsonRequestBehavior.AllowGet);
			}
			else
			{
				DateTime? date = null;
				int? skip = null;
				if(!string.IsNullOrEmpty(requestQueryString["date"]))
				{
					date = DateTime.Parse(requestQueryString["date"]);
				}
				if (!string.IsNullOrEmpty(requestQueryString["skip"]))
				{
					skip = int.Parse(requestQueryString["skip"]);
				}

				return await GetTrack(name, date, skip);
			}
		}

		/// <summary>
		/// Get perf counter values history track
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private async Task<ActionResult> GetTrack(string name, DateTime? date = null, int? skip = null, string appId = null)
		{
			int skip2 = 0;
			if (skip.HasValue)
			{
				skip2 = skip.Value;
			}

            string key = name + date.GetHashCode() + skip2.GetHashCode();
			var res = MemoryCache.Default.Get(key) as IEnumerable<IPerfCounterValue>;

			if (res == null || res.Count() <= 0)
			{
				// Not the best solution. Use Lazy here??
				res = await Db.QueryCounterValues(name, date, skip2, appId);
                if (res != null)
                {
                    MemoryCache.Default.Add(key, res, new DateTimeOffset(DateTime.Now.AddSeconds(3)));
                }
			}

			return new CustomJsonResult { Data = res };
			//return new CustomJsonResult(res, JsonRequestBehavior.AllowGet);
		}
	}
}