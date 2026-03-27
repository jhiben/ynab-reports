using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using YnabReports.Configuration;

namespace YnabReports.Services;

public class YnabAuthHandler : DelegatingHandler
{
    private readonly YnabApiOptions _options;

    public YnabAuthHandler(IOptions<YnabApiOptions> options)
    {
        _options = options.Value;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.AccessToken);
        return base.SendAsync(request, cancellationToken);
    }
}
