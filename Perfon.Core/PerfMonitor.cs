using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Perfon.Core.Common;
using Perfon.Core.PerfCounters;
using Perfon.Core.PerfCounterStorages;
using Perfon.Core.PerfCounterStorages.CSVFileStorage;
using Perfon.Core.PerfCounterStorages.InMemoryCacheStorage;
using Perfon.Interfaces.Common;
using Perfon.Interfaces.PerfCounters;
using Perfon.Interfaces.PerfCounterStorage;

namespace Perfon.Core
{
    /// <summary>
    /// Perf counters engine.
    /// Manages a list of counters.
    /// Periodically collects perf counter values and save it to perf storage using IPerfomanceCountersStorage
    /// </summary>
    public class PerfMonitor
    {
        /// <summary>
        /// Reports about errors and exceptions occured.
        /// </summary>
        public event EventHandler<IPerfonErrorEventArgs> OnError;

        /// <summary>
        /// Settings for Perfon engine
        /// </summary>
        public PerfonConfiguration Configuration { get; set; }

        
        public PerfMonitor()
        {
            Configuration = new PerfonConfiguration();

            GenerateDefaultPerfCounters();

            foreach (var item in countersList.Values)
            {
                countersListGeneric.Add(item as IPerformanceCounter);
            }
        }

        /// <summary>
        /// Shortcuts for main perf counters which should be process\calculated by external classes.
        /// </summary>
        public IPerformanceCounter ExceptionsNum
        {
            get { return exceptionsNum; }
            set { exceptionsNum = value; }
        }
        public IPerformanceCounter BadStatusNum
        {
            get { return badStatusNum; }
            set { badStatusNum = value; }
        }
        public IPerformanceCounter RequestNum
        {
            get { return requestNum; }
            set { requestNum = value; }
        }
        public IPerformanceCounter RequestProcessTime
        {
            get { return requestProcessTime; }
            set { requestProcessTime = value; }
        }
        public IPerformanceCounter RequestMaxProcessTime
        {
            get { return requestMaxProcessTime; }
            set { requestMaxProcessTime = value; }
        }
        public IPerformanceCounter BytesTrasmittedResp
        {
            get { return bytesTrasmittedResp; }
            set { bytesTrasmittedResp = value; }
        }
        public IPerformanceCounter BytesTrasmittedReq
        {
            get { return bytesTrasmittedReq; }
            set { bytesTrasmittedReq = value; }
        }

        

        /// <summary>
        /// Perf counters storage for fast access.
        /// PerfMonLib has only a few counters.
        /// Used internally
        /// Note: If number of counters will grow > 10, change ListDictionary to something faster
        /// </summary>
        private ListDictionary countersList = new ListDictionary();
        /// <summary>
        /// Used by storage drivers.
        /// </summary>
        public List<IPerformanceCounter> countersListGeneric = new List<IPerformanceCounter>();

        /// <summary>
        /// Register Perf Counter implemented by user. Custom Perf Counter.
        /// Use GetPerfCounter(name) for getting this counter for processing
        /// User should himself take care of calcualting this counter  
        /// </summary>
        /// <param name="counter"></param>
        public void AddUserCustomPerfCounter(IPerformanceCounter counter)
        {
            countersList.Add(counter.Name, counter);
            countersListGeneric.Add(counter);
        }
        /// <summary>
        /// Retrieve perf counter and process its value.
        /// Mainly used for User Implemented Custom Perf Counters when doing counter calculation.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IPerformanceCounter GetPerfCounter(string name)
        {
            if (countersList.Contains(name))
            {
                return countersList[name] as IPerformanceCounter;
            }
            else
            {
                return null;
            }
        }



        private void GenerateDefaultPerfCounters()
        {
            /// Should be calculated externally (out of this class). Calculations are implemented inside WebApi/MVC MessageHandlers/Filters
            requestNum = new PerformanceSumCounter("RequestsCount, num/sec");
            countersList.Add(RequestNum.Name, requestNum);

            requestProcessTime = new PerformanceAverageCounter("RequestProcessTime, ms");
            countersList.Add(RequestProcessTime.Name, RequestProcessTime);

            requestMaxProcessTime = new PerformanceMaxCounter("RequestMaxProcessTime, ms");
            countersList.Add(requestMaxProcessTime.Name, requestMaxProcessTime);

            bytesTrasmittedReq = new PerformanceSumCounter("KBytes req. transmitted, kb/sec", 1.0f / 1024, "n3");
            countersList.Add(bytesTrasmittedReq.Name, bytesTrasmittedReq);

            bytesTrasmittedResp = new PerformanceSumCounter("KBytes resp. transmitted, kb/sec", 1.0f / 1024, "n3");
            countersList.Add(bytesTrasmittedResp.Name, bytesTrasmittedResp);

            exceptionsNum = new PerformanceSumCounter("ExceptionsCount, num/sec");
            countersList.Add(ExceptionsNum.Name, ExceptionsNum);

            badStatusNum = new PerformanceSumCounter("BadStatusResponsesCount, num/sec");
            countersList.Add(BadStatusNum.Name, BadStatusNum);
            
            

            /// Calculated internally
            memoryUsed = new PerformanceSumAbsoluteCounter("Memory, mb", 1.0f/1024/1024);
            countersList.Add(memoryUsed.Name, memoryUsed);

            cpuUsage = new PerformanceDifferenceCounter("CPU, %", 1f, "n2");
            countersList.Add(cpuUsage.Name, cpuUsage);

            gcGen0 = new PerformanceDifferenceCounter("GC 0-gen collections, num");
            countersList.Add(gcGen0.Name, gcGen0);

            gcGen1 = new PerformanceDifferenceCounter("GC 1-gen collections, num");
            countersList.Add(gcGen1.Name, gcGen1);

            gcGen2 = new PerformanceDifferenceCounter("GC 2-gen collections, num");
            countersList.Add(gcGen2.Name, gcGen2);
        }

        /// <summary>
        /// Register perf counter storages
        /// </summary>
        /// <param name="storage"></param>
        public IPerfomanceCountersStorage RegisterStorages(params IPerfomanceCountersStorage[] storages)
        {
            storagesList.AddRange(storages);

            foreach (var st in storages)
            {
                st.OnError += (a, b) => {
                    if (OnError != null)
                    {
                        OnError(a,b);
                    }
                };
            }

            return storages[0];
        }
        /// <summary>
        /// Easy register some default laready implemented storages, if needed
        /// </summary>
        /// <param name="dbPath"></param>
        /// <returns></returns>
        public IPerfomanceCountersStorage RegisterCSVFileStorage(string dbPath)
        {
            var storage = new PerfCounterCSVFileStorage(dbPath);
            return RegisterStorages(storage);
        }
        public IPerfomanceCountersStorage RegisterLiteDbStorage(string dbPath)
        {
            var storage = new PerfCounterLiteDbStorage(dbPath);
            return RegisterStorages(storage);
        }
        public IPerfomanceCountersStorage RegisterInMemoryCacheStorage(long expirationInSeconds)
        {
            var storage = new PerfCounterInMemoryCacheStorage(expirationInSeconds);
            return RegisterStorages(storage);
        }
        /// <summary>
        /// Start polling and saving perf counters. Period is in ms
        /// </summary>
        /// <param name="pollPeriod_sec">Poll period, sec</param>
        public void Start(int pollPeriod_sec)
        {
            AppDomain.MonitoringIsEnabled = true;

            PollPeriod_ms = pollPeriod_sec*1000;
            PollPeriod_RevSec = 1.0f / (PollPeriod_ms/1000.0f);

            //Tune perf counters scale coefficients
            cpuUsage.PostProcessMultiplyCoeff = 100 * 1.0f / 1000 * PollPeriod_RevSec;
            //Notify all counters about poll period value
            foreach (var item in countersList.Values)
            {
                (item as IPerformanceCounter).ReversedPeriodValue = PollPeriod_RevSec;
            }

            // Add all counter names to db
            var listTemp = countersListGeneric.Select(a => a.GetPerfCounterData()).ToList();
            foreach (var item in storagesList)
            {
                item.StorePerfCounters(listTemp);
            }

            //Restart timer if needed
            if (timer != null)
            {
                timer.Dispose();
            }

            timer = new Timer(TimerTicker, null, PollPeriod_ms, PollPeriod_ms);

        }
        /// <summary>
        /// Stops perf counters collecting
        /// </summary>
        public void Stop()
        {
            if(timer!= null)
            {
                timer.Dispose();
                timer = null;
            }
        }


        /// <summary>
        /// Return UI html page for monitor perf counters
        /// </summary>
        public Lazy<string> UIPage = new Lazy<string>(() =>
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Perfon.Core.UI.PerfCountersUI.html";

            string result = "";

            try
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        result = reader.ReadToEnd();

                        using (Stream stream2 = assembly.GetManifestResourceStream("Perfon.Core.UI.PerfCountersUIPanel.html"))
                        {
                            using (StreamReader reader2 = new StreamReader(stream2))
                            {
                                result = result.Replace("PLACEHOLDER_FOR_PANEL", reader2.ReadToEnd());
                            }
                        }

                    }
                }
            }
            catch (Exception exc)
            {
                result = exc.ToString();
            }

            return result;
        });
        /// <summary>
        /// Return UI html panel as div for monitor perf counters
        /// </summary>
        private Lazy<string> uiPanel = new Lazy<string>(() =>
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Perfon.Core.UI.PerfCountersUIPanel.html";

            string result = "";

            try
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception exc)
            {
                result = exc.ToString();
            }

            return result;
        });
        public string UIPanel { get { return uiPanel.Value; } }



        private Timer timer { get; set; }

        private int PollPeriod_ms { get; set; }
        private float PollPeriod_RevSec { get; set; }


        /// <summary>
        /// Should be process by external classes. Implemented in WebApi and MVC wrapper libs.
        /// </summary>
        private IPerformanceCounter exceptionsNum;
        private IPerformanceCounter badStatusNum;
        private IPerformanceCounter requestNum;
        private IPerformanceCounter requestProcessTime;
        private IPerformanceCounter requestMaxProcessTime;
        private IPerformanceCounter bytesTrasmittedReq;
        private IPerformanceCounter bytesTrasmittedResp;

        //Tracked internally
        private IPerformanceCounter memoryUsed;
        private IPerformanceCounter cpuUsage;
        private IPerformanceCounter gcGen0;
        private IPerformanceCounter gcGen1;
        private IPerformanceCounter gcGen2;
        

        /// <summary>
        /// Perf counters storagess
        /// </summary>
        private List<IPerfomanceCountersStorage> storagesList = new List<IPerfomanceCountersStorage>();


        private void TimerTicker(object state)
        {
            try
            {
                CollectInternalCounters();

                //Check and notify about threshold violations occured
                foreach (var item in countersListGeneric)
                {
                    foreach (var thr in item.Thresholds)
                    {
                        thr.TestThresholdOk(item);
                    }
                }

                double count = requestNum.GetValue();
                if (count > Configuration.DoNotStorePerfCountersIfReqLessOrEqThan)
                {
                    var listTemp = countersListGeneric.Select(a =>a.GetPerfCounterData()).ToList();
                    
                    ///Note that it is not ideal solution!
                    ///Ideally, storing and resetting should be locked in one section for exact precise counter values, 
                    ///   and increment should use the same lock object!
                    ///But for our purposes precision is enough, for polling periods ~1sec 
                    foreach (var item in countersListGeneric)
                    {
                        item.Reset();
                    }
                    Stopwatch sw = Stopwatch.StartNew();
                    foreach (var item in storagesList)
                    {
                        item.StorePerfCounters(listTemp);
                    }
                    sw.Stop();
                    if (sw.Elapsed.TotalMilliseconds > 500)
                    {
                       OnError(null, new PerfonErrorEventArgs("Storing counters:" + sw.Elapsed.TotalMilliseconds.ToString("n0") + " ms"));
                    }
                }                

            }
            catch (Exception exc)
            {
                if (OnError != null)
                {
                    OnError(new object(), new PerfonErrorEventArgs(exc.ToString()));
                }
            }
        }

        private void CollectInternalCounters()
        {
            memoryUsed.Add(GC.GetTotalMemory(false));
            gcGen0.Add(GC.CollectionCount(0));
            gcGen1.Add(GC.CollectionCount(1));
            gcGen2.Add(GC.CollectionCount(2));
            if (AppDomain.MonitoringIsEnabled)
            {
                cpuUsage.Add((long)AppDomain.CurrentDomain.MonitoringTotalProcessorTime.TotalMilliseconds/Environment.ProcessorCount);
                //cpuUsage.Add((long)AppDomain.CurrentDomain.MonitoringTotalAllocatedMemorySize);
            }
        }
    }


}