# Perfon.Net
Performance monitoring .Net framework for Web Api applications with no using windows performance counters

Demo page - <todo>

PerfMonLib.Core - core library, that monitors performance and saves it into CSV file or in embedded LightDB database.
It do not uses windows perfomance counters, so it could used with non-privileged rights. 
For example, it could be used on the shared hosting plans, when you have no access to IIS or OS. 
A list of implemented performance counters:


PerfMonLib.WebApi - a wrapper for painless use PerfMonLib with Web Api 2 projects.
It provides Web Api for getting perf. counters through Web by /perfcounters and /perfcounters?name=perfcounterName
Also it provides /perfcountersUI API, which return a html page with counters visualiation dashboard.

TestServer - example of using PerfMonLib.WebApi. One could run and monitor with PerfMonLib using JMeterTests stress tests.

TODO:
PerfMonLib.WebMVC - a wrapper for painless usa PerfMonLib with Asp.Net MVC projects.
TestServer.MVC
Improve dashboard
