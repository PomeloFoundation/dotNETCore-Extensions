using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System
{
    public static class PomeloForStringExtensions
    {
        public static int[] IndexOfMany(this string self, string substr)
        {
            var ret = new List<int>();
            var ch1 = self.ToCharArray();
            var ch2 = substr.ToCharArray();
            for (var i = 0; i <= ch1.Length - ch2.Length; i++)
            {
                var flag = true;
                for (var j = 0; j < ch2.Length; i++)
                {
                    if (ch1[i + j] != ch2[j])
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                    ret.Add(i);
            }
            return ret.ToArray();
        }

        public static int CountSubString(this string self, string substr)
        {
            return self.IndexOfMany(substr).Count();
        }


        public static string PopFrontMatch(this string self, string str)
        {
            if (str.Length > self.Length)
                return self;
            else if (self.IndexOf(str) == 0)
                return self.Substring(str.Length);
            else
                return self;
        }

        public static string PopBackMatch(this string self, string str)
        {
            if (str.Length > self.Length)
                return self;
            else if (self.LastIndexOf(str) == self.Length - str.Length)
                return self.Substring(0, self.Length - str.Length);
            else
                return self;
        }

        public static string PopFront(this string self, int count = 1)
        {
            if (count > self.Length || count < 0)
                throw new IndexOutOfRangeException();
            return self.Substring(count);
        }

        public static string PopBack(this string self, int count = 1)
        {
            if (count > self.Length || count < 0)
                throw new IndexOutOfRangeException();
            return self.Substring(0, self.Length - count);
        }
    }
}
