using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;

class Program
{
    public static async Task Main(string[] args)
    {
        string apiKey = "";
        Console.Write("Example propmpts: ");
        string prompt;
        prompt = Console.ReadLine();
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            var requestBody = new
            {
                prompt = prompt,
                n = 1,
                size = "1024x1024"
            };

            string JsonBody = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(JsonBody, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/images/generations", content);
            string repsonseString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(repsonseString);


        }
        //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
    }
}