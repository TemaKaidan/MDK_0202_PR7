using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PR7_HttpNaewPAT_Mohov
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                using (StreamWriter file = new StreamWriter("debug.log", true))
                {
                    Trace.Listeners.Add(new TextWriterTraceListener(file));
                    Trace.AutoFlush = true;
                    await SingIn("student", "Asdfg123");
                    await AddRecord("Новая запись", "Описание для добавленной новой записи.", "Здесь должна быть картинка)");
                    string htmlCode = await GetHtmlFromUrl("http://news.permaviat.ru/main");
                    ParsingHtml(htmlCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
                Trace.WriteLine($"Произошла ошибка: {ex.Message}");
            }
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

        public static async Task<string> GetHtmlFromUrl(string url)
        {
            Trace.WriteLine($"Выполняем запрос: {url}");
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                Trace.WriteLine($"Статус выполнения: {response.StatusCode}");
                string htmlCode = await response.Content.ReadAsStringAsync();
                return htmlCode;
            }
        }

        public static void ParsingHtml(string htmlCode)
        {
            var html = new HtmlDocument();
            html.LoadHtml(htmlCode);
            var Document = html.DocumentNode;
            var Content = Document.Descendants(0).Where(n => n.HasClass(""));
            foreach (HtmlNode content in Content)
            {
                var src = content.ChildNodes[1].GetAttributeValue("src", "none");
                var name = content.ChildNodes[3].InnerText;
                var description = content.ChildNodes[5].InnerText;
                Console.WriteLine(name + "\n" + "Изображение: " + src + "\n" + "Описание: " + description);
            }
        }
    }
}