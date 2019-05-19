using RES_HTTP_Client.HttpClient.HttpHeader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace RES_HTTP_Client.HttpClient
{
    public partial class Response
    {


        public string Body { get; private set; }

        public int Status { get; private set; }

        public string StatusMessage { get; private set; }

        public string Version { get; private set; }

        public List<Header> Headers { get; private set; }
    }
}
