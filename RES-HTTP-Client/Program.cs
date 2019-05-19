using RES_HTTP_Client.HttpClient;
using System;
using System.Collections.Generic;

namespace RES_HTTP_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            SendRequest("localhost", new Request("/"));
            SendRequest("localhost", new Request("/phpmyadmin"));
            SendRequest("localhost", new Request("/phpmyadmin/"));
            SendRequest("google.com", new Request("/"));
            SendRequest("google.ch", new Request("/asdasdasdasd"));
            SendRequest("test.ch", new Request("/"));

            Console.ReadLine();

        }

        static void SendRequest(string url, Request request)
        {
            Client client = new Client("localhost", 80);

            client.Send(request);

            Response serverResponse = client.Read();

            Console.WriteLine("The server has responded with {0} {1}, the body is {2} bytes long", serverResponse.Status, serverResponse.StatusMessage, serverResponse.Body.Length);
        }
    }
}
