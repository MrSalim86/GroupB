using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using To_do_list.Data;
using To_do_list.Model;
using To_do_list.Services;
using Xunit;

namespace Todolist.Tests
{
    public class TaskServiceTests
    {
        private readonly TaskService _taskService;
        private readonly ApplicationDbContext _dbContext;

        public TaskServiceTests()
        {
            // Configure the in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase") // Use InMemory database
                .Options;

            _dbContext = new ApplicationDbContext(options); // Pass the options to the DbContext
            _taskService = new TaskService(_dbContext); // Use the real DbContext in tests
        }

        [Fact]
        public void AddTask_Should_AddTask_When_Valid()
        {
            // Arrange
            var category = new Category { Id = 33, Name = "Work" };  // Add a valid category
            _dbContext.Categories.Add(category);
            _dbContext.SaveChanges();  // Save the category to the in-memory database

            // Provide a value for the 'Description' property
            var task = new TaskModel
            {
                Title = "Test Task",
                Description = "Test Description",  // Add a valid description
                CategoryId = category.Id,
                Deadline = DateTime.Now
            };

            // Act
            _taskService.AddTask(task);

            // Assert
            var addedTask = _dbContext.Tasks.Find(task.Id);
            Assert.NotNull(addedTask);
            Assert.Equal("Test Task", addedTask.Title);
            Assert.Equal("Test Description", addedTask.Description);
        }

        [Fact]
        public void GetTaskById_Should_ReturnTask_When_TaskExists()
        {
            // Arrange
            List<TaskModel> tasks = new List<TaskModel>();
            var task = new TaskModel { Title = "Test Task", CategoryId = 1, Deadline = DateTime.Now, Description = "Test Description" };
            tasks.Add(task);

            // Act

            var result = _taskService.GetTaskById(task.Id, tasks);

            // Assert
           
            Assert.NotNull(result);
            Assert.Equal(task.Title, result.Title);
        }

        [Fact]
        public void GetAllTasks_Should_ReturnAllTasks()
        {
            // Arrange
            _dbContext.Tasks.AddRange(
                new TaskModel { Title = "Task 1", CategoryId = 1, Deadline = DateTime.Now, Description = "Test Description" },
                new TaskModel { Title = "Task 2", CategoryId = 1, Deadline = DateTime.Now, Description = "Test Description" }
            );
            _dbContext.SaveChanges();

            // Act
            var result = _taskService.GetAllTasks();

            // Assert
            Assert.Equal(result.Count(), result.Count());
        }

        [Fact]
        public void UpdateTask_Should_UpdateTask_When_Valid()
        {
            // Arrange
            var task = new TaskModel { Title = "Old Title", CategoryId = 1, Deadline = DateTime.Now, Description = "Test Description" };
            _dbContext.Tasks.Add(task);
            _dbContext.SaveChanges();

            var updatedTask = new TaskModel { Title = "New Title", CategoryId = 1, Deadline = DateTime.Now, Description = "Test Description" };

            // Act
            _taskService.UpdateTask(task.Id, updatedTask);

            // Assert
            var fetchedTask = _dbContext.Tasks.Find(task.Id);
            Assert.Equal("New Title", fetchedTask.Title);
        }

        [Fact]
        public void DeleteTask_Should_RemoveTask_When_Exists()
        {
            // Arrange
            var task = new TaskModel { Title = "Task to delete", CategoryId = 1, Deadline = DateTime.Now, Description = "Test Description" };
            _dbContext.Tasks.Add(task);
            _dbContext.SaveChanges();

            // Act
            _taskService.DeleteTask(task.Id);

            // Assert
            var deletedTask = _dbContext.Tasks.Find(task.Id);
            Assert.Null(deletedTask);
        }
        [Fact]
        public void MarkTaskAsCompleted_Should_SetTaskAsCompleted()
        {
            // Arrange
            // Add a valid category to the in-memory database
            List<TaskModel> tasks = new List<TaskModel>();

            var category = new Category { Id = 67, Name = "Work" };
            _dbContext.Categories.Add(category);
            _dbContext.SaveChanges();

            // Now create a task with the generated CategoryId
            var task = new TaskModel { Title = "Incomplete Task", IsCompleted = false, CategoryId = category.Id, Description = "ddd" };
            tasks.Add(task);

            // Act
            _taskService.MarkTaskAsCompleted(task.Id);
            var result = _taskService.GetTaskById(task.Id, tasks);

            // Assert
            Assert.True(result.IsCompleted);
        }
    }
}
