using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

class Program
{
    private static readonly string apiKey = "";

    static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.Write("Lütfen analiz yapmak istediğiniz web sayfa URL'ini giriniz: ");
        string inputUrl = Console.ReadLine();

        Console.WriteLine();
        Console.WriteLine("Web sayfası içeriği: ");
        string webContent = ExtractTextFromWeb(inputUrl);
        await AnalayzeWithAI(webContent, "Web Sayfası İçeriği");
    }

    static string ExtractTextFromWeb(string url)
    {
        var web = new HtmlWeb();
        var doc = web.Load(url);

        var bodyText = doc.DocumentNode.SelectSingleNode("//body")?.InnerText;
        if (bodyText == null) return "Sayfa içeriği okunamadı.";

        var cleanedText = System.Net.WebUtility.HtmlDecode(bodyText);
        cleanedText = cleanedText.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
        cleanedText = System.Text.RegularExpressions.Regex.Replace(cleanedText, @"\s+", " ");

        return cleanedText.Trim();
    }

    static async Task AnalayzeWithAI(string text, string sourceType)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = "Sen bir yapay zeka asistanısın. Kullanıcının gönderdiği metni analiz eder ve Türkçe olarak özetlersin. Yanıtlarını sadece Türkçe ver!" },
                    new { role = "user", content = $"Analyze and summarize the following {sourceType}:\n\n {text}" }
                }
            };

            string json = JsonConvert.SerializeObject(requestBody);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
            string responseJson = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<dynamic>(responseJson);
                string aiOutput = (string)result.choices[0].message.content;

                Console.WriteLine($"\nAI Analizi ({sourceType}): \n");
                Console.WriteLine(WrapText(aiOutput, 100));
            }
            else
            {
                Console.WriteLine("Hata: " + responseJson);
            }
        }
    }

    static string WrapText(string text, int maxLineWidth)
    {
        var words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var sb = new StringBuilder();
        int currentLineLength = 0;

        foreach (var word in words)
        {
            if (currentLineLength + word.Length + 1 > maxLineWidth)
            {
                sb.AppendLine();
                currentLineLength = 0;
            }

            if (currentLineLength > 0)
            {
                sb.Append(" ");
                currentLineLength++;
            }

            sb.Append(word);
            currentLineLength += word.Length;
        }

        return sb.ToString();
    }
}
