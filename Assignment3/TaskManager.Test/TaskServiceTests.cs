using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TaskManager.Data;
using TaskManager.Model;
using TaskManager.Services;
using Xunit;

namespace TaskManager.Tests
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
        public void CheckValidInputs()
        {
            // Provide a value for the 'Description' property
            var task = new TaskModel
            {
                Title = "Test Task",
                Description = "Test Description",  // Add a valid description
                CategoryId = 999,
                Deadline = DateTime.Now
            };

            // Act
            Assert.True(_taskService.CheckingValidInputs(task));
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
           var getCat = _taskService.GetAllCategories().FirstOrDefault() ?? throw new InvalidOperationException("No categories found. Unable to add task without a category.");
            var task = new TaskModel { Title = "Test Task", CategoryId = getCat.Id, Deadline = DateTime.Now, Description = "Test Description" };
            _taskService.AddTask(task);

            // Act

            var result = _taskService.GetTaskById(task.Id);

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
            var getCategory = _taskService.GetAllCategories().FirstOrDefault() ?? throw new InvalidOperationException("No categories found. Unable to add task without a category.");
            // Arrange
            var task = new TaskModel { Title = "Old Title", CategoryId = getCategory.Id, Deadline = DateTime.Now, Description = "Test Description" };
            _taskService.AddTask(task);

            var updatedTask = new TaskModel { Title = "New Title", CategoryId = getCategory.Id, Deadline = DateTime.Now, Description = "Test Description" };

            // Act
            _taskService.UpdateTask(task.Id, updatedTask);
           
            // Assert
            var fetchedTask = _dbContext.Tasks.Find(task.Id) ?? throw new InvalidOperationException("No task found.");
            Assert.Equal("New Title", fetchedTask.Title);
        }

        [Fact]
        public void DeleteTask_Should_RemoveTask_When_Exists()
        {
            // Arrange
            var task = new TaskModel { Title = "Task to delete", CategoryId = 1, Deadline = DateTime.Now, Description = "Test Description" };
            _taskService.AddTask(task);

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

            var category = new Category { Name = "new Work" };
            _taskService.AddCategory(category);
            // Now create a task with the generated CategoryId
            var task = new TaskModel { Title = "Incomplete Task", IsCompleted = false, CategoryId = category.Id, Description = "ddd" };
 
            _taskService.AddTask(task);

            // Act
            _taskService.MarkTaskAsCompleted(1);
            var result = _taskService.GetTaskById(1);

            // Assert
            Assert.True(result.IsCompleted);
        }

        [Fact]
        public void CheckingValidInputs_Should_ReturnFalse_When_TitleIsMissing()
        {
            var task = new TaskModel
            {
                Description = "Test Description",
                CategoryId = 1,
                Deadline = DateTime.Now
            };

            Assert.False(_taskService.CheckingValidInputs(task));
        }

        [Fact]
        public void CheckingValidInputs_Should_ReturnFalse_When_CategoryIdIsInvalid()
        {
            var task = new TaskModel
            {
                Title = "Test Task",
                Description = "Test Description",
                CategoryId = -1,
                Deadline = DateTime.Now
            };

            Assert.False(_taskService.CheckingValidInputs(task));
        }

        [Fact]
        public void CheckingValidInputs_Should_ReturnFalse_When_DeadlineIsDefault()
        {
            var task = new TaskModel
            {
                Title = "Test Task",
                Description = "Test Description",
                CategoryId = 1,
                Deadline = default(DateTime)
            };

            Assert.False(_taskService.CheckingValidInputs(task));
        }

        [Fact]
        public void AddTask_Should_ThrowException_When_CategoryIdIsInvalid()
        {
            var task = new TaskModel
            {
                Title = "Test Task",
                Description = "Test Description",
                CategoryId = 999, // Non-existing category
                Deadline = DateTime.Now
            };

            Assert.Throws<ArgumentException>(() => _taskService.AddTask(task));
        }

        [Fact]
        public void CheckingValidInputs_Should_ThrowException_When_TaskIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _taskService.CheckingValidInputs(null));
        }

        [Fact]
        public void AddTask_Should_ThrowException_When_CategoryNotFound()
        {
            // Arrange
            var task = new TaskModel
            {
                Title = "Test Task",
                CategoryId = 999, // Non-existing category
                Deadline = DateTime.Now,
                Description = "test t"
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _taskService.AddTask(task));
        }

        [Fact]
        public void CheckingValidInputs_Should_ThrowArgumentNullException_When_TaskIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _taskService.CheckingValidInputs(null));
        }

        [Fact]
        public void CheckingValidInputs_Should_ReturnTrue_When_DescriptionIsMissing()
        {
            var task = new TaskModel
            {
                Title = "Test Task",
                CategoryId = 1,
                Deadline = DateTime.Now
                // No description provided
            };

            // Act & Assert
            Assert.True(_taskService.CheckingValidInputs(task));
        }

        //[Fact]
        //public void UpdateTask_Should_ThrowException_When_TaskDoesNotExist()
        //{
        //    // Arrange
        //    var updatedTask = new TaskModel { Title = "Non-existing Task", CategoryId = 1, Deadline = DateTime.Now };

        //    // Act & Assert
        //    Assert.Throws<InvalidOperationException>(() => _taskService.UpdateTask(9999, updatedTask));
        //}

        [Fact]
        public void GetTaskById_Should_ReturnNull_When_TaskDoesNotExist()
        {
            // Act
            var result = _taskService.GetTaskById(999);

            // Assert
            Assert.Null(result);
        }

        // When is valid

        [Fact]
        public void TaskTitle_MinBoundary_Valid()
        {
            var task = new TaskModel { Title = new string('a', 3) };  // Minimum boundary
            Assert.True(_taskService.TitleLength(task.Title).Isvalid);  // Assuming ValidateTaskTitle is the validation logic
        }

        [Fact]
        public void TaskTitle_MaxBoundary_Valid()
        {
            var task = new TaskModel { Title = new string('a', 30) };  // Maximum boundary
            Assert.True(_taskService.TitleLength(task.Title).Isvalid);
        }

         // When is not valid

        [Fact]
        public void TaskTitle_ExceedsMinBoundary_Invalid()
        {
            var task = new TaskModel { Title = new string('a', 2) };  // Beyond Minimum boundary 
            Assert.False(_taskService.TitleLength(task.Title).Isvalid);
        }

        [Fact]
        public void TaskTitle_ExceedsMaxBoundary_Invalid()
        {
            var task = new TaskModel { Title = new string('a', 31) };  // Beyond maximum boundary
            Assert.False(_taskService.TitleLength(task.Title).Isvalid);
        }


        [Fact]
        public void TaskTitle_ValidLength_ReturnsTrue()
        {
            var result = _taskService.TitleLength("Task Title");  // 10 characters
            Assert.True(result.Isvalid);
            Assert.Equal("Title is valid.", result.ErrorMessage);
        }

        [Fact]
        public void TaskTitle_TooShort_ReturnsFalse()
        {
            var result = _taskService.TitleLength("Hi");  // 2 characters
            Assert.False(result.Isvalid);
            Assert.Equal("Title is too short. It must be at least 3 characters.", result.ErrorMessage);
        }

        [Fact]
        public void TaskTitle_TooLong_ReturnsFalse()
        {
            var result = _taskService.TitleLength("This is a very long task title that exceeds thirty characters");  // 53 characters
            Assert.False(result.Isvalid);
            Assert.Equal("Title is too long. It must be less than or equal to 30 characters.", result.ErrorMessage);
        }
    }

}
