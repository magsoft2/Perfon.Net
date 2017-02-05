using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using PerfMonLib.Core.PerfCounterStorages;
using PerfMonLib.WebApi;

namespace PerfMonLib.WebApi
{
    /// <summary>
    /// Web Api Controller returns html UI for display of performance counters
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "GET")]
    //[RoutePrefix(RouteConstants.RouteStart_Api+"/test")]
    [AuthPerfMonitorLibActions]
    public class PerfCountersUIController : ApiController
    {
        public IPerfomanceCountersStorage Db
        {
            get
            {
                //How to deal with DI here??
                return (this.ControllerContext.Configuration.Properties["PerfMonitorLib"] as PerfMonitorForWebApi).Storage;
            }
        }

        private static Lazy<string> GetUI = new Lazy<string>(() =>
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "PerfMonLib.WebApi.UI.PerfCountersUI.html";

                string result = "";

                try
                {
                    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            result = reader.ReadToEnd();
                        }
                    }
                }
                catch (Exception exc)
                {
                    result = exc.ToString();
                }

                return result;
            });
        


        /// <summary>
        /// Get UI
        /// </summary>
        /// <returns></returns>
        //[Route("api/perfmonitor/counters")]        
        //[HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent(GetUI.Value);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            
            return response;
        }
    }
}