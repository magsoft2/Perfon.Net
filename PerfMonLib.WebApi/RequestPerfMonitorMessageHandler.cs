using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using PerfMonLib.Core;

namespace PerfMonLib.WebApi
{
    /// <summary>
    /// Web Api MessageHandler for tracking number of requests and request processing time.
    /// </summary>
    public class RequestPerfMonitorMessageHandler : DelegatingHandler
    {
         /// <summary>
        /// Core PerfMon object managing perfCounters
        /// </summary>
        private PerfMonitor PerfMonitor {get;set;}


        public RequestPerfMonitorMessageHandler(PerfMonitor perfMonitor)
        {
            PerfMonitor = perfMonitor;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var st = Stopwatch.StartNew();

            PerfMonitor.RequestNum.Increment();

            var res = await base.SendAsync(request, cancellationToken);

            long lenReq = 0;
            if (request.Content != null)
            {
                if (request.Content.Headers.ContentLength.HasValue)
                {
                    lenReq = request.Content.Headers.ContentLength.Value;
                }
                lenReq += request.Content.Headers.ToString().Length;
            }
            lenReq += request.RequestUri.OriginalString.Length;
            lenReq += request.Headers.ToString().Length;
            PerfMonitor.BytesTrasmittedReq.Add(lenReq);

            /*
            SO: Don't be concerned by the LoadIntoBufferAsync, unless you actually do streaming content then almost all content is 
             * buffered by the host anyway, so doing it a little earlier in the pipeline will not add any extra overhead.
            */
            long lenResp = 0;
            if (res.Content != null)
            {
                await res.Content.LoadIntoBufferAsync();
                if (res.Content.Headers.ContentLength.HasValue)
                {
                    lenResp = res.Content.Headers.ContentLength.Value;
                }
                lenResp += res.Content.Headers.ToString().Length;
            }
            lenResp += res.Headers.ToString().Length;
            PerfMonitor.BytesTrasmittedResp.Add(lenResp);

            st.Stop();

            PerfMonitor.RequestProcessTime.Add(st.ElapsedMilliseconds);
            PerfMonitor.RequestMaxProcessTime.Add(st.ElapsedMilliseconds);


            if (!res.IsSuccessStatusCode)
            {
                PerfMonitor.BadStatusNum.Increment();
            }

            return res;
        }

    }
}