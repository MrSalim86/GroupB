using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using TaskManager.Data;
using TaskManager.Model;

namespace TaskManager.Services
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
        public TaskModel GetTaskById(int id)
        {
           
            return _context.Tasks.FirstOrDefault(t => t.Id == id);
        }

        public Boolean CheckingValidInputs(TaskModel task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task), "Task cannot be null.");
            }
               

            switch (task)
            {
                case TaskModel t when string.IsNullOrEmpty(t.Title):
                    Console.WriteLine("Title is required.");
                    return false;

                case TaskModel t when t.Deadline == default:
                    Console.WriteLine("Deadline is required.");
                    return false;

                case TaskModel t when t.CategoryId <= 0:
                    Console.WriteLine("Invalid CategoryId.");
                    return false;

                case TaskModel t when string.IsNullOrEmpty(t.Description):
                    Console.WriteLine("Description is either null or empty.");
                    break; // Description is optional, so not returning false

            }

            Console.WriteLine("All required inputs are valid.");
            return true;

        }
        

        // Add a new task
        public void AddTask(TaskModel task)
        {
            //if (CheckingValidInputs(task))
            //{
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

          //  }

            //return task;
         
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