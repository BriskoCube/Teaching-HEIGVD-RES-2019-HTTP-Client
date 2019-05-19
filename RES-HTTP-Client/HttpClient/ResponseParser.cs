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
        public class ResponseParser
        {
            const int BUFFER_SIZE = 65536;
            byte[] buffer = new byte[BUFFER_SIZE];

            Response response;
            StringBuilder rawHeader;
            StringBuilder rawData;


            int byteRead, dataLength;
            char lastChar;
            bool parsingBody = false;


            public ResponseParser()
            {
            }

            private void Reset()
            {
                response = new Response();
                response.Headers = new List<Header> { };
                rawHeader = new StringBuilder();
                rawData = new StringBuilder();
                dataLength = 0;
                byteRead = 0;
                lastChar = ' ';
                parsingBody = false;
            }

            public Response Parse(NetworkStream serverStream)
            {
                Reset();

                do
                {
                    byteRead = serverStream.Read(buffer, 0, BUFFER_SIZE);

                    string all = Encoding.UTF8.GetString(buffer, 0, byteRead);

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
                                        StartParsingBody(i);
                                        parsingBody = true;
                                    }
                                    else
                                    {
                                        HandleHeader();
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

            private void HandleHeader()
            {
                string stringHeader = rawHeader.ToString();
                Header header = Header.Parse(stringHeader);

                if (header == null)
                {
                    ParseResponseLine(stringHeader);
                }
                else
                {
                    response.Headers.Add(header);
                }
            }

            private void StartParsingBody(int i)
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


            private void ParseResponseLine(string line)
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
        }
    }
}
