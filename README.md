# Perfon.Net
Perfon.Net is a performance monitoring .Net framework for web applications without using windows performance counters.

It has built-in REST API and UI dashboard.


[See Demo site](http://perfon.1gb.ru "Demo site")

[See Demo page only, returned from Perfon.Net Api](http://perfon.1gb.ru/api/perfcountersui "See Demo page only, returned from Perfon.Net Api")

Install from NuGet: `Install-Package Perfon.WebApi` [(link)](https://www.nuget.org/packages/Perfon.WebApi "Nuget link") for using in Asp.Net WebApi projects.

Install from NuGet: `Install-Package Perfon.Mvc` [(link)](https://www.nuget.org/packages/Perfon.Mvc "Nuget link") for using in Asp.Net Mvc 5 projects.


It is designed mainly for use in WebApi and MVC projects, placed on shared hosting plans, when you have no access to IIS or OS and work without access to IIS or OS, or have non-privileged rights.


It has three built-in storage drivers, allowing to save counters data to CSV file, in embedded LiteDB database (www.litedb.org, SQLite analog) or to keep it in memory cache.
You could develop own storage driver by implementing simple interface.
Custom counters could be impelemented easily deriving from Perfon.Net base performance counters.
Threshold notifications could be set on counters.

A list of additional storages: 
* [Perfon.Storage.PostgreSql](https://github.com/magsoft2/Perfon.Storage.PostgreSql "Perfon.Storage.PostgreSql") 
* [Perfon.Storage.MySql](https://github.com/magsoft2/Perfon.Storage.MySql "Perfon.Storage.MySql") 


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


Perfon.Net provides Web Api for getting performance counters and built-in dashboard:

* get counters list: api/perfcounters 
* get values for selected counter: api/perfcounters?name={name} [&date={date}&skip={number}]
* html page dashboard with visualization of counters: api/perfcountersui	
* html div: api/perfcountersuipanel


It could be tested on the Demo site: [http://perfon.1gb.ru/api/perfcountersui](http://perfon.1gb.ru/api/perfcountersui "http://perfon.1gb.ru/api/perfcountersui")


Counter values have been verified with counters of JMeter and Windows PerfMon for both WebApi and MVC projects under JMeter stress tests.


--
# How to use:

### Perfon.WebApi 
A wrapper for painless use Perfon with Web Api 2 projects.

How to use:

1. Add a reference to Perfon.WebApi.

2. Use it after Web Api application configuration code:
```c#
	GlobalConfiguration.Configure(WebApiConfig.Register); //your Web App initialization code
	
	PerfMonitor = new PerfMonitorForWebApi();
    //PerfMonitor.RegisterCSVFileStorage(AppDomain.CurrentDomain.BaseDirectory); -> use it if you want to save counters to CSV file
    //PerfMonitor.RegisterInMemoryCacheStorage(60*60*1); -> use it if you want to save counters in memory wih expiration 1 hour = 60*60 sec
    PerfMonitor.RegisterLiteDbStorage(AppDomain.CurrentDomain.BaseDirectory+"\\path_to_db"); //use it for storing perfomance counters data to LiteDB file
    //PerfMonitor.RegisterStorages( new Perfon.Storage.PostgreSql.PerfCounterPostgreSqlStorage(@"host=xxx;port=xxx;Database=db_name;username=user_name;password=pswd")) // For use PostgreSql as Storage
	PerfMonitor.OnError += (a, errArg) => Console.WriteLine("PerfLibForWebApi:"+errArg.Message); // NOT mandatory: if you need error report from the lib    
    
	//NOT mandatory: Change some default settings if needed
	PerfMonitor.Configuration.DoNotStorePerfCountersIfReqLessOrEqThan = 0; //Do not store perf values if RequestsNum = 0 during poll period
    PerfMonitor.Configuration.EnablePerfApi = true; // Enable getting perf values by API GET addresses 'api/perfcounters' and  'api/perfcounters?name={name}'
    PerfMonitor.Configuration.EnablePerfUIApi = true; // Enable getting UI html page with perf counters values by API GET 'api/perfcountersui' or 'api/perfcountersuipanel'
            
	//Start the poll with period 10sec
    PerfMonitor.Start(GlobalConfiguration.Configuration, 10);
```


LiteDb file size for daily data with poll period 10 sec is ~4Mb, for 5 sec period ~ 7Mb


Note, that you need to enable attribute routing via config.MapHttpAttributeRoutes() in your Web Api application for getting perf values through Rest Api and use DashboardUI API;

Features:
* Use PerfMonitor.UIPage and PerfMonitor.UIPanel for getting UI html as string for return it in your own controllers
* Register ThresholdMaxNotification on any Performance Counter to get notification event about threshold violations
* PerfMonitor.Storage.QueryCounterValues(name) and GetCountersList() for get counter track records.


### Perfon.Mvc 
A wrapper for painless use Perfon with Web Mvc 5 projects.

1. Add a reference to Perfon.Mvc.

2. Use it after Web Mvc application configuration code:
```c#
	Perfon.Mvc.PerfMonitorForMvc PerfMonitor = new Perfon.Mvc.PerfMonitorForMvc();

    PerfMonitor.RegisterLiteDbStorage(AppDomain.CurrentDomain.BaseDirectory+"\\path_to_db");
    PerfMonitor.OnError += (a, b) =>{ };

    //Change some default settings if needed
    PerfMonitor.Configuration.DoNotStorePerfCountersIfReqLessOrEqThan = 0;
    PerfMonitor.Configuration.EnablePerfApi = true; 
    PerfMonitor.Configuration.EnablePerfUIApi = true;
    PerfMonitor.Start(this, RouteTable.Routes, 10);
```

--
			
### Perfon.Core
Core library, that monitors performance metrics of your application.
It does not uses windows perfomance counters, so it could used with non-privileged rights. 

--


### TestServer, TestMvcApp
Example of using Perfon.WebApi and Perfon.Mvc. One could run and monitor with Perfon using JMeterTests stress tests.

--

License: MIT

--

### TODO:
* Add description of options and conventions
* Add more perf counters (GC time )
* Add example of implementing custom counter
* Add example of implementing own IPerfCountersStorage
* Example of using&implementing NotificationRecievers
* Add comparison with win perf counters values (Kb/sec, CPU%, req/sec)
* Add support of Asp.Net Core? Compare with MS Application Insights (Cons: It needs registration on Azure)
* Cache based on tags?
