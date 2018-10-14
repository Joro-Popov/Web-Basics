using System.Collections;

namespace SIS.HTTP.Cookies
{
    using System.Collections.Generic;

    using Contracts;

    public class HttpCookieCollection : IHttpCookieCollection
    {
        private readonly Dictionary<string, HttpCookie> cookies;

        public HttpCookieCollection()
        {
            this.cookies = new Dictionary<string, HttpCookie>();
        }

        public void Add(HttpCookie cookie)
        {
            this.cookies[cookie.Key] = cookie;
        }

        public HttpCookie GetCookie(string key)
        {
            return !this.ContainsCookie(key) ? null : this.cookies[key];
        }

        public bool ContainsCookie(string key)
        {
            return this.cookies.ContainsKey(key);
        }

        public bool HasCookies()
        {
            return this.cookies.Count > 0;
        }

        public IEnumerator<HttpCookie> GetEnumerator()
        {
            foreach (var cookie in this.cookies)
            {
                yield return cookie.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString() => string.Join("; ", this.cookies.Values);
        
    }
}
