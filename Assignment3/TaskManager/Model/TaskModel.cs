using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskManager.Model
{
    public class TaskModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        public bool IsCompleted { get; set; }

        // Foreign key for Category
        [Required]
        public int CategoryId { get; set; }

        // Ignore during serialization to avoid validation issues
       
    }
}