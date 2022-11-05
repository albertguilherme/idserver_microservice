using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using static IdentityModel.OidcConstants;

namespace Movies.Client.HttpHandlers
{
    public class AuthenticationDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public AuthenticationDelegatingHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //var cli = _httpClientFactory.CreateClient("IDPClient");

            //var tokenResponse = await cli.RequestClientCredentialsTokenAsync(_tokenRequest);
            //if (tokenResponse.IsError)
            //    throw new HttpRequestException("Something went wrong while requesting the access token");

            //request.SetBearerToken(tokenResponse.AccessToken);

            var acessToken = (await _httpContextAccessor.HttpContext
                .GetTokenAsync(OpenIdConnectParameterNames.AccessToken)) ?? "";

            if (!string.IsNullOrEmpty(acessToken))
            {
                request.SetBearerToken(acessToken);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
