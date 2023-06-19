using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyRtdServer
{
    public interface ExcelRealTimeDataInterface
    {
        Task<List<string>> ReadDataAsync();
    }

    public class RealTimeDataFactory
    {
        public static ExcelRealTimeDataInterface Create(string protocol, string server, string topic, List<string> items)
        {
            if (protocol == "DDE")
            {
                //return new DdeExcelRealTimeData(server, topic, items);
            }
            else if (protocol == "RTD")
            {
                // Modify this line to provide parameters to the RtdExcelRealTimeData constructor
                return new RtdExcelRealTimeData(server, topic, items); // server is ProgId here
            }
            else
            {
                throw new ArgumentException("Invalid protocol");
            }

            // Add a return statement here or at the end of the method
            throw new ArgumentException("Invalid protocol");
        }
    }
}
