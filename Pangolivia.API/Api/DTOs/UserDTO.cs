namespace Pangolivia.API.DTOs
{
public class UserDTO
    {
        public int Id { get; set; }
        public string AuthUuid { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}