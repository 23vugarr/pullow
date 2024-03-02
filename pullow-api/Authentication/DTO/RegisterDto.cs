using System.ComponentModel.DataAnnotations;

namespace pullow_api.Authentication.DTO
{
    public class RegisterDto
    {
        public string FullName { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

    }
}
