using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ErsteAPI_TextKorrektur
{
    class OpenAIConnector
    {
        private const string Model = "gpt-4o";
        private const int Temperature = 1;
        private const int MaxTokens = 2048;
        private const int TopP = 1;
        private const int FrequencyPenalty = 0;
        private const int PresencePenalty = 0;
        private readonly string _apikey;
        private readonly HttpClient _httpClient;

        public OpenAIConnector(string apikey)
        {
            _apikey = apikey;
            _httpClient = new HttpClient();
            ConfigureHttpClient(); 
        }

        public async Task<string?> Prompt(string message, string responseFormat = "")
        {
            var payload = new StringContent(CreatePayload(message), Encoding.UTF8, mediaType: "application/json");
            var result = await _httpClient.PostAsync(requestUri: "/v1/chat/completions", payload);
            var response = await result.Content.ReadAsStringAsync();

            return ParseResponse(response);
        }

        private string ParseResponse(string response)
        {
            var responseObject = JsonNode.Parse(response);
            return responseObject["choices"]?[0]?["message"]?["content"]?.GetValue<string>() ?? "Sorry i dont have a response";
        }

        private void ConfigureHttpClient()
        {
            _httpClient.BaseAddress = new Uri("https://api.openai.com");
            _httpClient.DefaultRequestHeaders.Add(name: "Authorization", $"Bearer {_apikey}");
        }

        private static string CreatePayload(string message, string responseFormat = "text")
        {
            JsonObject payload = new JsonObject();
            JsonObject responseFormatObject = new JsonObject();
            JsonObject messageObject = new JsonObject();
            JsonObject messageContentObject = new JsonObject();
            JsonArray messageContentArray = new JsonArray();
            JsonArray messageArray = new JsonArray();

            messageContentObject.Add(propertyName: "type", "text");
            messageContentObject.Add(propertyName: "text", message);
            messageContentArray.Add(messageContentObject);

            messageObject.Add(propertyName: "role", "user");
            messageObject.Add(propertyName: "content", messageContentArray);
            messageArray.Add(messageObject);
            payload.Add(propertyName: "messages", messageArray);

            responseFormatObject.Add(propertyName: "type", responseFormat);
            payload.Add(propertyName: "response_format", responseFormatObject);

            payload.Add(propertyName: "model", Model);
            payload.Add(propertyName: "temperature", Temperature);
            payload.Add(propertyName: "max_tokens", MaxTokens);
            payload.Add(propertyName: "top_p", TopP);
            payload.Add(propertyName: "frequency_penalty", FrequencyPenalty);
            payload.Add(propertyName: "presence_penalty", PresencePenalty);

            return payload.ToJsonString();
        }
    }
}
