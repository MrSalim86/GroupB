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
            List<TaskModel> tasks = new List<TaskModel>();

            // Arrange
            // Add a valid category to the in-memory database
            var category = new Category { Id = 1, Name = "Work" };
            _context.Categories.Add(category);
            _context.SaveChanges();

            // Now create a task with a valid CategoryId
            var task = new TaskModel { Title = "Integration Test Task", CategoryId = category.Id, Deadline = DateTime.Now, Description = "Test Description" };

            // Act
            tasks.Add(task);
            var result = _taskService.GetTaskById(task.Id, tasks);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Integration Test Task", result.Title);
        }

        // Integration Test 2: Ensure DeleteTask removes task
        [Fact]
        public void DeleteTask_Should_RemoveTaskFromDatabase()
        {
            List<TaskModel> tasks = new List<TaskModel>();
            // Arrange
            // Add a valid category to the in-memory database
            var category = new Category { Id = 20, Name = "Work" };
            _context.Categories.Add(category);
            _context.SaveChanges();

            // Now create a task with a valid CategoryId
            var task = new TaskModel { Title = "Task to be deleted", CategoryId = category.Id, Deadline = DateTime.Now, Description = "Test Description" };


            // Act
            tasks.Add(task);
            _taskService.DeleteTask(task.Id);
            var result = _taskService.GetTaskById(task.Id, tasks);

            // Assert
            Assert.Null(result);
        }
    }
}
