using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using To_do_list.Data;
using To_do_list.Model;
using To_do_list.Services;

namespace To_do_list.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly TaskService _taskService;
        private readonly ApplicationDbContext _context;

     

        public TasksController(TaskService taskService, ApplicationDbContext context)
        {
            _taskService = taskService;
            _context = context;
        }

        // Get all tasks
        [HttpGet]
        public ActionResult<IEnumerable<TaskModel>> GetTasks()
        {
            return Ok(_taskService.GetAllTasks());
        }

        // Get a specific task by id
        [HttpGet("{id}")]
        public ActionResult<TaskModel> GetTask(int id)
        {
           
        var task = _taskService.GetTaskById(id, _context.Tasks.ToList());
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        // Add a new task
        [HttpPost]
        public ActionResult<TaskModel> AddTask([FromBody] TaskModel newTask)
        {
            var category = _taskService.GetCategoryById(newTask.CategoryId);
            if (category == null)
            {
                return BadRequest(new { message = "Invalid CategoryId. Category not found." });
            }

            _taskService.AddTask(newTask);
            return CreatedAtAction(nameof(GetTask), new { id = newTask.Id }, newTask);
        }

        // Update a task
        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, [FromBody] TaskModel updatedTask)
        {
            _taskService.UpdateTask(id, updatedTask);
            return NoContent();
        }

        // Mark a task as completed
        [HttpPatch("{id}/complete")]
        public IActionResult MarkTaskAsCompleted(int id)
        {
            _taskService.MarkTaskAsCompleted(id);
            return NoContent();
        }

        // Delete a task
        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            _taskService.DeleteTask(id);
            return NoContent();
        }

        // Get all categories
        [HttpGet("categories")]
        public ActionResult<IEnumerable<Category>> GetCategories()
        {
            return Ok(_taskService.GetAllCategories());
        }

        // Add a new category
        [HttpPost("categories")]
        public ActionResult<Category> AddCategory([FromBody] Category category)
        {
            _taskService.AddCategory(category);
            return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, category);
        }
    }
}