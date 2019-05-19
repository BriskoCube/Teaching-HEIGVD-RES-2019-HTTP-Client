using System;
using System.Collections.Generic;
using System.Text;

namespace RES_HTTP_Client.HttpClient.HttpHeader
{
    public class SingleHeader : Header
    {
        string value = "";

        public SingleHeader(string key, string value) : base(key)
        {
            this.value = value;
        }

        public override string Value()
        {
            return value;
        }

        public override string[] Values()
        {
            return new string[] { value };
        }
    }
}
