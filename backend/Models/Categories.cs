using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models
{
    public class Categories
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string category_id {  get; set; }

        [Required]
        public string category_name { get; set; }
        
        [JsonIgnore]
        public ICollection<Articles> Articles { get; set; }



    }
}
