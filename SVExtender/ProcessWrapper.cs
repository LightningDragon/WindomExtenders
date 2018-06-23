using System;
using System.Linq;
using System.Runtime.InteropServices;
using static SVExtender.External;
using static SVExtender.Helper;

namespace SVExtender
{
    unsafe class ProcessWrapper
    {
        bool _started;
        Startupinfo _startInfo = new Startupinfo { cb = Marshal.SizeOf(typeof(Startupinfo)) };
        ProcessInformation _processInfo;

        public ProcessInformation ProcessInfo => _processInfo;

        public IntPtr Alloc(uint size, int type = 0x1000, int protect = 0x0040) => VirtualAllocEx(ProcessInfo.hProcess, IntPtr.Zero, size, type, protect);

        public bool Write(IntPtr address, IntPtr buff, int size) => WriteProcessMemory(ProcessInfo.hProcess, address, buff, size, IntPtr.Zero);

        public bool Write(IntPtr address, byte[] buff, int size) => WriteProcessMemory(ProcessInfo.hProcess, address, buff, size, IntPtr.Zero);

        public bool Write(IntPtr address, void* buff, int size) => WriteProcessMemory(ProcessInfo.hProcess, address, buff, size, IntPtr.Zero);

        public void WriteNops(IntPtr addres, int size) => ErrorCheck(Write(addres, Enumerable.Repeat<byte>(0x90, size).ToArray(), size));

        public void WriteByte(IntPtr address, byte b) => Write(address, &b, 1);

        public bool Suspend() => SuspendThread(ProcessInfo.hThread) != -1;

        public bool Resume() => ResumeThread(ProcessInfo.hThread) != -1;

        public bool Start(string path, string args = null, uint flags = 0)
        {
            if (!_started)
            {
                _started = CreateProcess(path, args, IntPtr.Zero, IntPtr.Zero, false, flags, IntPtr.Zero, null, ref _startInfo, out _processInfo);
            }

            return _started;
        }

        public bool Exit(uint exitCode)
        {
            if (_started)
            {
                _started = !TerminateProcess(ProcessInfo.hProcess, exitCode);
                return !_started;
            }

            return false;
        }

        public void WriteCall(IntPtr ptr, IntPtr functPtr)
        {
            byte* data = stackalloc byte[5];
            data[0] = 0xE8;
            *(uint*)&data[1] = (uint)functPtr - ((uint)ptr + 5);
            ErrorCheck(Write(ptr, data, 5));
        }

        public void WriteJump(IntPtr ptr, int jmpPtr)
        {
            byte* data = stackalloc byte[5];
            data[0] = 0xE9;
            *(uint*)&data[1] = (uint)jmpPtr - ((uint)ptr + 5);
            ErrorCheck(Write(ptr, data, 5));
        }

        public void WriteInstruction(byte opcode, byte register, IntPtr value, IntPtr addres, int size)
        {
            byte* data = stackalloc byte[6];
            data[0] = opcode;
            data[1] = register;
            *(uint*)&data[2] = (uint)value;

            if (size <= 6)
            {
                ErrorCheck(Write(addres, data, size));
            }
            else
            {
                ErrorCheck(Write(addres, data, 6));
                WriteNops(addres + 6, size - 6);
            }
        }
    }
}
