using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class StopwatchUtil
    {
        private static StopwatchUtil _instance = null;

        private StopwatchUtil() { }

        public static StopwatchUtil getInstance()
        {
            if (_instance == null)
                _instance = new StopwatchUtil();

            return _instance;
        }

        public string GetTimestamp(Stopwatch _SW)
        {
            long microseconds = _SW.ElapsedTicks / (Stopwatch.Frequency / (1000L * 1000L));

            return (microseconds < 1000) ? $"{microseconds} us" : $"{_SW.ElapsedMilliseconds} ms";
        }
    }
}
