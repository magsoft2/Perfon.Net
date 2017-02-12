using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Timers;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Perfon.Core.Notifications;
using Perfon.WebApi;

namespace TestServer
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected PerfMonitorForWebApi PerfMonitor { get; set; }

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            

            PerfMonitor = new PerfMonitorForWebApi();
            //PerfMonitor.RegisterCSVFileStorage(AppDomain.CurrentDomain.BaseDirectory + "\\" + ConfigurationManager.AppSettings["DB_Path"]);
            //PerfMonitor.RegisterInMemoryCacheStorage(60);
            PerfMonitor.RegisterLiteDbStorage(AppDomain.CurrentDomain.BaseDirectory + "\\" + ConfigurationManager.AppSettings["DB_Path"]);
            PerfMonitor.OnError += PerfMonitor_OnError; 
            var thr1 = new ThresholdMaxNotification(500);
            thr1.OnThresholdViolated += (a, b) => Console.WriteLine(b.Message);
            thr1.OnThresholdViolationRecovered += (a, b) => Console.WriteLine(b.Message);
            PerfMonitor.PerfMonitorBase.RequestNum.AddThreshold(thr1);

            //Change some default settings if needed
            PerfMonitor.Configuration.DoNotStorePerfCountersIfReqLessOrEqThan = 0; //Do not store perf values if RequestsNum = 0 during poll period
            PerfMonitor.Configuration.EnablePerfApi = true; // Enable getting perf values by API GET addresses 'api/perfcounters' and  'api/perfcounters/{name}'
            PerfMonitor.Configuration.EnablePerfUIApi = true; // Enable getting UI html page with perf counters values by API GET 'api/perfcountersui' or 'api/perfcountersuipanel'
            PerfMonitor.Start(GlobalConfiguration.Configuration, 5);
            
        }

        void PerfMonitor_OnError(object sender, Perfon.Interfaces.Common.IPerfonErrorEventArgs e)
        {
            File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\errors.log", "\n" + DateTime.Now.ToString() + " " + e.Message);
            //Console.WriteLine("PerfLibForWebApi:" + e.Message);
        }

        protected void Application_End()
        {
            PerfMonitor.Stop();
        }

    }
}