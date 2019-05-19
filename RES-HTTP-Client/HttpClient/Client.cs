using RES_HTTP_Client.HttpClient.HttpHeader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using static RES_HTTP_Client.HttpClient.Response;

namespace RES_HTTP_Client.HttpClient
{
    public class Client
    {
        TcpClient clientSocket;
        NetworkStream serverStream;

        private static List<Header> defaultHeaders = new List<Header> {
            new SingleHeader("Host", "localhost"),
            new SingleHeader("Cache-Control", "no-cache"),
            new SingleHeader("Accept", "*/*"),
            new SingleHeader("User-Agent", "Cubic/0.1"),
        };

        public Client(string ip = "127.0.0.1", int port = 80)
        {
            clientSocket = new TcpClient();

            clientSocket.Connect(ip, port);

            serverStream = clientSocket.GetStream();
        }

        public void Send(Request request)
        {
            Console.WriteLine("Requesting {0} using {1}", request.Url, Enum.GetName(typeof(Method), request.Method));

            string data = request.RequestLine();

            var headers = MergeHeaders(request.Headers, defaultHeaders);

            foreach (Header header in headers)
            {
                data += header.Formatted();
            }

            data += "\r\n";

            byte[] outStream = Encoding.ASCII.GetBytes(data);
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
        }

        public Response Read()
        {
            ResponseParser parser = new ResponseParser();
            return parser.Parse(serverStream);
        }


        private static List<Header> MergeHeaders(List<Header> headers, List<Header> defaults)
        {
            List<Header> merged = new List<Header>(headers);

            foreach(Header defaultHeader in defaultHeaders)
            {
                if(merged.Where( header => header.Key.ToLower() == defaultHeader.Key.ToLower()).Count() == 0)
                {
                    merged.Add(defaultHeader);
                }
            }

            return merged;
        }
    }
}
