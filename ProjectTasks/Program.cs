using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using ProjectTasks.Infrastracture;
using ProjectTasks.Interfaces;
using ProjectTasks.Middleware;
using ProjectTasks.Repository;
using ProjectTasks.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();
//var connectionString = builder.Configuration.GetConnectionString("ProjectDb");
var keyVaultEndpoint = builder.Configuration.GetSection("KeyVault:BaseUrl").Value;
var clientId = builder.Configuration.GetSection("AzureAd:ClientId").Value;
var clientSecret = builder.Configuration.GetSection("AzureAd:ClientSecret").Value;
var tenantId = builder.Configuration.GetSection("AzureAd:TenantId").Value;
var secretClient = new SecretClient(new Uri(keyVaultEndpoint), new ClientSecretCredential(tenantId, clientId, clientSecret));
var connectionString = secretClient.GetSecret("projectDbConnectionString").Value.Value;
var signatureKey = secretClient.GetSecret("secretKeyForToken").Value.Value;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin.Admin"));
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IProjectsService,ProjectsService>();
builder.Services.AddScoped<ITasksService, TasksService>();
builder.Services.AddScoped<IProjectTaskRepository, ProjectTaskRepository>();
//builder.Services.AddScoped<IFilesService, FilesServiceLocal>();
builder.Services.AddScoped<IFilesService, FilesServiceAzure>();
builder.Services.AddScoped<IRabbitMQMessagingService, RabbitMQMessagingService>();
builder.Services.AddScoped<IReportGeneratorService, ReportGeneratorService>();

builder.Services.AddDbContext<DatabaseContext>(options => { options.UseSqlServer(connectionString); });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    IdentityModelEventSource.ShowPII = true;
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
