using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string user_id { get; set; }
        [Required]
        public string username { get; set; } = string.Empty;
        [Required]
        public string email { get; set; } = string.Empty;
        [Required]
        public string password { get; set; } = string.Empty;
        [Required]
        public string role { get; set; } = string.Empty;
        [Required]
        public  DateTime created_at { get; set; } =DateTime.Now;
        public virtual ICollection<Comments> Comments { get; set; }

        public ICollection<Likes> Likes { get; set; }


    }
}
