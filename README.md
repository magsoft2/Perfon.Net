# Perfon.Net
Performance monitoring .Net framework for Web Api applications with no using windows performance counters

Demo page - todo

### Perfon.Core
Core library, that monitors performance and saves it into CSV file or in embedded LightDB database (Analog of SQLLite).
It do not uses windows perfomance counters, so it could used with non-privileged rights. 
For example, it could be used on the shared hosting plans, when you have no access to IIS or OS. 
A list of implemented performance counters:
* Number of requests per second
* Number of exceptions per second
* Number of bad status codes per second
* Average Request processing time during poll period
* Max Request processing time during poll period
* KBytes/sec Send
* KBytes/sec Recieved
* Number of GC generation 0,1,2 collections during the poll period
* CPU %
* Total .Net memory



### Perfon.WebApi 
A wrapper for painless use Perfon with Web Api 2 projects.
It provides Web Api for getting perf. counters through Web by /perfcounters and /perfcounters?name=perfcounterName
Also it provides /perfcountersUI API, which return a html page with counters visualiation dashboard.
	How to use:
1. Add a reference to Perfon.WebApi.
2. Use it after Web Api application config:
```c#
	GlobalConfiguration.Configure(WebApiConfig.Register); //your Web App initialization code
	
	PerfMonitor = new PerfMonitorForWebApi();
    //PerfMonitor.RegisterCSVFileStorage(AppDomain.CurrentDomain.BaseDirectory + "\\perf.csv"); -> use it if you want to save counters to CSV file
    PerfMonitor.RegisterLiteDbStorage(AppDomain.CurrentDomain.BaseDirectory); //use it for storing perfomance counters data to LightDB file
    PerfMonitor.OnError += (a, b) => Console.WriteLine("PerfLibForWebApi:"+b.Message); // if you need error report from the lib    
    // Now pass your Web App HttpConfiguration and some parameters (poll period etc) to the lib
    PerfMonitor.Start(GlobalConfiguration.Configuration, 2000, 0, true);
```

### TestServer
Example of using Perfon.WebApi. One could run and monitor with Perfon using JMeterTests stress tests.

---

### TODO:
* Demo site
* Perfon.WebMVC - a wrapper for painless use Perfon with Asp.Net MVC projects.
* TestServer.MVC
* Improve dashboard
* Add more perf counters
