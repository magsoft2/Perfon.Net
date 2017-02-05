using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;

namespace Perfon.WebApi
{
    public class AuthPerfMonitorLibActionsAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var perfLib = actionContext.ControllerContext.Configuration.Properties["PerfMonitorLib"] as PerfMonitorForWebApi;

            if (perfLib == null)
            {
                throw new Exception("Cannot run PerfMonLib without PerfMonitorLib property set");
            }

            if (perfLib.IsPerfCountersControllerEnabled)
            {
                base.OnActionExecuting(actionContext);
            }
            else
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
            }

        }

        public override Task OnActionExecutingAsync(System.Web.Http.Controllers.HttpActionContext actionContext, System.Threading.CancellationToken cancellationToken)
        {
            var perfLib = actionContext.ControllerContext.Configuration.Properties["PerfMonitorLib"] as PerfMonitorForWebApi;

            if (perfLib == null)
            {
                throw new Exception("Cannot run PerfMonLib without PerfMonitorLib property set");
            }

            if (perfLib.IsPerfCountersControllerEnabled)
            {
                return base.OnActionExecutingAsync(actionContext, cancellationToken);
            }
            else
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
            }
        }
    }
}
