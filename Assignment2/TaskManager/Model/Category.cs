using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TaskManager.Model
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        //// Navigation property for related tasks
        //[JsonIgnore]
        //[NotMapped]
        //public List<TaskModel> Tasks { get; set; }
    }
}