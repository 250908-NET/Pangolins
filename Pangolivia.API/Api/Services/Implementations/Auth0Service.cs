using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Pangolivia.API.DTOs;
using Pangolivia.API.Models;
using Pangolivia.API.Repositories;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;

namespace Pangolivia.API.Services
{
    public class Auth0Service : IAuth0Service
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly string _domain;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public Auth0Service(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _domain = _configuration["Auth0:Domain"] ?? throw new ArgumentNullException("Auth0:Domain");
            _clientId = _configuration["Auth0:ClientId"] ?? throw new ArgumentNullException("Auth0:ClientId");
            _clientSecret = _configuration["Auth0:ClientSecret"] ?? throw new ArgumentNullException("Auth0:ClientSecret");
        }

        public async Task<Auth0User> GetUsersInfo(string userSub)
        {
            try
            {
                // Get Management API token
                var auth0Client = new AuthenticationApiClient(_domain);
                var token = await auth0Client.GetTokenAsync(new ClientCredentialsTokenRequest
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret,
                    Audience = $"https://{_domain}/api/v2/"
                });

                // Use Management API to get user profile
                var managementClient = new ManagementApiClient(token.AccessToken, _domain);
                var user = await managementClient.Users.GetAsync(userSub);

                if (user == null)
                {
                    throw new KeyNotFoundException($"User with ID {userSub} not found");
                }

                return new Auth0User()
                {
                    Sub = userSub,
                    Nickname = user.NickName ?? string.Empty, // Provide default value if null
                    ProfilePictureUrl = user.Picture ?? string.Empty // Provide default value if null
                };
            }
            catch (Exception ex)
            {
                // Log the error here
                throw; // Re-throw to maintain the stack trace
            }
        }
    }
}