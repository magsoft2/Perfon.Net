using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
using Perfon.Core;
using Perfon.Core.PerfCounterStorages;

namespace Perfon.WebApi
{
    /// <summary>
    /// Wrapper on PerfMonitor for Web Api.
    /// It registers filters and handlers thus tracking request counters.
    /// </summary>
    public class PerfMonitorForWebApi
    {
        public PerfMonitor PerfMonitorBase { get; private set; }

        public bool IsPerfCountersControllerEnabled { get; set; }

        /// <summary>
        /// Reports about errors and exceptions occured.
        /// </summary>
        public event EventHandler<ErrorEventArgs> OnError;


        public PerfMonitorForWebApi()
        {
            PerfMonitorBase = new PerfMonitor();

            //Bubble up errors
            PerfMonitorBase.OnError += (a,b)=>
            {
                if (OnError != null)
                {
                    OnError(a,b);
                }
            };
        }

        /// <summary>
        /// Register perf counter storages
        /// </summary>
        /// <param name="storage"></param>
        public void RegisterStorages(params IPerfomanceCountersStorage[] storage)
        {
            Storage = PerfMonitorBase.RegisterStorages(storage);
        }
        public void RegisterCSVFileStorage(string filePathName)
        {
            Storage = PerfMonitorBase.RegisterCSVFileStorage(filePathName);
        }
        public void RegisterLiteDbStorage(string dbPathName)
        {
            Storage = PerfMonitorBase.RegisterLiteDbStorage(dbPathName);
        }
        /// <summary>
        /// Start polling and saving perf counters. Period is in ms
        /// </summary>
        /// <param name="pollPeriod_ms">Poll period, ms</param>
        public void Start(HttpConfiguration httpConfiguration, int pollPeriod_ms, int doNotStorePerfCountersIfReqLessOrEqThan = -1, 
                                            bool enablePerfApiAndUI = false, int? minPollPeriod = null, int? maxPollPeriod = null)
        {
            PerfMonitorBase.Start(pollPeriod_ms, doNotStorePerfCountersIfReqLessOrEqThan, minPollPeriod, maxPollPeriod);

            httpConfiguration.Filters.Add(new ExceptionCounterFilter(this.PerfMonitorBase));

            httpConfiguration.MessageHandlers.Add(new RequestPerfMonitorMessageHandler(this.PerfMonitorBase));

            //httpConfiguration.Services.Add(typeof(PerfMonitorForWebApi), this);

            IsPerfCountersControllerEnabled = enablePerfApiAndUI;

            httpConfiguration.Properties["PerfMonitorLib"] = this;

            //var dic = new HttpRouteValueDictionary();
            //dic.Add("controller", "PerfMonitorController");
            //httpConfiguration.Routes.Insert(0, "PerfMonitorRoute", new HttpRoute(
            ////name: "PerfMonitorRoute",
            //routeTemplate: "api/perfmonitor2",
            //defaults: dic, //new HttpRouteValueDictionary {  controller = "PerfMonitorController"},
            //constraints: null
            ////handler: new RoutSpecificHandler { InnerHandler = new HttpControllerDispatcher(config) }
            //));

        }
        /// <summary>
        /// Stops perf counters polling
        /// </summary>
        public void Stop()
        {
            PerfMonitorBase.Stop();
        }

        /// <summary>
        /// Need to be removed
        /// Inject it into controller!
        /// </summary>
        internal IPerfomanceCountersStorage Storage { get; private set; }
    }
}
