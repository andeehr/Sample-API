using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sample.Data.Entities
{
    public abstract class BaseEntity<TKey>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TKey Id { get; protected set; }

        public DateTime CreatedAt { get; protected set; }
        public DateTime? UpdatedAt { get; protected set; }
        public DateTime? DeletedAt { get; protected set; }
        public bool Deleted => DeletedAt is not null;

        public BaseEntity() => CreatedAt = DateTime.UtcNow;

        public void Delete() => DeletedAt = DateTime.UtcNow;

        public void Update() => UpdatedAt = DateTime.UtcNow;
    }
}