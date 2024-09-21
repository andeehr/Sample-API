namespace Sample.Common.DTOs
{
    public class UserInfo
    {
        public string Username { get; set; }
        public string Role { get; set; }
        public IEnumerable<string>? Permissions { get; set; }
    }
}