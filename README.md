# Perfon.Net
Performance monitoring .Net framework for Web Api applications with no using windows performance counters.
Built-in REST API and UI dashboard.

[See Demo page](http://perfon.1gb.ru/api/perfcountersui "Demo page")

### Perfon.Core
Core library, that monitors performance metrics of ypur application.
It does not uses windows perfomance counters, so it could used with non-privileged rights. 
It has three Storage Drivers, allowing to save counters data to CSV file, in embedded LightDB database (www.litedb.org, Analog of SQLite)
or in in-memory cache.
You could implement simple storage interface and register it for storing data in your own way.
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

One could easy implement own counter deriving base or any specific counter of the framework.
Todo: add sample for it.


### Perfon.WebApi 
A wrapper for painless use Perfon with Web Api 2 projects.
It provides Web Api for getting perf. counters through Web by /perfcounters and /perfcounters?name={perfcounterName}
Also it provides /perfcountersUI API, which return a html page with counters visualiation dashboard.
	How to use:
1. Add a reference to Perfon.WebApi.
2. Use it after Web Api application configuration code:
```c#
	GlobalConfiguration.Configure(WebApiConfig.Register); //your Web App initialization code
	
	PerfMonitor = new PerfMonitorForWebApi();
    //PerfMonitor.RegisterCSVFileStorage(AppDomain.CurrentDomain.BaseDirectory); -> use it if you want to save counters to CSV file
    //PerfMonitor.RegisterInMemoryCacheStorage(60*60*1); -> use it if you want to save counters in memory wih expiration 1 hour = 60*60 sec
    PerfMonitor.RegisterLiteDbStorage(AppDomain.CurrentDomain.BaseDirectory); //use it for storing perfomance counters data to LightDB file
    PerfMonitor.OnError += (a, b) => Console.WriteLine("PerfLibForWebApi:"+b.Message); // NOT mandatory: if you need error report from the lib    
    
	//NOT mandatory: Change some default settings if needed
	PerfMonitor.Configuration.DoNotStorePerfCountersIfReqLessOrEqThan = 0; //Do not store perf values if RequestsNum = 0 during poll period
    PerfMonitor.Configuration.EnablePerfApi = true; // Enable getting perf values by API GET addresses 'api/perfcounters' and  'api/perfcounters?name={name}'
    PerfMonitor.Configuration.EnablePerfUIApi = true; // Enable getting UI html page with perf counters values by API GET 'api/perfcountersui' or 'api/perfcountersuipanel'
            
	//Start the poll with period 2000
    PerfMonitor.Start(GlobalConfiguration.Configuration, 2000);
```

Features:
* Use PerfMonitor.UIPage and PerfMonitor.UIPanel for getting UI html as string for return it in your own controllers
* Register ThresholdMaxNotification on any Performance Counter to get notification event about threshold violations
* PerfMonitor.Storage.QueryCounterValues(name) and GetCountersList() for get counter track records.

### TestServer
Example of using Perfon.WebApi. One could run and monitor with Perfon using JMeterTests stress tests.

---

### TODO:
* Improve Demo site
* Improve dashboard
* Add description of options and conventions
* Implement PostgreSQL storage for performance counters data.
* Perfon.WebMVC - a wrapper for painless use Perfon with Asp.Net MVC projects.
* TestServer.MVC
* Add more perf counters
* Add example of implementing custom counter
* Add example of implementing own IPerfCountersStorage
* Example of using&implementing NotificationRecievers
* Add compariosn with win perf counters values (Kb/sec, CPU%, req/sec)
* Cache based on tags?
