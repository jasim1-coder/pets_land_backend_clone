using System.ComponentModel.DataAnnotations;

namespace Pet_s_Land.DTOs
{
    public class UserDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        [Required(ErrorMessage = "Phone number is required")]

        public string PhoneNo { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
