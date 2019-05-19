using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RES_HTTP_Client.HttpClient.HttpHeader
{
    public abstract class Header
    {
        string key = "";

        public Header(string key)
        {
            this.key = key;
        }
        
        public static Header Parse(string rawHeader)
        {
            string[] headerParts = rawHeader.Replace("\r\n", "").Split(": ");

            if(headerParts.Length >= 2)
            {
                return new SingleHeader(headerParts[0], string.Join(": ", headerParts.Skip(1)));
            }

            return null;
        }

        public string Key { get => key; set => key = value; }

        public string Formatted()
        {
            return string.Format("{0}: {1}\r\n", Key, Value());
        }

        abstract public string Value();
        abstract public string[] Values();
    }
}
