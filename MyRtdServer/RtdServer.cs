using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;

namespace MyRtdServer
{
    [Guid("C6621F2A-24A6-4E41-A291-4482E010AE89")]
    [ProgId("MyRtdServer.RtdServer")]
    [ComVisible(true)]
    public class RtdServer : IRtdServer
    {
        private IRTDUpdateEvent rtdUpdateEvent;
        private Dictionary<int, string> topics = new Dictionary<int, string>();

        public int ServerStart(IRTDUpdateEvent callbackObject)
        {
            rtdUpdateEvent = callbackObject;
            return 1;
        }

        public object ConnectData(int topicId, ref Array Strings, ref bool GetNewValues)
        {
            var topic = Strings.GetValue(0).ToString();

            if (!topics.ContainsKey(topicId))
            {
                topics.Add(topicId, topic);
            }

            GetNewValues = true;
            return "Waiting for update...";
        }

        public Array RefreshData(ref int topicCount)
        {
            var data = new object[2, topics.Count];
            var i = 0;

            foreach (var key in topics.Keys)
            {
                data[0, i] = key;
                data[1, i] = DateTime.Now.ToLongTimeString();
                i++;
            }

            topicCount = topics.Count;
            return data;
        }

        public void DisconnectData(int topicId)
        {
            if (topics.ContainsKey(topicId))
            {
                topics.Remove(topicId);
            }
        }

        public int Heartbeat()
        {
            return 1;
        }

        public void ServerTerminate()
        {
            topics.Clear();
        }

        [return: MarshalAs(UnmanagedType.Struct)]
        public object ConnectData([In] int topicId, [In, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] ref object[] parameters, [In, Out] ref bool newValue)
        {
            throw new NotImplementedException();
        }

        object[,] IRtdServer.RefreshData(ref int topicCount)
        {
            throw new NotImplementedException();
        }
    }
}
