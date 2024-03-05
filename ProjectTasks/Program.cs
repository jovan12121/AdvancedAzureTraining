using Microsoft.EntityFrameworkCore;
using ProjectTasks.Infrastracture;
using ProjectTasks.Interfaces;
using ProjectTasks.Repository;
using ProjectTasks.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();
var connectionString = builder.Configuration.GetConnectionString("ProjectDb");
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IProjectsService,ProjectsService>();
builder.Services.AddScoped<ITasksService, TasksService>();
builder.Services.AddScoped<IProjectTaskRepository, ProjectTaskRepository>();
builder.Services.AddDbContext<DatabaseContext>(options => { options.UseSqlServer(connectionString); });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
