//using TaskModel = To_do_list.Model.Task; // Alias to avoid conflict with System.Threading.Tasks.Task
//using To_do_list.Services;
//namespace To_do_list
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);

//            // Register TaskService as a singleton
//            builder.Services.AddSingleton<TaskService>();

//            // Add services to the container
//            builder.Services.AddControllers();
//            builder.Services.AddEndpointsApiExplorer();
//            builder.Services.AddSwaggerGen();

//            var app = builder.Build();

//            // Get TaskService from the DI container


//            // Configure the HTTP request pipeline
//            if (app.Environment.IsDevelopment())
//            {
//                app.UseSwagger();
//                app.UseSwaggerUI();
//            }

//            app.UseHttpsRedirection();
//            app.UseAuthorization();
//            app.MapControllers();
//            app.Run();
//        }
//    }
//}

using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Model;
using TaskManager.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure the DbContext to use an in-memory database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("InMemoryDb"));
builder.Services.AddScoped<TaskService>();
builder.Services.AddSwaggerGen();
// Build the app
var app = builder.Build();

// Optionally seed the in-memory database with some initial data



// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

//Seed the database with dummy data 
using (var scope = app.Services.CreateScope())
{ 
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();         
    // Add dummy categories
    if (!context.Categories.Any())     
    {        
        var workCategory = new Category { Name = "Work" };        
        var personalCategory = new Category { Name = "Personal" };       
        context.Categories.AddRange(workCategory, personalCategory);     
        context.SaveChanges();                  
            
        
    // Add dummy tasks linked to categories
    context.Tasks.AddRange(new TaskModel
    {
    Title = "Finish Project",
    Description = "Complete the to-do list project",
    Deadline = DateTime.Now.AddDays(2),
    IsCompleted = false,
    CategoryId = workCategory.Id
    }, new TaskModel    
    {
    Title = "Buy Groceries",
    Description = "Buy fruits, vegetables, and milk",
    Deadline = DateTime.Now.AddDays(1),
    IsCompleted = false,
    CategoryId = personalCategory.Id
    },
    new TaskModel
    {
        Title = "Send Emails",
        Description = "Send the project update emails to the team",
        Deadline = DateTime.Now.AddDays(1),
        IsCompleted = false,
        CategoryId = workCategory.Id
    });
    context.SaveChanges();     
    
 } }


app.Run();