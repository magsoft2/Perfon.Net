using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public void Start(HttpConfiguration httpConfiguration, int pollPeriod_ms)
        {
            PerfMonitorBase.Start(pollPeriod_ms);

            httpConfiguration.Filters.Add(new ExceptionCounterFilter(this.PerfMonitorBase));

            httpConfiguration.MessageHandlers.Add(new RequestPerfMonitorMessageHandler(this.PerfMonitorBase));

            //httpConfiguration.Services.Add(typeof(PerfMonitorForWebApi), this);

            httpConfiguration.Properties[EnumKeyNames.PerfMonitorLib.ToString()] = this;

            //httpConfiguration.MapHttpAttributeRoutes();
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
