namespace Sample.Data.Entities
{
    public class Role : BaseEntity<int>
    {
        public Role() : base()
        {
            Users = new HashSet<User>();
            Permissions = new HashSet<Permission>();
        }

        public string Description { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Permission> Permissions { get; set; }
    }
}