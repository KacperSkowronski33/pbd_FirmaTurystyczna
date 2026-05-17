using System.Net.Http.Headers;
using Microsoft.JSInterop;

namespace BookingApp.Client.Handlers 
{
    public class JwtAuthorizationHandler : DelegatingHandler
    {
        private readonly IJSRuntime _jsRuntime;

        public JwtAuthorizationHandler(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", cancellationToken, "authToken");

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}