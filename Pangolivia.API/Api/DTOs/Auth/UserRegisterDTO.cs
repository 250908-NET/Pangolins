using System.ComponentModel.DataAnnotations;

namespace Pangolivia.API.DTOs
{
    public class UserRegisterDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}