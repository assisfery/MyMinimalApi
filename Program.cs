using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/todo", async (TodoDb db) => {
    return await db.Todos.ToListAsync();
});

app.MapPost("/todo", async (Todo todo, TodoDb db) => {
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created("todo/" + todo.Id, todo);
});

app.MapGet("todo/{id}", async (int id, TodoDb db) =>
{
    return await db.Todos.FindAsync(id) is Todo todo ? Results.Ok(todo) : Results.NotFound();
});

app.MapDelete("todo/{id}", async (int id, TodoDb db) =>
{
    Todo? todo = await db.Todos.FindAsync(id);

    if (todo == null)
        return Results.NotFound();

    db.Todos.Remove(todo);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapPut("todo/{id}", async (int id, Todo inputTodo, TodoDb db) =>
{
    Todo? todo = await db.Todos.FindAsync(id);

    if (todo == null)
        return Results.NotFound();

    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;

    await db.SaveChangesAsync();

    return Results.Ok(todo);
});

app.Run();
