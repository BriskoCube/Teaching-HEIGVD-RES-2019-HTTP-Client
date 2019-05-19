using RES_HTTP_Client.HttpClient.HttpHeader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace RES_HTTP_Client.HttpClient
{
    public class Response
    {
        public static Response Parse(NetworkStream serverStream, int bufferSize = 4096)
        {
            

            Response response = new Response();
            response.Headers = new List<Header> { };

            byte[] buffer = new byte[bufferSize];
            int byteRead = 0;
            char lastChar = ' ';
            StringBuilder rawHeader = new StringBuilder();
            StringBuilder rawData = new StringBuilder();
            int dataLength = 0;

            bool parsingBody = false;

            do
            {
                byteRead = serverStream.Read(buffer, 0, bufferSize);

                if (parsingBody)
                {
                    rawData.Append(Encoding.UTF8.GetString(buffer, 0, byteRead));
                }

                for (int i = 0; i < byteRead && !parsingBody; i++)
                {
                    switch ((char)buffer[i])
                    {
                        case '\r':
                            break;
                        case '\n':
                            if (lastChar == '\r')
                            {
                                // If header is empty we have two CRLF wich is the end of headers
                                if (rawHeader.Length == 0)
                                {
                                    StartParsingBody(response, buffer, byteRead, rawData, out dataLength, i);
                                    parsingBody = true;
                                }
                                else
                                {
                                    HandleHeader(response, rawHeader);
                                }
                                rawHeader.Clear();
                            }

                            break;
                        default:
                            rawHeader.Append((char)buffer[i]);
                            break;
                    }

                    lastChar = (char)buffer[i];
                }
            } while (serverStream.DataAvailable);

            response.Body = rawData.ToString();

            return response;
        }

        private static void HandleHeader(Response response, StringBuilder rawHeader)
        {
            string stringHeader = rawHeader.ToString();
            Header header = Header.Parse(stringHeader);

            if (header == null)
            {
                ParseResponseLine(response, stringHeader);
            }
            else
            {
                response.Headers.Add(header);
            }
        }

        private static void StartParsingBody(Response response, byte[] buffer, int byteRead, StringBuilder rawData, out int dataLength, int i)
        {
            rawData.Append(Encoding.UTF8.GetString(buffer, i, byteRead - i));

            var contentLength = response.Headers.Where(header => header.Key.ToLower() == "content-length");

            dataLength = int.MaxValue;

            if (contentLength.Count() == 1)
            {
                string strValue = contentLength.ElementAt(0).Value();
                int value = 0;
                if (int.TryParse(strValue, out value))
                {
                    dataLength = value;
                }
            }
        }


        private static void ParseResponseLine(Response response, string line)
        {
            string[] data = line.Split(" ");


            if (data.Length >= 3)
            {
                int status = 0;
                if (!int.TryParse(data[1], out status))
                {
                    throw new Exception("Error while parsing server response");
                }

                response.Status = status;

                response.Version = data[0];
                response.StatusMessage = string.Join(" ", data.Skip(2));
            }

        }

        public string Body { get; private set; }

        public int Status { get; private set; }

        public string StatusMessage { get; private set; }

        public string Version { get; private set; }

        public List<Header> Headers { get; private set; }
    }
}
