using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Movies.Client.Utils
{
    public static class RequestUriUtil
    {
        public static HttpRequestMessage AddQueryParams(this HttpRequestMessage message, Dictionary<string, string> queryStringParams)
        {
            bool startingQuestionMarkAdded = false;
            
            var sb = new StringBuilder();
            foreach (var parameter in queryStringParams)
            {
                if (parameter.Value == null)
                {
                    continue;
                }

                sb.Append(startingQuestionMarkAdded ? '&' : '?');
                sb.Append(parameter.Key);
                sb.Append('=');
                sb.Append(parameter.Value);
                startingQuestionMarkAdded = true;
            }

            message.RequestUri = new Uri((message.RequestUri!.AbsoluteUri.EndsWith(@"/") ? "" : @"/") + sb.ToString());
            return message;
        }

        public static HttpRequestMessage AddJsonBody(this HttpRequestMessage message, object body)
        {
            using var reqContent = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            message.Content = reqContent;

            return message;
        }
    }
}