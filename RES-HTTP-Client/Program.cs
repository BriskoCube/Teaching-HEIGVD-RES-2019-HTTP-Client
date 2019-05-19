using RES_HTTP_Client.HttpClient;
using System;
using System.Collections.Generic;

namespace RES_HTTP_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client("localhost", 80);

            client.Send(new Request("/"));

            Response serverResponse = client.Read();

            Console.WriteLine("The server has responded with {0} {1}, the body is {2} bytes long", serverResponse.Status, serverResponse.StatusMessage, serverResponse.Body.Length);

            Console.ReadLine();

        }
    }
}
