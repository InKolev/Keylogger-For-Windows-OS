using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace HIT.Services
{
    public class HttpService : IHttpService
    {
        public async Task SendAsBson<T>(T data, string baseAddress, string requestUrl)
        {
            // TODO: Introduce an HttpClientProvider to detach from the HttpClient type
            // and make the code more testable
            using (var client = new HttpClient())
            {
                // Set the Base address for all request uri's
                client.BaseAddress = new Uri(baseAddress);

                // Set the Accept header to ContentType BSON
                var mediaType = new MediaTypeWithQualityHeaderValue("application/bson");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(mediaType);

                // POST using a BSON formatter
                var bsonFormatter = new BsonMediaTypeFormatter();
                var result = await client.PostAsync(requestUrl, data, bsonFormatter);

                result.EnsureSuccessStatusCode();
            }
        }

        public async Task SendAsBson<T>(T data, string requestUrl)
        {
            // TODO: Introduce an HttpClientProvider to detach from the HttpClient type
            // and make the code more testable
            using (var client = new HttpClient())
            {
                // Set the Accept header to ContentType BSON
                var mediaType = new MediaTypeWithQualityHeaderValue("application/bson");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(mediaType);

                // POST using a BSON formatter
                var bsonFormatter = new BsonMediaTypeFormatter();
                var result = await client.PostAsync(requestUrl, data, bsonFormatter);

                result.EnsureSuccessStatusCode();
            }
        }
    }
}
