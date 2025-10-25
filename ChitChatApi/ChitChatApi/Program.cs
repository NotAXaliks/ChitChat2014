using ChitChatApi.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendDev", policy =>
    {
        policy.SetIsOriginAllowed(origin => true) 
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); 
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Временная миграция паролей
    var users = db.Employees.ToList();
    foreach (var u in users)
    {
        if (!u.Password.StartsWith("$2"))
            u.Password = BCrypt.Net.BCrypt.HashPassword(u.Password);
    }
    db.SaveChanges();

    Console.WriteLine("Хэширование паролей завершено.");
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontendDev");
app.UseRouting();


app.MapControllers();
app.Run();
