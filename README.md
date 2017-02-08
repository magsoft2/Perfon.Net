# Perfon.Net
Perfon.Net is a performance monitoring .Net framework for web applications without using windows performance counters.

It is designed mainly for use in WebApi and MVC projects, placed on shared hosting plans, when you have no access to IIS or OS and work without access to IIS or OS, or have non-privileged rights.
It has built-in REST API and UI dashboard

[See Demo site](http://perfon.1gb.ru "Demo site")

[See Demo page only, returned from Perfon.Net Api](http://perfon.1gb.ru/api/perfcountersui "See Demo page only, returned from Perfon.Net Api")


### Perfon.Core
Core library, that monitors performance metrics of your application.
It does not uses windows perfomance counters, so it could used with non-privileged rights. 
It has three built-in storage drivers, allowing to save counters data to CSV file, in embedded LightDB database (www.litedb.org, SQLite analog) or to keep it in memory cache.
You could develop own storage driver implementing simple interface.
Custom counters could be impelemented easily deriving from Peron.Net base performance counters.
Threshold notifications could be set on counters.
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
It provides Web Api for getting performance counters and built-in dashboard:
get counters list: api/perfcounters 
get values for selected counter: api/perfcounters?name={name}	
html page dashboard iwth visualization of counters: api/perfcountersui	
html div: api/perfcountersuipanel
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
