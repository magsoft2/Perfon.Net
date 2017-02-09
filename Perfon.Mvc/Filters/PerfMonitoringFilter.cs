using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Perfon.Core;

namespace Perfon.Mvc.Filters
{
    public class PerfMonitoringFilter : ActionFilterAttribute
    {
        private PerfMonitor PerfMonitor {get;set;}


        public PerfMonitoringFilter(PerfMonitor perfMonitor)
        {
            PerfMonitor = perfMonitor;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            filterContext.HttpContext.Response.Filter = new ResponseLengthCalculatingStream(filterContext.HttpContext.Response.Filter, PerfMonitor);

            var request = filterContext.HttpContext.Request;

            var st = Stopwatch.StartNew();

            PerfMonitor.RequestNum.Increment();

            base.OnActionExecuting(filterContext);

            filterContext.HttpContext.Items["stopwatch"] = st;


            long lenReq = 0;

            lenReq += request.TotalBytes;
            lenReq += request.RawUrl.Length;
            lenReq += request.Headers.ToString().Length;
            PerfMonitor.BytesTrasmittedReq.Add(lenReq);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);

            var res = filterContext.HttpContext.Response;

            long lenResp = 0;
            lenResp += res.Headers.ToString().Length;
            PerfMonitor.BytesTrasmittedResp.Add(lenResp);

            if (res.StatusCode < 200 || res.StatusCode > 202)
            {
                PerfMonitor.BadStatusNum.Increment();
            }

            var st = filterContext.HttpContext.Items["stopwatch"] as Stopwatch;

            st.Stop();

            PerfMonitor.RequestProcessTime.Add(st.ElapsedMilliseconds);
            PerfMonitor.RequestMaxProcessTime.Add(st.ElapsedMilliseconds);


        }
    }
}