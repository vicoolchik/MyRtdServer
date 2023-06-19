using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public interface IRtdClient
{
    object GetValue(params object[] args);
}

public class RtdClient : IRtdClient
{
    readonly string _rtdProgId;
    static IRtdServer _rtdServer;

    public RtdClient(string rtdProgId)
    {
        _rtdProgId = rtdProgId;
    }

    public object GetValue(params object[] args)
    {
        const int topicCount = 1;

        var rnd = new Random();
        var topicId = rnd.Next(int.MaxValue);
        var rtdServer = GetRtdServer();

        rtdServer.ConnectData(topicId, args, true);

        object val = null;
        while (val == null)
        {
            var alive = rtdServer.Heartbeat();
            if (alive != 1)
                GetRtdServer();
            else
            {
                var refresh = rtdServer.RefreshData(topicCount);
                if (refresh.Length <= 0) continue;

                if (refresh[0, 0].ToString() == topicId.ToString())
                {
                    val = refresh[1, 0];
                }
            }
        }

        rtdServer.DisconnectData(topicId);

        return val;
    }

    IRtdServer GetRtdServer()
    {
        if (_rtdServer == null)
        {
            try
            {
                Type rtd = Type.GetTypeFromProgID(_rtdProgId);
                Console.WriteLine($"Type obtained from ProgID: {rtd}");
                _rtdServer = (IRtdServer)Activator.CreateInstance(rtd);

                UpdateEvent updateEvent = new UpdateEvent();
                _rtdServer.ServerStart(updateEvent);
            }
            catch (COMException ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                throw;
            }
        }
        return _rtdServer;
    }
}

public class UpdateEvent : IRTDUpdateEvent
{
    public long Count { get; set; }
    public int HeartbeatInterval { get; set; }

    public UpdateEvent()
    {
        // Do not call the RTD Heartbeat()
        // method.
        HeartbeatInterval = -1;
    }

    public void Disconnect()
    {
        // Do nothing.
    }

    public void UpdateNotify()
    {
        Count++;
    }
}

[ComImport,
TypeLibType((short)0x1040),
Guid("EC0E6191-DB51-11D3-8F3E-00C04F3651B8")]
public interface IRtdServer
{
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(10)]
    int ServerStart([In, MarshalAs(UnmanagedType.Interface)] IRTDUpdateEvent callback);

    [return: MarshalAs(UnmanagedType.Struct)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(11)]
    object ConnectData([In] int topicId, [In, MarshalAs(UnmanagedType.SafeArray,
                                                SafeArraySubType = VarEnum.VT_VARIANT)] ref object[] parameters, [In, Out] ref bool newValue);

    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(12)]
    object[,] RefreshData([In, Out] ref int topicCount);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(13)]
    void DisconnectData([In] int topicId);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(14)]
    int Heartbeat();

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(15)]
    void ServerTerminate();
}

[ComImport,
    TypeLibType((short)0x1040),
    Guid("A43788C1-D91B-11D3-8F39-00C04F3651B8")]
public interface IRTDUpdateEvent
{
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(10),
        PreserveSig]
    void UpdateNotify();

    [DispId(11)]
    int HeartbeatInterval
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(11)]
        get; [param: In]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(11)]
        set;
    }

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(12)]
    void Disconnect();
}