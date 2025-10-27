using Pangolivia.API.DTOs;
using Pangolivia.API.Models;

namespace Pangolivia.API.Services
{
    public interface IAuth0Service
    {
        // auth0|123876123876123
        // google|123876123876123

        Task<Auth0User> GetUsersInfo(string userSub);
    }
}
