var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocument();

var app = builder.Build();

app.UseOpenApi();
app.UseSwaggerUI();

app.MapGet("tasks", () =>
{
    return Results.Ok(new int[] { 2, 3, 5, 7, 11, 13, 17, 19 });
});

app.Run();