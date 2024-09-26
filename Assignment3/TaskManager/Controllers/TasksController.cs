using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TaskManager.Data;
using TaskManager.Model;
using TaskManager.Services;

namespace TaskManager.Controllers
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
           
        var task = _taskService.GetTaskById(id);
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

            
           
            if (_taskService.AddTask(newTask) == "OK") 
            {
                return CreatedAtAction(nameof(GetTask), new { id = newTask.Id }, newTask); 
            }
            return BadRequest(new { message = _taskService.AddTask(newTask) });
        }

        // Update a task
        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, [FromBody] TaskModel updatedTask)
        {
            _taskService.UpdateTask(id, updatedTask);
            return Ok(_taskService.GetTaskById(id));

        }

        // Mark a task as completed
        [HttpPatch("{id}/complete")]
        public IActionResult MarkTaskAsCompleted(int id)
        {
            if (_taskService.GetTaskById(id) != null)
            {

                _taskService.MarkTaskAsCompleted(id);
                return Ok();
            }

            else
            {
                return NotFound();
            }
            
            
            
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