using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Perfon.Core;
using Perfon.Mvc;

namespace Perfon.Mvc.Controllers
{
    public class PerfCountersUIController : Controller
    {
        private PerfMonitorForMvc PerfMonitorLib
        {
            get
            {
                //How to deal with DI here??
                return (HttpContext.Application[EnumKeyNames.PerfMonitorLib.ToString()] as PerfMonitorForMvc);
            }
        }

        //
        // GET: /PerfCountersUI/
        public ActionResult Page()
        {
            return Content(PerfMonitorLib.UIPage, "text/html");

            HtmlString s = new HtmlString(PerfMonitorLib.UIPanel);
            ViewData["dashboard"] = s;
            return View();            
        }

        //
        // GET: /PerfCountersUI/
        public ActionResult PageStandalone()
        {
            return Content(PerfMonitorLib.UIPage, "text/html");
        }
	}
}