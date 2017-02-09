using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Perfon.Core;


namespace Perfon.Mvc.Filters
{

    public class ExceptionCounterFilter : FilterAttribute, IExceptionFilter
    {
        private PerfMonitor PerfMonitor {get;set;}


        public ExceptionCounterFilter(PerfMonitor perfMonitor)
        {
            PerfMonitor = perfMonitor;
        }


        public void OnException(ExceptionContext exceptionContext)
        {
            PerfMonitor.ExceptionsNum.Increment();
        }
    }
}