using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using To_do_list.Data;
using To_do_list.Model;

namespace To_do_list.Services
{
    public class TaskService
    {
        private readonly ApplicationDbContext _context;

        public TaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all tasks
        public IEnumerable<TaskModel> GetAllTasks()
        {
            return _context.Tasks.ToList();
        }

        // Get task by id
        public TaskModel GetTaskById(int id, List<TaskModel> listOftask)
        {
            return listOftask.FirstOrDefault(t => t.Id == id);
        }

        // Add a new task
        public void AddTask(TaskModel task)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == task.CategoryId);
            if (category != null)
            {
               
                _context.Tasks.Add(task);
                _context.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Invalid CategoryId.");
            }
        }

        // Update an existing task
        public void UpdateTask(int id, TaskModel updatedTask)
        {
            var existingTask = _context.Tasks.FirstOrDefault(t => t.Id == id);
            if (existingTask != null)
            {
                existingTask.Title = updatedTask.Title;
                existingTask.Description = updatedTask.Description;
                existingTask.Deadline = updatedTask.Deadline;
                existingTask.IsCompleted = updatedTask.IsCompleted;
                existingTask.CategoryId = updatedTask.CategoryId;

                _context.Tasks.Update(existingTask);
                _context.SaveChanges();
            }
        }

        // Mark a task as completed
        public void MarkTaskAsCompleted(int id)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                task.IsCompleted = true;
                _context.SaveChanges();
            }
        }

        // Delete a task
        public void DeleteTask(int id)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                _context.SaveChanges();
            }
        }

        // Get all categories
        public IEnumerable<Category> GetAllCategories()
        {
            return _context.Categories.ToList();
        }

        // Add a new category
        public void AddCategory(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        // Add this method to TaskService
        public Category GetCategoryById(int categoryId)
        {
            return _context.Categories.FirstOrDefault(c => c.Id == categoryId);
        }
    }
}