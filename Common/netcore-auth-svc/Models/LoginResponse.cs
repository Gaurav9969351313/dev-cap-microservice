using System.ComponentModel.DataAnnotations;

namespace netcore_auth_svc.Models
{
    public class LoginResponse
    {
        [Required]
        public string Token { get; set; }
    }
}