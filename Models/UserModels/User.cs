﻿using System.ComponentModel.DataAnnotations;
using System.Net;
using Pet_s_Land.Models.AdressModels;
using Pet_s_Land.Models.CartModels;
using Pet_s_Land.Models.OrderModels;
using Pet_s_Land.Models.WhishlistModel;

namespace Pet_s_Land.Models.UserModels
{
    public class User
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "name is required")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }

        [Required]
        [StringLength(15)]
        [RegularExpression(@"^\+?[1-9][0-9]{7,14}$", ErrorMessage = "Invalid phone number.")]
        public string? PhoneNumber { get; set; }


        [Required(ErrorMessage ="User Name is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        [RegularExpression(@"^(?=.[A-Za-z])(?=.\d)(?=.[@$!%?&])[A-Za-z\d@$!%*?&]{8,}$",
         ErrorMessage = "Password must contain at least one letter, one number, and one special character.")]
        public string Password { get; set; }

        public string? Role { get; set; } = "User";
        public bool IsBlocked { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public virtual Cart? Cart { get; set; }

        public virtual List<Order> Orders { get; set; }
        public List<WishList> WishList { get; set; }

        public ICollection<Address> Addresses { get; set; }
    }
}
