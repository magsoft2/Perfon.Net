using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perfon.Core.PerfCounters;

namespace Perfon.Core.PerfCounterStorages
{
    /// <summary>
    /// Driver for store/restore performance counter values in simple CSV file
    /// Fie names are one per date: perfCounters_yyyy-MM-dd.csv
    /// </summary>
    public class PerfCounterCSVFileStorage:IPerfomanceCountersStorage
    {
        private string PathToDb { get; set; }

        public string ColumnDelimiter { get; set; }

        public PerfCounterCSVFileStorage(string pathToDb)
        {
            PathToDb = pathToDb+"\\";
            ColumnDelimiter = ";";
        }

        /// <summary>
        /// Awaitable.
        /// Stores perf counters into file as a line
        /// Add headers if file doesn't exists
        /// </summary>
        /// <param name="counters"></param>
        /// <returns></returns>
        public async Task StorePerfCounters(IEnumerable<IPerfCounterData> counters)
        {
            try
            {
                var sb = new StringBuilder();

                DateTime date = DateTime.Now.Date;
                var dbName = GetDbName(date);

                if (!File.Exists(PathToDb + dbName))
                {
                    //store headers
                    sb.Append("time").Append(ColumnDelimiter);
                    foreach (var item in counters)
                    {
                        sb.Append(item.Name).Append(ColumnDelimiter);
                    }
                    sb.Append(Environment.NewLine);
                }

                sb.Append(DateTime.Now).Append(ColumnDelimiter);
                foreach (var item in counters)
                {
                    sb.Append(item.FormattedValue).Append(ColumnDelimiter);
                }
                sb.Append(Environment.NewLine);

                var file = File.AppendText(PathToDb + dbName);                
                await file.WriteAsync(sb.ToString());
                file.Flush();
                file.Dispose();
            }
            catch (Exception exc)
            {
                if (OnError != null)
                {
                    OnError(new object(), new ErrorEventArgs(exc.ToString()));
                }
            }
        }

        public Task<IEnumerable<PerfCounterValue>> QueryCounterValues(string counterName, DateTime? date = null, int skip=0)
        {
            var list = new List<PerfCounterValue>();

            if (!date.HasValue)
            {
                date = DateTime.Now;
            }

            var dbName = GetDbName(date.Value);

            if (File.Exists(PathToDb + dbName))
            {
                var lines = File.ReadLines(PathToDb + dbName).GetEnumerator();
                if (lines != null)
                {
                    lines.MoveNext();
                    var headers = lines.Current.ToString().Split(new string[] { ColumnDelimiter }, StringSplitOptions.RemoveEmptyEntries);
                    var columnIdx = headers.ToList().IndexOf(counterName);
                    if (columnIdx >= 0)
                    {
                        while (lines.MoveNext())
                        {
                            var values = lines.Current.ToString().Split(new string[] { ColumnDelimiter }, StringSplitOptions.RemoveEmptyEntries);
                            if (columnIdx < values.Length)
                            {
                                var item = new PerfCounterValue(DateTime.Parse(values[0]), float.Parse(values[columnIdx]));
                                if (item.Timestamp.Date == date.Value.Date)
                                {
                                    list.Add(item);
                                }
                            }
                        }
                    }
                }
            }

            return Task.FromResult(list as IEnumerable<PerfCounterValue>);
        }

        public Task<IEnumerable<string>> GetCountersList()
        {
            var res = new List<string>();

            DateTime date = DateTime.Now.Date;
            var dbName = GetDbName(date);

            if (File.Exists(PathToDb + dbName))
            {
                var lines = File.ReadLines(PathToDb + dbName);
                if (lines != null)
                {
                    var line = lines.FirstOrDefault();
                    if (line != null)
                    {
                        var headers = line.Split(new string[]{ColumnDelimiter}, StringSplitOptions.RemoveEmptyEntries);
                        res = headers.ToList<string>();
                    }
                }
            }

            return Task.FromResult(res as IEnumerable<string>);
        }


        /// <summary>
        /// Reports about errors and exceptions occured.
        /// </summary>
        public event EventHandler<ErrorEventArgs> OnError;



        private static string GetDbName(DateTime now)
        {
            return "perfCounters_" + now.ToString("yyyy-MM-dd") + ".csv";
        }

    }
}
