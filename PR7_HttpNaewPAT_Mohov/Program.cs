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
        private static HttpClient _httpClient;
        private static string _cookie;

        static async Task Main(string[] args)
        {
            try
            {
                using (StreamWriter file = new StreamWriter("debug.log", true))
                {
                    Trace.Listeners.Add(new TextWriterTraceListener(file));
                    Trace.AutoFlush = true;
                    var handler = new HttpClientHandler();
                    handler.CookieContainer = new System.Net.CookieContainer();
                    _httpClient = new HttpClient(handler);
                    _cookie = await SingIn("admin", "admin");
                    Console.WriteLine($"Полученная Cookie: {_cookie}");
                    await AddRecord("Новая запись", "Описание для добавленной новой записи.", "https://www.permaviat.ru/_res/news/1189img.jpg");
                    string htmlCode = await GetHtmlFromUrl("http://127.0.0.1/main");
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

        public static async Task<string> SingIn(string Login, string Password)
        {
            string url = "http://127.0.0.1/ajax/login.php";
            Trace.WriteLine($"Выполняем запрос: {url}");
            var formData = new Dictionary<string, string>
            {
                { "login", Login },
                { "password", Password }
            };
            var content = new FormUrlEncodedContent(formData);
            HttpResponseMessage response = await _httpClient.PostAsync(url, content);
            Trace.WriteLine($"Статус выполнения: {response.StatusCode}");
            string responseFromServer = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Ответ сервера: {responseFromServer}");
            if (response.Headers.TryGetValues("Set-Cookie", out var cookieValues))
            {
                string cookie = cookieValues.FirstOrDefault();
                return cookie;
            }
            return null;
        }

        public static async Task AddRecord(string name, string description, string imageUrl)
        {
            string url = "http://127.0.0.1/ajax/add.php";
            Trace.WriteLine($"Выполняем запрос: {url}");
            var formData = new Dictionary<string, string>
            {
                { "name", name },
                { "description", description },
                { "src", imageUrl }
            };
            var content = new FormUrlEncodedContent(formData);
            HttpResponseMessage response = await _httpClient.PostAsync(url, content);
            Trace.WriteLine($"Статус выполнения: {response.StatusCode}");
            string responseFromServer = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Ответ сервера: {responseFromServer}");
            if (response.IsSuccessStatusCode) Console.WriteLine(responseFromServer);
            else Console.WriteLine($"Ошибка при добавлении записи: {response.StatusCode}");
        }

        public static async Task<string> GetHtmlFromUrl(string url)
        {
            Trace.WriteLine($"Выполняем запрос: {url}");
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            Trace.WriteLine($"Статус выполнения: {response.StatusCode}");
            string htmlCode = await response.Content.ReadAsStringAsync();
            return htmlCode;
        }

        public static void ParsingHtml(string htmlCode)
        {
            var html = new HtmlDocument();
            html.LoadHtml(htmlCode);
            var Document = html.DocumentNode;
            var newsItems = Document.Descendants("div").Where(n => n.HasClass("news"));
            foreach (var newsItem in newsItems)
            {
                var image = newsItem.Descendants("img").FirstOrDefault()?.GetAttributeValue("src", "none");
                var name = newsItem.Descendants("div").FirstOrDefault(n => n.HasClass("name"))?.InnerText.Trim();
                var description = newsItem.Descendants("div").FirstOrDefault(n => !n.HasClass("name"))?.InnerText.Trim();
                Console.WriteLine($"Заголовок: {name}");
                Console.WriteLine($"Картинка: {image}");
                Console.WriteLine($"Описание: {description}");
                Console.WriteLine(new string('-', 40));
            }
        }
    }
}