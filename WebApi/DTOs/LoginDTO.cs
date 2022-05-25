using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs
{
    public class LoginDTO
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
