using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace To_do_list.Model
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        // Navigation property for related tasks
        [JsonIgnore]
        public List<TaskModel> Tasks { get; set; }
    }
}