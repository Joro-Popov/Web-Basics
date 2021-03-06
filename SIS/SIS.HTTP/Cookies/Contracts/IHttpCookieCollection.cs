﻿namespace SIS.HTTP.Cookies.Contracts
{
    using System.Collections.Generic;

    public interface IHttpCookieCollection : IEnumerable<HttpCookie>
    {
        void Add(HttpCookie cookie);
        
        HttpCookie GetCookie(string key);

        bool ContainsCookie(string key);

        bool HasCookies();
    }
}
