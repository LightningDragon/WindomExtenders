using System;
using System.Linq;
using System.Runtime.InteropServices;
using static Extender.External;
using static Extender.Helper;

namespace Extender
{
    unsafe class ProcessWrapper
    {
        bool started;
        STARTUPINFO startInfo = new STARTUPINFO() { cb = Marshal.SizeOf(typeof(STARTUPINFO)) };
        PROCESS_INFORMATION processInfo;

        public IntPtr Alloc(uint size, int type = 0x1000, int protect = 0x0040) => VirtualAllocEx(processInfo.hProcess, IntPtr.Zero, size, type, protect);

        public bool Write(IntPtr address, IntPtr buff, int size) => WriteProcessMemory(processInfo.hProcess, address, buff, size, IntPtr.Zero);

        public bool Write(IntPtr address, byte[] buff, int size) => WriteProcessMemory(processInfo.hProcess, address, buff, size, IntPtr.Zero);

        public bool Write(IntPtr address, void* buff, int size) => WriteProcessMemory(processInfo.hProcess, address, buff, size, IntPtr.Zero);

        public void WriteNops(int addres, int size) => ErrorCheck(Write((IntPtr)addres, Enumerable.Repeat<byte>(0x90, size).ToArray(), size));

        public void WriteByte(IntPtr address, byte b) => Write(address, &b, 1);

        public bool Suspend() => SuspendThread(processInfo.hThread) != -1;

        public bool Resume() => ResumeThread(processInfo.hThread) != -1;

        public bool Start(string path, string args = null, uint flags = 0)
        {
            if (!started)
            {
                started = CreateProcess(path, args, IntPtr.Zero, IntPtr.Zero, false, flags, IntPtr.Zero, null, ref startInfo, out processInfo);
            }

            return started;
        }

        public bool Exit(uint exitCode)
        {
            if (started)
            {
                started = !TerminateProcess(processInfo.hProcess, exitCode);
                return !started;
            }

            return false;
        }

        public void WriteCall(int ptr, IntPtr functPtr)
        {
            byte* data = stackalloc byte[5];
            data[0] = 0xE8;
            *((uint*)&data[1]) = (uint)functPtr - ((uint)ptr + 5);
            ErrorCheck(Write((IntPtr)ptr, data, 5));
        }

        public void WriteJump(IntPtr ptr, int jmpPtr)
        {
            byte* data = stackalloc byte[5];
            data[0] = 0xE9;
            *((uint*)&data[1]) = (uint)jmpPtr - ((uint)ptr + 5);
            ErrorCheck(Write(ptr, data, 5));
        }

        public void WriteInstruction(byte opcode, byte register, IntPtr value, int addres, int size)
        {
            byte* data = stackalloc byte[6];
            data[0] = opcode;
            data[1] = register;
            *((uint*)&data[2]) = (uint)value;

            if (size <= 6)
            {
                ErrorCheck(Write((IntPtr)addres, data, size));
            }
            else
            {
                ErrorCheck(Write((IntPtr)addres, data, 6));
                WriteNops(addres + 6, size - 6);
            }
        }
    }
}
