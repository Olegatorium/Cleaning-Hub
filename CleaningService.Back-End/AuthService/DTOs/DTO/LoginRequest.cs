using System.ComponentModel.DataAnnotations;

namespace Models.DTO
{
    public class LoginRequest
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }


        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
