using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Perfon.Core;
using Perfon.Core.Notifications;

namespace TestMvcApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RouteConfig.RegisterRoutes(RouteTable.Routes);

            Perfon.Mvc.PerfMonitorForMvc PerfMonitor = new Perfon.Mvc.PerfMonitorForMvc();

            PerfMonitor.RegisterLiteDbStorage(AppDomain.CurrentDomain.BaseDirectory);
            PerfMonitor.OnError += (a, b) =>
            {
                File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\errors.log", "\n" + DateTime.Now.ToString() + " " + b.Message);
                Console.WriteLine("PerfLibForMvc:" + b.Message);
            };
            var thr1 = new ThresholdMaxNotification(500);
            thr1.OnThresholdViolated += (a, b) => Console.WriteLine(b.Message);
            thr1.OnThresholdViolationRecovered += (a, b) => Console.WriteLine(b.Message);
            PerfMonitor.PerfMonitorBase.RequestNum.AddThreshold(thr1);

            //Change some default settings if needed
            PerfMonitor.Configuration.DoNotStorePerfCountersIfReqLessOrEqThan =-1;
            PerfMonitor.Configuration.EnablePerfApi = true; 
            PerfMonitor.Configuration.EnablePerfUIApi = true;
            PerfMonitor.Start(this, RouteTable.Routes, 5);
            
        }
    }
}
