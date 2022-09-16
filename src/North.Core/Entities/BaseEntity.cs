using System.ComponentModel.DataAnnotations;

namespace North.Core.Entities
{
    public class BaseEntity
    {
        [Required]
        public Guid Id { get; set; }

        public BaseEntity(Guid id)
        {
            Id = id;
        }
    }
}
