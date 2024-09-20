namespace Sample.Api.Config
{
    public class JwtOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpirationHours { get; set; }
        public bool Enabled { get; set; }
        public string JwtKey { get; set; }
    }
}