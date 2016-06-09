using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.Data.Excel.Infrastructure
{
    public class ColNumber 
    {
        public ColNumber(string number = null)
        {
            value = number;
        }

        private string value = null;

        public static bool operator >(ColNumber a, ColNumber b)
        {
            return string.Compare(a.value, b.value) > 0; 
        }

        public static bool operator <(ColNumber a, ColNumber b)
        {
            return string.Compare(a.value, b.value) < 0;
        }

        public static bool operator >=(ColNumber a, ColNumber b)
        {
            return string.Compare(a.value, b.value) >= 0;
        }

        public static bool operator <=(ColNumber a, ColNumber b)
        {
            return string.Compare(a.value, b.value) <= 0;
        }

        public static bool operator !=(ColNumber a, ColNumber b)
        {
            return a.value != b.value;
        }

        public static bool operator ==(ColNumber a, ColNumber b)
        {
            return a.value == b.value;
        }

        public static ColNumber operator ++(ColNumber a)
        {
            var tmp = FromNumberSystem26(a.value);
            tmp++;
            return new ColNumber(ToNumberSystem26(tmp));
        }

        public static ColNumber operator --(ColNumber a)
        {
            var tmp = FromNumberSystem26(a.value);
            tmp--;
            return new ColNumber(ToNumberSystem26(tmp));
        }

        public static implicit operator ColNumber(string s)
        {
            return new ColNumber(s);
        }

        public static implicit operator string(ColNumber rn)
        {
            return rn.value;
        }

        public static string ToNumberSystem26(ulong n)
        {
            string s = string.Empty;
            while (n > 0)
            {
                var m = n % 26;
                if (m == 0) m = 26;
                s = (char)(m + 64) + s;
                n = (n - m) / 26;
            }
            return s;
        }

        public override bool Equals(object obj)
        {
            return this == obj as ColNumber;
        }

        public override int GetHashCode()
        {
            return Convert.ToInt32(FromNumberSystem26(this.value));
        }
        
        public static ulong FromNumberSystem26(string s)
        {
            if (string.IsNullOrEmpty(s)) return 0;
            long n = 0;
            for (long i = s.Length - 1, j = 1; i >= 0; i--, j *= 26)
            {
                char c = Char.ToUpper(s[Convert.ToInt32(i)]);
                if (c < 'A' || c > 'Z') return 0;
                n += (c - 64) * j;
            }
            return Convert.ToUInt64(n);
        }

        public override string ToString()
        {
            return value;
        }
    }
}
