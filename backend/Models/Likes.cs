using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Likes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string like_id { get; set; }
        [Required]
        public string articles_id { get; set; } = string.Empty;
        public virtual Articles Article { get; set; }
        [Required]
        public string user_id { get; set; } = string.Empty;
        public virtual User user { get; set; }

        public virtual ICollection<Likes> Like { get; set; }



    }
}
