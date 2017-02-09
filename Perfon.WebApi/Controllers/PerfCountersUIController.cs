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
using Perfon.Core;
using Perfon.Core.PerfCounterStorages;
using Perfon.WebApi;

namespace Perfon.WebApi
{
    /// <summary>
    /// Web Api Controller returns html UI for display of performance counters
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "GET")]
    [AuthPerfMonitorLibActions]
    //[RoutePrefix("api/perfcountersui")]
    public class PerfCountersUIController : ApiController
    {
        private PerfMonitorForWebApi PerfMonitorLib
        {
            get
            {
                //How to deal with DI here??
                return (this.ControllerContext.Configuration.Properties[EnumKeyNames.PerfMonitorLib.ToString()] as PerfMonitorForWebApi);
            }
        }

        
        


        /// <summary>
        /// Get UI as full html page
        /// </summary>
        /// <returns></returns>
        [Route("api/perfcountersui")]    
        [HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent(PerfMonitorLib.UIPage);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            
            return response;
        }

        /// <summary>
        /// Get UI as div panel only.
        /// Foe embedding in your pages
        /// </summary>
        /// <returns></returns>
        [Route("api/perfcountersuipanel")]  
        [HttpGet]
        public async Task<HttpResponseMessage> GetPanel()
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent(PerfMonitorLib.UIPanel);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            return response;
        }
    }
}