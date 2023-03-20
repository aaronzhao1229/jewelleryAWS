using System.Text;
using API.Data;
using API.Entities;
using API.Middleware;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. Orders are not important.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put Bearer + your token in the box below",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            jwtSecurityScheme, Array.Empty<string>()
        }
    });
}
);


builder.Services.AddDbContext<StoreContext>(opt => 
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddCors(); // add CORS service. We also need to add some middleware because we're going to modify the request on its way out as we need to add that CORS header that goes along with goes along with our request.

// Identity

builder.Services.AddIdentityCore<User>(opt => 
{
     opt.User.RequireUniqueEmail = true;
}
) // We could have used IdentityUser here, but later on we will need more properties than those IdentityUser gives us and we have derive from IdentityUser class in order to add our own properties to the User class. Therefore, we use User here.
    .AddRoles<Role>()
    .AddEntityFrameworkStores<StoreContext>(); // this will give us six tables (a table for uses, a table for roles and other tables) in our database of identity information stores. 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt => 
      {
          opt.TokenValidationParameters = new TokenValidationParameters
          {
              ValidateIssuer = false,
              ValidateAudience = false,
              ValidateLifetime = true,
              ValidateIssuerSigningKey = true, //this is the one where our server checks for the validity of the token based on the signature that it used when creating the token.
              IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTSettings:TokenKey"]))
              // we need to specify the issue assigning key so that our API nows which key was used to sign it. 
          };
      }
      );
// this is how we're going to use our token. Our users are going to present the token inside effectively and authorization header in the HTTP request, and then our server is going to check that for its validity. 
builder.Services.AddAuthorization();
// Three ways to add our own service that we're asking the framework really to provide for us to other classes in our application when dependency injection is used.
// AddScoped(), when we talk about a scope, we're talking about the scope of the request. In the case of an API controller, this scope is valid for the lifetime of the request. So an HTTP request that comes into our API project or service is going to be the scope of the request. Request comes in, the logic is applied and the response goes out. The entire request is what we're talking about when we say AddScoped.
// AddTransient(), transient is a very short lived service. It's created and disposed of as it's used inside our classes. It's not kept alive for the length of the request. Therefore, it's generally not used for an API request.
// AddSingleton(), this is kept alive for a lifetime of our application. So it's not disposed of at all.  When we talk about dependency injection, what we are asking the framework to do and take care of for us is as we use these services in our various classes, we're asking our framework to create a new instance of the service and dispose of that instance once we're done with it. 
// Therefore, the most common way of using these services would be to scope it to the HTTP request.
builder.Services.AddScoped<TokenService>();
// Identity


var app = builder.Build();

// Configure the HTTP request pipeline. Orders are important.

// we are going to create our own exception handlding middleware and return it to the client in a format that's easier to work with on the client as well.

//exception middleware needs to be on the top of the middleware tree just in case any of our middleware throws an exception
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
       c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
       // keep the toke persisted if we refresh our browser
    });
}

app.UseCors(opt => 
{
    opt.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:3000"); // add AllowCredentials() to allow the cookies to be passed from the client
});

app.UseAuthentication(); // this needs to be before UseAuthorization()
app.UseAuthorization();

app.MapControllers();


// this part is to create a database and seed the data when we run the code
var scope = app.Services.CreateScope();
// The idea behind this is we need to get hold of our DBContext service by creating a scope and store it inside the scope variable. From there we can create a variable to store our context in.
// get hold of the StoreContext
var context = scope.ServiceProvider.GetRequiredService<StoreContext>();

// we also want to get hold of a logger so that we can log any error that we get. The type we specify in the ILogger is the class that we're executing (Program)
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

// Now we need to execute some code against our context. This is the code that could go wrong and it could generate an exception if we haven't got all of our ducks lined up as we need to. So we do this in try catch block
try
{
    await context.Database.MigrateAsync(); //Applies any pending migrations for the context to the database. Will create the database if it does not already exist.
    await DbInitializer.Initialize(context, userManager);
}
catch (Exception ex)
{
    logger.LogError(ex, "A problem occurred during migration");
}


app.Run();
