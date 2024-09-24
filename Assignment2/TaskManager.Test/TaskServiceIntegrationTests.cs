using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Data;
using TaskManager.Model;
using TaskManager.Services;
using Xunit;

namespace TaskManager.Tests

{
    public class TaskServiceIntegrationTests
    {

        private readonly TaskService _taskService;
        private readonly ApplicationDbContext _context;

        public TaskServiceIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
            _taskService = new TaskService(_context);
        }

        // Integration Test 1: Add and retrieve task
        [Fact]
        public void AddTask_And_GetTask_Should_WorkCorrectly()
        {
         

            // Arrange
            // Add a valid category to the in-memory database
            var category = new Category { Name = "Work" };
            _taskService.AddCategory(category);

            // Now create a task with a valid CategoryId
            var task = new TaskModel { Title = "Integration Test Task", CategoryId = category.Id, Deadline = DateTime.Now, Description = "Test Description" };

            // Act
            _taskService.AddTask(task);
            var result = _taskService.GetTaskById(task.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Integration Test Task", result.Title);
        }

        // Integration Test 2: Ensure DeleteTask removes task
        [Fact]
        public void DeleteTask_Should_RemoveTaskFromDatabase()
        {
           
            // Arrange
            // Add a valid category to the in-memory database
            var category = new Category { Id = 20, Name = "Work" };
           _taskService.AddCategory(category);

            // Now create a task with a valid CategoryId
            var task = new TaskModel { Title = "Task to be deleted", CategoryId = category.Id, Deadline = DateTime.Now, Description = "Test Description" };


            // Act
            _taskService.AddTask(task);
            _taskService.DeleteTask(task.Id);
            var result = _taskService.GetTaskById(task.Id);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void AddCategory_And_GetCategoryById_Should_WorkCorrectly()
        {
            var category = new Category { Name = "Personal" };
            _taskService.AddCategory(category);

            var result = _taskService.GetCategoryById(category.Id);
            Assert.NotNull(result);
            Assert.Equal("Personal", result.Name);
        }

        [Fact]
        public void UpdateTask_Should_UpdateTaskCorrectly()
        {
            var category = new Category { Name = "Work" };
            _taskService.AddCategory(category);

            var AddTask = new TaskModel { Title = "Old Title", CategoryId = category.Id, Deadline = DateTime.Now, Description = "Old Description" };
            //add
            _taskService.AddTask(AddTask);
            
            //update
            var updatedTask = new TaskModel { Title = "New Title", CategoryId = category.Id, Deadline = DateTime.Now.AddDays(1), Description = "New Description" };
            _taskService.UpdateTask(AddTask.Id, updatedTask);

            var result = _taskService.GetTaskById(AddTask.Id);
            Assert.Equal("New Title", result.Title);
            Assert.Equal("New Description", result.Description);
        }

        [Fact]
        public void DeleteTask_Should_Not_Throw_When_TaskDoesNotExist()
        {
            // Act & Assert
            var exception = Record.Exception(() => _taskService.DeleteTask(999));
            Assert.Null(exception); // Should not throw
        }

        [Fact]
        public void MarkTaskAsCompleted_Should_NotChangeTask_When_AlreadyCompleted()
        {
            var category = new Category { Name = "Work" };
            _taskService.AddCategory(category);

            var task = new TaskModel { Title = "Complete Task", IsCompleted = true, CategoryId = category.Id, Description = "Completed Task" };
            _taskService.AddTask(task);

            _taskService.MarkTaskAsCompleted(task.Id);
            var result = _taskService.GetTaskById(task.Id);

            Assert.True(result.IsCompleted);
        }



    }
}
