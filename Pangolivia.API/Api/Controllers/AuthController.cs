using Microsoft.AspNetCore.Mvc;
using Pangolivia.API.DTOs;
using Pangolivia.API.Services;

namespace Pangolivia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        
    }
}
