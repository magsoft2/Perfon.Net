using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http.Filters;
using PerfMonLib.Core;

namespace PerfMonLib.WebApi
{
    /// <summary>
    /// Web Api IFilter implemenation for tracking number of exceptions occured
    /// </summary>
    public class ExceptionCounterFilter : ExceptionFilterAttribute
    {
        /// <summary>
        /// Core PerfMon object managing perfCounters
        /// </summary>
        private PerfMonitor PerfMonitor {get;set;}


        public ExceptionCounterFilter(PerfMonitor perfMonitor)
        {
            PerfMonitor = perfMonitor;
        }


        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            PerfMonitor.ExceptionsNum.Increment();
        }
    }
}