using Microsoft.EntityFrameworkCore;
using PokemonWebApi.Data;
using PokemonWebApi.Repositories;
using PokemonWebApi.Services;
using AutoMapper;
using PokemonWebApi.Mappings;
using System.Reflection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddSingleton<HttpClient>();

// Add controllers
builder.Services.AddControllers();

// Database Context Dependency Injection
var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");
var connectionString = $"Data Source={dbHost};Initial Catalog={dbName};User ID=sa;Password={dbPassword};Encrypt=True;TrustServerCertificate=True;";
builder.Services.AddDbContext<PokemonDbContext>(opt => opt.UseSqlServer(connectionString));

// Registering repositories and context
builder.Services.AddScoped<IPokemonRepository, PokemonRepository>();
builder.Services.AddScoped<IPokemonDbContext, PokemonDbContext>();

// Registering services
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IPokemonService, PokemonService>();
builder.Services.AddScoped<IMoveService, MoveService>();

builder.Services.AddHttpClient<IPokeApiService, PokeApiService>();

// Registering AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "PokemonTeamBuilder API",
        Description = "An ASP.NET Core Web API for managing Pokemonteams",
        
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
