using ProjectTaskCosmos.Interfaces;
using ProjectTaskCosmos.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IProjectsService, ProjectsService>();
builder.Services.AddScoped<ITasksService, TasksService>();
builder.Services.AddScoped<IFilesService, AzureFilesService>();
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
