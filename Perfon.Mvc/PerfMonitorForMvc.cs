using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Perfon.Core;
using Perfon.Core.PerfCounterStorages;
using Perfon.Mvc.Filters;

namespace Perfon.Mvc
{
    /// <summary>
    /// Wrapper on PerfMonitor for Asp.Net MVC 5
    /// It registers filters allowing to track request counters.
    /// It sets own route as the first route
    /// </summary>
    public class PerfMonitorForMvc
    {
        public PerfMonitor PerfMonitorBase { get; private set; }

        /// <summary>
        /// Settings for Perfon engine
        /// </summary>
        public PerfonConfiguration Configuration
        {
            get
            {
                return PerfMonitorBase.Configuration;
            }
        }

        /// <summary>
        /// Reports about errors and exceptions occured.
        /// </summary>
        public event EventHandler<Perfon.Core.ErrorEventArgs> OnError;


        public PerfMonitorForMvc()
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
        /// <summary>
        /// Easy register some default perf counter storages implemented in the lib, if needed
        /// </summary>
        /// <param name="dbPath"></param>
        public void RegisterCSVFileStorage(string dbPath)
        {
            Storage = PerfMonitorBase.RegisterCSVFileStorage(dbPath);
        }
        public void RegisterLiteDbStorage(string dbPath)
        {
            Storage = PerfMonitorBase.RegisterLiteDbStorage(dbPath);
        }
        public void RegisterInMemoryCacheStorage(long expirationInSeconds=60*60)
        {
            Storage = PerfMonitorBase.RegisterInMemoryCacheStorage(expirationInSeconds);
        }
        /// <summary>
        /// Start polling and saving perf counters. Period is in ms
        /// </summary>
        /// <param name="pollPeriod_ms">Poll period, ms</param>
        public void Start(HttpApplication app, RouteCollection routes, int pollPeriod_ms)
        {
            PerfMonitorBase.Start(pollPeriod_ms);

            app.Application[EnumKeyNames.PerfMonitorLib.ToString()] = this;

            if (Configuration.EnablePerfApi)
            {
                var r = routes.MapRoute(
                    name: "PerfMonitor",
                    url: "api/perfcounters/",
                    defaults: new { controller = "PerfCounters", action = "Get" }
                );
                routes.Remove(r);
                routes.Insert(0, r);
            }

            GlobalFilters.Filters.Add(new PerfMonitoringFilter(PerfMonitorBase));
            GlobalFilters.Filters.Add(new ExceptionCounterFilter(PerfMonitorBase));

        }

        

        /// <summary>
        /// Stops perf counters polling
        /// </summary>
        public void Stop()
        {
            PerfMonitorBase.Stop();
        }


        public string UIPage
        {
            get
            {
                return PerfMonitorBase.UIPage.Value;
            }
        }
        public string UIPanel
        {
            get
            {
                return PerfMonitorBase.UIPanel;
            }
        }


        /// <summary>
        /// Need to be removed
        /// Inject it into controller!
        /// </summary>
        public IPerfomanceCountersStorage Storage { get; private set; }
        
    }
}
