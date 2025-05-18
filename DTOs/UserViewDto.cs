namespace Pet_s_Land.DTOs
{
    public class UserViewDto
    {
        public class UserViewDTO
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
            public bool IsBlocked { get; set; }
            public int TotalCartItems { get; set; }  // New property
            public int TotalOrders { get; set; }

        }
    }
}
