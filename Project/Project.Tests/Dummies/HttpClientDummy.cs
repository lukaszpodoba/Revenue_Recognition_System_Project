using System.Net;
using Newtonsoft.Json;

namespace Project.Tests.Dummies;

public class HttpClientDummy : HttpClient
{
    public override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var jsonResponse = new
        {
            conversion_rates = new Dictionary<string, double>
            {
                { "USD", 0.25 },
                { "PLN", 1.0 }
            }
        };

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonConvert.SerializeObject(jsonResponse))
        };

        return await Task.FromResult(responseMessage);
    }
}
