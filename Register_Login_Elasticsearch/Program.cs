using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Register_Login_Elasticsearch.Extensions;
using Microsoft.Extensions.Configuration;
using Register_Login_Elasticsearch.Repositories;
using System.Text;
using TokenHandler = Register_Login_Elasticsearch.Security.TokenHandler;
using Register_Login_Elasticsearch.AutoMapper;
using Register_Login_Elasticsearch.Services;
using Register_Login_Elasticsearch.Services.Contracts;
using Register_Login_Elasticsearch.Security;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddElastic(builder.Configuration);

builder.Services.AddAutoMapper(typeof(MapperProfile).Assembly);
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<Verification_Code>();
builder.Services.AddTransient<IEmailSender,EmailSender>();
builder.Services.AddMemoryCache();

ConfigureLogs();
builder.Host.UseSerilog();

builder.Services.AddDbContext<RepositoryContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection")));

//JWT*************************
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer
                (options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["AppSettings:Issuer"],
                        ValidAudience = builder.Configuration["AppSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:SecurityKey"]!)),
                        ClockSkew = TimeSpan.Zero
                    };

                });
builder.Services.AddSingleton<TokenHandler>();

//JWT*************************


var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

#region helper
void ConfigureLogs()
{
    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

    var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

    Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .Enrich.WithExceptionDetails()
        .WriteTo.Debug()
        .WriteTo.Console()
        .WriteTo.Elasticsearch(ConfigurationELS(configuration, env))
        .CreateLogger();
}

ElasticsearchSinkOptions ConfigurationELS(IConfiguration configuration, string? env)
{
    return new ElasticsearchSinkOptions(new Uri(configuration["Elastic:Uri"]!))
    {
        AutoRegisterTemplate = true,
        IndexFormat = $"users-{env?.ToLower().Replace(".","-")}-{DateTime.UtcNow:yyyy-MM}"
    };
}

#endregion
