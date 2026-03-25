namespace Sample.Data.Entities
{
    public class Role : BaseEntity<int>
    {
        public Role() : base()
        {
            Users = new HashSet<User>();
            Permissions = new HashSet<Permission>();
        }

        public Role(string description, int id) : base(id)
        {
            Description = description;
            Users = new HashSet<User>();
            Permissions = new HashSet<Permission>();
        }

        public string Description { get; private set; }
        public ICollection<User> Users { get; private set; }
        public ICollection<Permission> Permissions { get; private set; }
    }
}