
using ErsteAPI_TextKorrektur;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace TextKorrekturApp
{
    class Program
    {
        private static string apiKey = "";

        static async Task Main(string[] args)
        {
            apiKey = Environment.GetEnvironmentVariable("APIKey");
            await RunChatLoop();
        }

        static async Task RunChatLoop()
        {
            bool keepRunnig = true;
            var openAIConnector = new OpenAIConnector(apiKey);
            
            while(keepRunnig)
            {
                var message = Console.ReadLine();
                var result = await openAIConnector.Prompt(message);
                Console.WriteLine(result);
            }
        }
    }

}
