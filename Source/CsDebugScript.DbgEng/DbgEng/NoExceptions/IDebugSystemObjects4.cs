using System;
using System.Runtime.InteropServices;
using System.Text;

namespace DbgEng.NoExceptions
{
    [ComImport, Guid("489468E6-7D0F-4AF5-87AB-25207454D553"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugSystemObjects4 : IDebugSystemObjects3
    {
#pragma warning disable CS0108 // XXX hides inherited member. This is COM default.

        #region IDebugSystemObjects
        [PreserveSig]
        int GetEventThread(
            [Out] out uint Thread);

        [PreserveSig]
        int GetEventProcess(
            [Out] out uint Process);

        [PreserveSig]
        int GetCurrentThreadId(
            [Out] out uint Id);

        [PreserveSig]
        int SetCurrentThreadId(
            [In] uint Id);

        [PreserveSig]
        int GetCurrentProcessId(
            [Out] out uint Id);

        [PreserveSig]
        int SetCurrentProcessId(
            [In] uint Id);

        [PreserveSig]
        int GetNumberThreads(
            [Out] out uint Number);

        [PreserveSig]
        int GetTotalNumberThreads(
            [Out] out uint Total,
            [Out] out uint LargestProcess);

        [PreserveSig]
        int GetThreadIdsByIndex(
            [In] uint Start,
            [In] uint Count,
            [Out] out uint Ids,
            [Out] out uint SysIds);

        [PreserveSig]
        int GetThreadIdByProcessor(
            [In] uint Processor,
            [Out] out uint Id);

        [PreserveSig]
        int GetCurrentThreadDataOffset(
            [Out] out ulong Offset);

        [PreserveSig]
        int GetThreadIdByDataOffset(
            [In] ulong Offset,
            [Out] out uint Id);

        [PreserveSig]
        int GetCurrentThreadTeb(
            [Out] out ulong Teb);

        [PreserveSig]
        int GetThreadIdByTeb(
            [In] ulong Offset,
            [Out] out uint Id);

        [PreserveSig]
        int GetCurrentThreadSystemId(
            [Out] out uint Id);

        [PreserveSig]
        int GetThreadIdBySystemId(
            [In] uint SysId,
            [Out] out uint Id);

        [PreserveSig]
        int GetCurrentThreadHandle(
            [Out] out ulong Handle);

        [PreserveSig]
        int GetThreadIdByHandle(
            [In] ulong Handle,
            [Out] out uint Id);

        [PreserveSig]
        int GetNumberProcesses(
            [Out] out uint Number);

        [PreserveSig]
        int GetProcessIdsByIndex(
            [In] uint Start,
            [In] uint Count,
            [Out] out uint Ids,
            [Out] out uint SysIds);

        [PreserveSig]
        int GetCurrentProcessDataOffset(
            [Out] out ulong Offset);

        [PreserveSig]
        int GetProcessIdByDataOffset(
            [In] ulong Offset,
            [Out] uint Id);

        [PreserveSig]
        int GetCurrentProcessPeb(
            [Out] out ulong Peb);

        [PreserveSig]
        int GetProcessIdByPeb(
            [In] ulong Offset,
            [Out] out uint Id);

        [PreserveSig]
        int GetCurrentProcessSystemId(
            [Out] out uint Id);

        [PreserveSig]
        int GetProcessIdBySystemId(
            [In] uint SysId,
            [Out] out uint Id);

        [PreserveSig]
        int GetCurrentProcessHandle(
            [Out] out ulong Handle);

        [PreserveSig]
        int GetProcessIdByHandle(
            [In] ulong Handle,
            [Out] out uint Id);

        [PreserveSig]
        int GetCurrentProcessExecutableName(
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] uint BufferSize,
            [Out] out uint ExeSize);
        #endregion

        #region IDebugSystemObjects2
        [PreserveSig]
        int GetCurrentProcessUpTime(
            [Out] out uint Uptime);

        [PreserveSig]
        int GetImplicitThreadDataOffset(
            [Out] out ulong Offset);

        [PreserveSig]
        int SetImplicitThreadDataOffset(
            [In] ulong Offset);

        [PreserveSig]
        int GetImplicitProcessDataOffset(
            [Out] out ulong Offset);

        [PreserveSig]
        int SetImplicitProcessDataOffset(
            [In] ulong Offset);
        #endregion

        #region IDebugSystemObjects3
        [PreserveSig]
        int GetEventSystem(
            [Out] out uint System);

        [PreserveSig]
        int GetCurrentSystemId(
            [Out] out uint Id);

        [PreserveSig]
        int SetCurrentSystemId(
            [In] uint Id);

        [PreserveSig]
        int GetNumberSystems(
            [Out] out uint Number);

        [PreserveSig]
        int GetSystemIdsByIndex(
            [In] uint Start,
            [In] uint Count,
            [Out, MarshalAs(UnmanagedType.LPArray)] uint[] Ids);

        int GetTotalNumberThreadsAndProcesses(
            [Out] out uint TotalThreads,
            [Out] out uint TotalProcesses,
            [Out] out uint LargestProcessThreads,
            [Out] out uint LargestSystemThreads,
            [Out] out uint LargestSystemProcesses);

        [PreserveSig]
        int GetCurrentSystemServer(
            [Out] out ulong Server);

        [PreserveSig]
        int GetSystemByServer(
            [In] ulong Server,
            [Out] out uint System);

        [PreserveSig]
        int GetCurrentSystemServerName(
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder Buffer,
            [In] uint BufferSize,
            [Out] out uint NameSize);
        #endregion

#pragma warning restore CS0108 // XXX hides inherited member. This is COM default.

        [PreserveSig]
        int GetCurrentProcessExecutableNameWide(
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] uint BufferSize,
            [Out] out uint ExeSize);

        [PreserveSig]
        int GetCurrentSystemServerNameWide(
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder Buffer,
            [In] uint BufferSize,
            [Out] out uint NameSize);
    }
}
