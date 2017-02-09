using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Perfon.Core;

namespace Perfon.Mvc.Filters
{
    public class ResponseLengthCalculatingStream : MemoryStream
    {
        private PerfMonitor PerfMonitor {get;set;}


        private readonly Stream responseStream;
        private long responseSize = 0;

        public ResponseLengthCalculatingStream(Stream responseStream, PerfMonitor perfMonitor)
        {
            this.responseStream = responseStream;
            PerfMonitor = perfMonitor;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            responseSize += count;
            responseStream.Write(buffer, offset, count);
        }

        public override void Flush()
        {
            PerfMonitor.BytesTrasmittedResp.Add(responseSize);
            responseSize = 0;
            base.Flush();
        }

        public override System.Threading.Tasks.Task WriteAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
        {
            responseSize += count;
            return base.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override System.Threading.Tasks.Task FlushAsync(System.Threading.CancellationToken cancellationToken)
        {
            PerfMonitor.BytesTrasmittedResp.Add(responseSize);
            return base.FlushAsync(cancellationToken);
        }

    }
}