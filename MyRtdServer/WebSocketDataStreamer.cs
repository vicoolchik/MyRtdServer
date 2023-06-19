using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace MyRtdServer
{
    public class WebSocketDataStreamer
    {
        private ExcelRealTimeDataInterface _realTimeData;
        private ClientWebSocket _webSocket;
        private BufferBlock<List<string>> _bufferBlock;
        private ActionBlock<List<string>> _sendBlock;
        private string _webSocketUri;

        public WebSocketDataStreamer(ExcelRealTimeDataInterface realTimeData, string webSocketUri)
        {
            _realTimeData = realTimeData;
            _webSocketUri = webSocketUri;
            _webSocket = new ClientWebSocket();
        }

        public async Task StreamDataAsync()
        {
            try
            {
                await _webSocket.ConnectAsync(new Uri(_webSocketUri), CancellationToken.None);

                _bufferBlock = new BufferBlock<List<string>>();
                _sendBlock = new ActionBlock<List<string>>(async data =>
                {
                    foreach (var row in data)
                    {
                        string json = JsonConvert.SerializeObject(row);
                        var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(json));

                        if (_webSocket.State == WebSocketState.Open)
                        {
                            await _webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                        else
                        {
                            throw new Exception("Websocket connection is not open.");
                        }
                    }
                });

                _bufferBlock.LinkTo(_sendBlock, new DataflowLinkOptions { PropagateCompletion = true });

                while (_webSocket.State == WebSocketState.Open)
                {
                    var data = await _realTimeData.ReadDataAsync();
                    await _bufferBlock.SendAsync(data);
                }
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine($"WebSocketException occurred: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occurred: {ex.Message}");
            }
            finally
            {
                _bufferBlock.Complete();
                await _sendBlock.Completion;

                if (_webSocket.State != WebSocketState.Closed)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }
            }
        }
    }
}
