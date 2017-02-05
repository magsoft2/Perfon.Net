using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perfon.Core.PerfCounters;

namespace Perfon.Core.PerfCounterStorages
{
    public class PerfCounterCSVFileStorage:IPerfomanceCountersStorage
    {
        private string PathToFile { get; set; }

        public string ColumnDelimiter { get; set; }

        /// <summary>
        /// Reports about errors and exceptions occured.
        /// </summary>
        public event EventHandler<ErrorEventArgs> OnError;


        public PerfCounterCSVFileStorage(string pathToFile)
        {
            PathToFile = pathToFile;
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

                if (!File.Exists(PathToFile))
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

                var file = File.AppendText(PathToFile);                
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


        public Task<IEnumerable<PerfCounterValue>> QueryCounterValues(string counterName, DateTime? date = null)
        {
            var list = new List<PerfCounterValue>();

            if (!date.HasValue)
            {
                date = DateTime.Now;
            }

            if (File.Exists(PathToFile))
            {
                var lines = File.ReadLines(PathToFile).GetEnumerator();
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
                                var item = new PerfCounterValue(DateTime.Parse(values[0]), double.Parse(values[columnIdx]));
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

            if (File.Exists(PathToFile))
            {
                var lines = File.ReadLines(PathToFile);
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
    }
}
