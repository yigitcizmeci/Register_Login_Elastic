using System.ComponentModel.DataAnnotations;

namespace Register_Login_Elasticsearch.Models
{
    public class Users
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public required string Name { get; set; }
        [Required(ErrorMessage = "Surname is required")]
        public required string Surname { get; set; }
        [Required(ErrorMessage = "UserName is required")]
        public required string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }
        [Required(ErrorMessage = "E-mail is required")]
        public required string Email { get; set; }


    }
}
