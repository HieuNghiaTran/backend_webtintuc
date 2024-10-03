using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Comments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public  string comment_id { get; set; }
        [Required]
        public string  content { get; set; }
        [Required]
        public string articles_id {  get; set; }
        public virtual Articles Article { get; set; }
        public DateTime createAt { get; set; }
        [Required]
        public string user_id { get; set; }
        public virtual User user { get; set; }
        public virtual ICollection<Articles> Articles { get; set; }

    }
}
