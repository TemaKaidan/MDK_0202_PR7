using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PR7_HttpNaewPAT_Mohov
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            WebRequest request = WebRequest.Create("");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Console.WriteLine(response.StatusDescription);
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string data = reader.ReadToEnd();
            Console.WriteLine(data);
            reader.Close();
            dataStream.Close();
            response.Close();
            Console.Read();
        }
    }
}