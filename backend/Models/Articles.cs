using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Articles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string articles_id { get; set; }

        [Required(ErrorMessage = "Bạn phải nhập Title")]
        public string title { get; set; }

        [Required]
        public string content { get; set; } = string.Empty;

        [Required]
        public string category_id { get; set; } = string.Empty;
        public virtual Categories category { get; set; }

        public int view { get; set; }
        public DateTime createAt { get; set; } = DateTime.UtcNow;

        [Required]
        public string status { get; set; } = string.Empty;

        public virtual ICollection<Likes> Likes { get; set; }
        public virtual ICollection<Comments> Comments { get; set; }
    }
}
