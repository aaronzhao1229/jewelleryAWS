using API.Data;
using API.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. Orders are not important.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<StoreContext>(opt => 
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddCors(); // add CORS service. We also need to add some middleware because we're going to modify the request on its way out as we need to add that CORS header that goes along with goes along with our request.

var app = builder.Build();

// Configure the HTTP request pipeline. Orders are important.

// we are going to create our own exception handlding middleware and return it to the client in a format that's easier to work with on the client as well.

//exception middleware needs to be on the top of the middleware tree just in case any of our middleware throws an exception
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(opt => 
{
    opt.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:3000"); // add AllowCredentials() to allow the cookies to be passed from the client
});

app.UseAuthorization();

app.MapControllers();


// this part is to create a database and seed the data when we run the code
var scope = app.Services.CreateScope();
// The idea behind this is we need to get hold of our DBContext service by creating a scope and store it inside the scope variable. From there we can create a variable to store our context in.
// get hold of the StoreContext
var context = scope.ServiceProvider.GetRequiredService<StoreContext>();

// we also want to get hold of a logger so that we can log any error that we get. The type we specify in the ILogger is the class that we're executing (Program)
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

// Now we need to execute some code against our context. This is the code that could go wrong and it could generate an exception if we haven't got all of our ducks lined up as we need to. So we do this in try catch block
try
{
    context.Database.Migrate(); //Applies any pending migrations for the context to the database. Will create the database if it does not already exist.
    DbInitializer.Initialize(context);
}
catch (Exception ex)
{
    logger.LogError(ex, "A problem occurred during migration");
}


app.Run();
