using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sample.Data.Entities
{
    public abstract class BaseEntity<TKey>
    {
        public BaseEntity() => CreatedAt = DateTime.UtcNow;

        public BaseEntity(TKey id)
        {
            Id = id;
            CreatedAt = DateTime.UtcNow;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TKey Id { get; protected set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public bool Deleted => DeletedAt is not null;

        public void Delete() => DeletedAt = DateTime.UtcNow;

        public void Update() => UpdatedAt = DateTime.UtcNow;
    }
}