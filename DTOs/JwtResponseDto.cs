namespace Pet_s_Land.DTOs
{
    public class JwtResponseDto
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
