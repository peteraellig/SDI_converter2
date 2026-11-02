using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SdiConverter;

internal sealed record SystemLoadSample(string Cpu, string Ram, string FreeRam, string EngineCpu, string EngineRam);

/// <summary>Reads system CPU/RAM and the CPU/RAM of the running engine for the load cells at the bottom of the window.</summary>
internal sealed class SystemLoad
{
    [StructLayout(LayoutKind.Sequential)]
    private struct Memory
    {
        public uint Length, Load;
        public ulong TotalPhys, AvailPhys, TotalPage, AvailPage, TotalVirtual, AvailVirtual, AvailExtended;
    }

    [DllImport("kernel32.dll")] private static extern bool GetSystemTimes(out long idle, out long kernel, out long user);
    [DllImport("kernel32.dll")] private static extern bool GlobalMemoryStatusEx(ref Memory memory);

    public double LastCpuPercent { get; private set; }
    public double LastEngineCpuPercent { get; private set; }
    public long LastEngineRamBytes { get; private set; }
    private long? idleBefore, totalBefore;
    private long engineId, engineCpu, engineAt;

    public SystemLoadSample Read(Process? engine)
    {
        LastEngineCpuPercent = 0; LastEngineRamBytes = 0;
        string cpu = "—", ram = "—", free = "—", engineCpuText = "—", engineRam = "—";
        if (GetSystemTimes(out var idle, out var kernel, out var user))
        {
            long total = kernel + user;
            if (totalBefore is { } oldTotal && idleBefore is { } oldIdle && total > oldTotal)
            {
                LastCpuPercent = Math.Clamp(100.0 * (1 - (idle - oldIdle) / (double)(total - oldTotal)), 0, 100);
                cpu = $"{LastCpuPercent:0}%";
            }
            idleBefore = idle;
            totalBefore = total;
        }
        var memory = new Memory { Length = (uint)Marshal.SizeOf<Memory>() };
        if (GlobalMemoryStatusEx(ref memory)) { ram = $"{memory.Load}%"; free = $"{memory.AvailPhys / 1073741824.0:0.0} GB"; }
        if (engine is not null)
        {
            try
            {
                if (!engine.HasExited)
                {
                    engine.Refresh();
                    var now = Environment.TickCount64;
                    var time = engine.TotalProcessorTime.Ticks;
                    if (engine.Id == engineId && now > engineAt)
                    {
                        var percent = (time - engineCpu) / (double)(now - engineAt) / 10000 / Environment.ProcessorCount * 100;
                        LastEngineCpuPercent = Math.Clamp(percent, 0, 100);
                        engineCpuText = $"{Math.Clamp(percent, 0, 100):0.0}%";
                    }
                    LastEngineRamBytes = engine.WorkingSet64;
                    engineRam = $"{engine.WorkingSet64 / 1048576.0:0} MB";
                    engineId = engine.Id;
                    engineCpu = time;
                    engineAt = now;
                }
            }
            catch (Exception ex) when (ex is InvalidOperationException or System.ComponentModel.Win32Exception) { }
        }
        return new(cpu, ram, free, engineCpuText, engineRam);
    }
}
