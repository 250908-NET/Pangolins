using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pangolivia.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("auth_uuid")]
        [MaxLength(255)]
        public string AuthUuid { get; set; } = string.Empty;

        [Required]
        [Column("username")]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        // Timestamps for auditing (Just in Case)
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}