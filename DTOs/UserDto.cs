using System.ComponentModel.DataAnnotations;

namespace Pet_s_Land.DTOs
{
    public class UserDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int PhoneNo { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
