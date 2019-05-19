using System;
using System.Collections.Generic;
using System.Text;

namespace RES_HTTP_Client.HttpClient.HttpHeader
{
    public class MultipleHeader : Header
    {
        string[] values;

        public MultipleHeader(string key, params string[] values): base(key)
        {
            this.values = values;
        }

        public override string Value()
        {
            return String.Join(',', values);
        }

        public override string[] Values()
        {
            return values;
        }
    }
}
