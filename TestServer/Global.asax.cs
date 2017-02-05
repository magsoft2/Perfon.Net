using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Timers;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using PerfMonLib.Core.Notifications;
using PerfMonLib.WebApi;

namespace TestServer
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected PerfMonitorForWebApi PerfMonitor { get; set; }

        protected void Application_Start()
        {

            GlobalConfiguration.Configure(WebApiConfig.Register);
            

            PerfMonitor = new PerfMonitorForWebApi();
            PerfMonitor.RegisterCSVFileStorage(AppDomain.CurrentDomain.BaseDirectory + "\\perf.csv");
            PerfMonitor.RegisterLiteDbStorage(AppDomain.CurrentDomain.BaseDirectory);
            PerfMonitor.OnError += (a, b) => 
            {
                Console.WriteLine("PerfLibForWebApi:"+b.Message);
            };
            var thr1 = new ThresholdMaxNotification(500);
            thr1.OnThresholdViolated += (a, b) => Console.WriteLine(b.Message);
            thr1.OnThresholdViolationRecovered += (a, b) => Console.WriteLine(b.Message);
            PerfMonitor.PerfMonitorBase.RequestNum.AddThreshold(thr1);

            PerfMonitor.Start(GlobalConfiguration.Configuration, 2000, 0, true);

        }

        protected void Application_End()
        {
            PerfMonitor.Stop();
        }

    }
}