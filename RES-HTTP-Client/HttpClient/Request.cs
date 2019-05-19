using RES_HTTP_Client.HttpClient.HttpHeader;
using System;
using System.Collections.Generic;
using System.Text;

namespace RES_HTTP_Client.HttpClient
{
    public class Request
    {
        public Request(string url = "/", Method method = Method.GET) : this(url, method, new List<Header> { }) { }

        public Request(string url, Method method, List<Header> headers)
        {
            Url = url;
            Method = method;
            Headers = headers;
        }

        public List<Header> Headers { get; set; }
        public Method Method { get; set; }
        public string Url { get; set; }

        public string RequestLine()
        {
            return string.Format("{0} {1} HTTP/1.1\r\n", Enum.GetName(typeof(Method), this.Method), Url);
        }
    }

    public enum Method
    {
        GET,
        POST,
        DELETE,
        PUT
    }
}
