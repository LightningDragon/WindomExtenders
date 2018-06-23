using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SVExtender
{
    static class Helper
    {
        public static readonly byte[] NullTerminator = { 0x00 };

        public static Encoding ShiftJis = Encoding.GetEncoding("shift_jis");

        public static T ErrorCheck<T>(T obj)
        {
            if (EqualityComparer<T>.Default.Equals(obj, default(T))) throw new Win32Exception();
            return obj;
        }
    }
}
