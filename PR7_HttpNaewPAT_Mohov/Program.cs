using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PR7_HttpNaewPAT_Mohov
{
    public class Program
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

        public static async Task SingIn(string Login, string Password)
        {
            string url = "http://news.permaviat.ru/ajax/login.php";
            Trace.WriteLine($"Выполняем запрос: {url}");
            using (HttpClient client = new HttpClient())
            {
                var formData = new Dictionary<string, string>
                {
                    { "login", Login },
                    { "password", Password }
                };
                var content = new FormUrlEncodedContent(formData);
                HttpResponseMessage response = await client.PostAsync(url, content);
                Trace.WriteLine($"Статус выполнения: {response.StatusCode}");
                string responseFromServer = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseFromServer);
            }
        }

        public static void GetContent(Cookie Token)
        {
            string url = "";
            Debug.WriteLine($"Выполняем запрос: {url}");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(Token);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Debug.WriteLine($"Статус выполнения: {response.StatusCode}");
            string responseFromServer = new StreamReader(response.GetResponseStream()).ReadToEnd();
            Console.WriteLine(responseFromServer);
        }

        public static void ParsingHtml(string htmlCode)
        {
            var html = new HtmlDocument();
            html.LoadHtml(htmlCode);
            var Document = html.DocumentNode;
            IEnumerable<HtmlNode> Content = Document.Descendants(0).Where(n => n.HasClass(""));
            foreach (HtmlNode content in Content)
            {
                var src = content.ChildNodes[1].GetAttributeValue("src", "none");
                var name = content.ChildNodes[3].InnerText;
                var description = content.ChildNodes[5].InnerText;
                Console.WriteLine(name + "\n" + "Изображение: " + src + "\n" + "Описание: " + description);
            }
        }
        public static string GetHtmlFromUrl(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string htmlCode = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return htmlCode;
        }

        public static async Task AddRecord(string title, string description, string imageUrl)
        {
            string url = "http://news.permaviat.ru/add";
            Trace.WriteLine($"Выполняем запрос: {url}");
            using (HttpClient client = new HttpClient())
            {
                var formData = new Dictionary<string, string>
                {
                    { "title", title },
                    { "description", description },
                    { "image", imageUrl }
                };
                var content = new FormUrlEncodedContent(formData);
                HttpResponseMessage response = await client.PostAsync(url, content);
                Trace.WriteLine($"Статус выполнения: {response.StatusCode}");
                string responseFromServer = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseFromServer);
            }
        }
    }
}