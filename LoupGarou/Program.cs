using LoupGarou.Data;
using Microsoft.EntityFrameworkCore;
using LoupGarou.Services.Interfaces;
using LoupGarou.Services;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAllowedOrigins",
        policy =>
        {
            policy.WithOrigins("*")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IVoteService, VoteServie>();

builder.Services.AddDbContext<LoupGarouDbContext>(
    o => o.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlServer"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null
        )
    )
);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger(c =>
{
    c.SerializeAsV2 = true;
});
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "LoupGarou API V1");
    c.RoutePrefix = string.Empty;
});

app.UseCors("MyAllowedOrigins");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LoupGarouDbContext>();
    db.Database.Migrate();
}

app.Run();
