using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Http
{
    public class SmartCookies : IDictionary<string, string>
    {
        public HttpContext httpContext { get; }

        public SmartCookies(IHttpContextAccessor accessor)
        {
            httpContext = accessor.HttpContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expires">seconds</param>
        /// <returns></returns>
        public string this[string key, long expires]
        {
            set
            {
                var time = DateTime.Now.AddSeconds(expires);
                httpContext.Response.Cookies.Append(key, value, new CookieOptions { Expires = time });
            }
        }

        public string this[string key]
        {
            get
            {
                try
                {
                    if (httpContext.Request.Cookies.Keys.Contains(key))
                        return httpContext.Request.Cookies[key];
                    else
                        return null;
                }
                catch
                {
                    return null;
                }
            }

            set
            {
                httpContext.Response.Cookies.Delete(key);
                httpContext.Response.Cookies.Append(key, value);
            }
        }

        public int Count
        {
            get
            {
                try
                {
                    return httpContext.Request.Cookies.Count;
                }
                catch
                {
                    return -1;
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                try
                {
                    return httpContext.Request.Cookies.Keys;
                }
                catch
                {
                    return null;
                }
            }
        }

        public ICollection<string> Values
        {
            get
            {
                try
                {
                    var tmp = httpContext.Request.Cookies.Select(x => x.Value).ToList();
                    var ret = new List<string>();
                    foreach (var t in httpContext.Request.Cookies)
                        ret.Add(t.Value);
                    return ret;
                }
                catch
                {
                    return null;
                }
            }
        }

        public void Add(KeyValuePair<string, string> item)
        {
            httpContext.Response.Cookies.Delete(item.Key);
            httpContext.Response.Cookies.Append(item.Key, item.Value);
        }

        public void Add(string key, string value)
        {
            httpContext.Response.Cookies.Append(key, value);
        }

        public void Clear()
        {
            foreach (var k in Keys)
            {
                httpContext.Response.Cookies.Delete(k);
            }
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            try
            {
                if (this[item.Key] == item.Value)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public bool ContainsKey(string key)
        {
            try
            {
                if (this[key] != null)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            for (var i = arrayIndex; i < array.Count(); i++)
            {
                httpContext.Response.Cookies.Delete(array[i].Key);
                httpContext.Response.Cookies.Append(array[i].Key, array[i].Value);
            }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return new CookiesEnumerator(this);
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            if (this[item.Key] == item.Value)
            {
                httpContext.Response.Cookies.Delete(item.Key);
                return true;
            }
            return false;
        }

        public bool Remove(string key)
        {
            httpContext.Response.Cookies.Delete(key);
            return true;
        }

        public bool TryGetValue(string key, out string value)
        {
            try
            {
                if (this[key] == null)
                {
                    value = null;
                    return false;
                }
                else
                {
                    value = this[key];
                    return true;
                }
            }
            catch
            {
                value = null;
                return false;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class CookiesEnumerator : IEnumerator<KeyValuePair<string, string>>
    {
        private SmartCookies cookies;

        private int position = -1;

        public CookiesEnumerator(SmartCookies cookies)
        {
            this.cookies = cookies;
        }

        public KeyValuePair<string, string> Current
        {
            get
            {
                try
                {
                    return new KeyValuePair<string, string>(cookies.Keys.ToList()[position], cookies[cookies.Keys.ToList()[position]]);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public void Dispose()
        {
            return;
        }

        public bool MoveNext()
        {
            position++;
            return position < cookies.Count;
        }

        public void Reset()
        {
            position = -1;
        }
    }
}
