using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Movies.Client.Models;
using Movies.Client.Utils;
using Newtonsoft.Json;
using System.Text;

namespace Movies.Client.ApiServices
{
    public class MovieApiService : IMovieApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MovieApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async  Task<IEnumerable<Movie>> GetMovies()
        {
            var cli = _httpClientFactory.CreateClient("MovieAPIClient");

            var req = new HttpRequestMessage(HttpMethod.Get, "movies");
            
            var response = await cli.SendAsync(req, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var movies = JsonConvert.DeserializeObject<List<Movie>>(content);
            return movies!;
        }

        public async Task<Movie> GetMovie(int id)
        {
            var cli = _httpClientFactory.CreateClient("MovieAPIClient");

            var req = new HttpRequestMessage(HttpMethod.Get, $"movies/{id}");

            var response = await cli.SendAsync(req, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var movies = JsonConvert.DeserializeObject<Movie>(content);
            return movies!;
        }

        public async Task<Movie> UpdateMovie(Movie movie)
        {
            var cli = _httpClientFactory.CreateClient("MovieAPIClient");

            var req = new HttpRequestMessage(HttpMethod.Put, $"movies/{movie.Id}").AddJsonBody(movie);
            
            var response = await cli.SendAsync(req, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var movies = JsonConvert.DeserializeObject<Movie>(content);
            return movies!;
        }


        public async Task<Movie> CreateMovie(Movie movie)
        {
            var cli = _httpClientFactory.CreateClient("MovieAPIClient");

            var req = new HttpRequestMessage(HttpMethod.Post, $"movies").AddJsonBody(movie);
            
            var response = await cli.SendAsync(req, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var movies = JsonConvert.DeserializeObject<Movie>(content);
            return movies!;
        }


        public async Task DeleteMovie(int id)
        {
            var cli = _httpClientFactory.CreateClient("MovieAPIClient");

            var req = new HttpRequestMessage(HttpMethod.Delete, $"movies/{id}");

            var response = await cli.SendAsync(req, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
        }

        public async Task<UserInfoViewModel> GetUserInfo()
        {
            var idpClient = _httpClientFactory.CreateClient("IDPClient");

            var metaDataResponse = await idpClient.GetDiscoveryDocumentAsync();

            if (metaDataResponse.IsError)
            {
                throw new HttpRequestException("Something went wrong while requesting the access token");
            }

            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var userInfoResponse = await idpClient.GetUserInfoAsync(
               new UserInfoRequest
               {
                   Address = metaDataResponse.UserInfoEndpoint,
                   Token = accessToken
               });

            if (userInfoResponse.IsError)
            {
                throw new HttpRequestException("Something went wrong while getting user info");
            }

            var userInfoDictionary = new Dictionary<string, string>();

            foreach (var claim in userInfoResponse.Claims)
            {
                if (!userInfoDictionary.ContainsKey(claim.Type))
                    userInfoDictionary.Add(claim.Type, claim.Value);
                else
                    userInfoDictionary[claim.Type] += $",{claim.Value}"; 
            }

            return new UserInfoViewModel(userInfoDictionary);
        }
    }
}
