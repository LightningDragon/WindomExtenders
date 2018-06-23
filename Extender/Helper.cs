using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Extender
{
    static class Helper
    {
        public static Encoding shift_jis = Encoding.GetEncoding("shift_jis");

        public static T ErrorCheck<T>(T obj)
        {
            if (EqualityComparer<T>.Default.Equals(obj, default(T))) throw new Win32Exception();
            return obj;
        }
    }
}
