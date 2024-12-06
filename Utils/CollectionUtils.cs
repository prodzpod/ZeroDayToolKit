using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ZeroDayToolKit.Utils
{
    public static class CollectionUtils
    {
        public static T[] Range<T>(this IEnumerable<T> arr, int start = 0, int end = 0)
        {
            var len = arr.Count();
            if (start < 0) start = len - start;
            if (end <= 0) end = len - end;
            return arr.Skip(start).Take(end - start).ToArray();
        }
        public static string Join<T>(this IEnumerable<T> arr, string delim) => string.Join(delim, arr);
        public static bool Identical<T>(this IEnumerable<T> _src, IEnumerable<T> _dest)
        {
            T[] src = [.. _src], dest = [.. _dest];
            if (src.Length != dest.Length) return false;
            for (int i = 0; i < src.Length; i++) if (!src[i].Equals(dest[i])) return false;
            return true;
        }
        public static T[] Shuffle<T>(this IEnumerable<T> _list)
        {
            T[] list = [.. _list];
            int n = list.Length;
            while (n > 1)
            {
                n--;
                int k = Hacknet.Utils.random.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
            return list;
        }
        public static string[] Split(this string str, string delim) => str.Split([delim], StringSplitOptions.None);
        public static string[] Split(this string str, IEnumerable<string> delim) => str.Split([..delim], StringSplitOptions.None);
        public static int GoodIndexOf(this string str, string substr, int start = 0) {
            if (start < 0) start = 0; if (start >= str.Length) return str.Length;
            var ret = str.IndexOf(substr, start);
            if (ret == -1) return str.Length;
            return ret;
        }
        public static int GoodIndexOf(this string str, IEnumerable<string> substr, int start = 0) => -substr.Max(x => -str.GoodIndexOf(x, start));
        public static string Slice(this string str, int start = 0, int end = 0)
        {
            if (start < 0) start = str.Length - start;
            if (end <= 0) end = str.Length - end;
            if (end < start) (start, end) = (end, start);
            if (start >= str.Length) return "";
            return str.Substring(start, end - start);
        }
    }
}
