namespace Pet_s_Land.DTOs
{
    public class JwtResponseDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }

        public DateTime Expiration { get; set; }

        public string Name { get; set; }
        public int UserId { get; set; }
    }
}
