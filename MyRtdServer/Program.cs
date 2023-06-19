using MyRtdServer;
using System.Collections.Generic;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        List<string> prtItems = new List<string>
        {
            "pname",
            "ticker",
            "last",
            "volume",
            "lasttick"
        };

        // Define the DDE server and topic details
        //string mt4DdeServer = "MT4";
        string mt4DdeTopic = "USDCHF";
        //var mt4Data = RealTimeDataFactory.Create("DDE", mt4DdeServer, mt4DdeTopic, prtItems);

        //string prtDdeServer = "prortdde";
        //string prtDdeTopic = "PXI";
        //var prtData = RealTimeDataFactory.Create("DDE", prtDdeServer, prtDdeTopic, prtItems);

        var webSocketUri = "ws://localhost:8080/ws";
        //var webSocketDataStreamer = new WebSocketDataStreamer(mt4Data, webSocketUri);
        //await webSocketDataStreamer.StreamDataAsync();

        // webSocketDataStreamer = new WebSocketDataStreamer(prtData, webSocketUri);
        //await webSocketDataStreamer.StreamDataAsync();

        //string rtdProgId = "barchart.rtd"; // Replace with your ProgId
        //var rtdData = RealTimeDataFactory.Create("RTD", rtdProgId, mt4DdeTopic, prtItems);

        //var webSocketDataStreamer = new WebSocketDataStreamer(rtdData, webSocketUri);
        //await webSocketDataStreamer.StreamDataAsync();

        string rtdProgId = "MyRtdServer.RtdServer";
        var rtdData = RealTimeDataFactory.Create("RTD", rtdProgId, mt4DdeTopic, prtItems);

        var webSocketDataStreamer = new WebSocketDataStreamer(rtdData, webSocketUri);
        await webSocketDataStreamer.StreamDataAsync();
    }
}
