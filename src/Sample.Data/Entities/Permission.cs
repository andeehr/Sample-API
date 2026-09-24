namespace Sample.Data.Entities
{
    public class Permission : BaseEntity<int>
    {
        public Permission() : base()
        {
            Roles = new HashSet<Role>();
        }

        public Permission(string description, int id) : base(id)
        {
            Description = description;
            Roles = new HashSet<Role>();
        }

        public string Description { get; private set; }
        public ICollection<Role> Roles { get; private set; }
    }
}