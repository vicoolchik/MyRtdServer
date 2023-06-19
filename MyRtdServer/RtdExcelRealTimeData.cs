using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRtdServer
{
    public class RtdExcelRealTimeData : ExcelRealTimeDataInterface
    {
        private readonly RtdClient _rtdClient;
        private List<string> _items;

        public RtdExcelRealTimeData(string rtdProgId, string rtdTopic, List<string> items)
        {
            _rtdClient = new RtdClient(rtdProgId);
            _items = items;
        }

        public async Task<List<string>> ReadDataAsync()
        {
            List<string> data = new List<string>();

            try
            {
                foreach (var item in _items)
                {
                    var value = _rtdClient.GetValue(item);
                    data.Add(value.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return data;
        }
    }
}
