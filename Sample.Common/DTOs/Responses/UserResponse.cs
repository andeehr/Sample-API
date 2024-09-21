namespace Sample.Common.DTOs.Responses
{
    public class UserResponse
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public IEnumerable<string> Permissions { get; set; }
    }
}