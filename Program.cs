var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSignalR();
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("reactapp", builder =>
    {
        builder.WithOrigins(
            "http://localhost:3000",
            "https://mybetschatroomapp.netlify.app")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<SharedDb>();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Logging middleware
app.Use(async (context, next) =>
{
    Console.WriteLine("Request:");
    Console.WriteLine($"{context.Request.Method} {context.Request.Path}");
    foreach (var header in context.Request.Headers)
    {
        Console.WriteLine($"{header.Key}: {header.Value}");
    }

    await next();

    Console.WriteLine("Response:");
    foreach (var header in context.Response.Headers)
    {
        Console.WriteLine($"{header.Key}: {header.Value}");
    }
});

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("reactapp");

app.UseAuthorization();

app.MapControllers();
app.MapHub<chat_app.Hubs.ChatHub>("/chat");

app.Run();
