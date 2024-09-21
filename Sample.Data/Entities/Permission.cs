namespace Sample.Data.Entities
{
    public class Permission : BaseEntity<int>
    {
        public Permission() : base()
        {
            Roles = new HashSet<Role>();
        }

        public string Description { get; set; }
        public ICollection<Role> Roles { get; set; }
    }
}