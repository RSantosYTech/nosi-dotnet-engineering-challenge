using Microsoft.EntityFrameworkCore;
using NOS.Engineering.Challenge.API.Extensions;
using NOS.Engineering.Challenge.Data;
using NOS.Engineering.Challenge.Repository;

var builder = WebApplication.CreateBuilder(args)
        .ConfigureWebHost()
        .RegisterServices();

//Adding in Logger configuration. 
builder.Logging.ClearProviders()
    //Adding console logging support.
    .AddConsole();

//Adding in Entity Framework (SQL Server) configuration.
builder.Services.AddDbContext<ContentDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ContentTestDB")));
builder.Services.AddTransient<IContentRepository, ContentRepository>();

//Added in response caching functionality to speed up requests when new data isn't needed to be retrieved.
builder.Services.AddResponseCaching();

var app = builder.Build();

app.MapControllers();
app.UseSwagger()
    .UseSwaggerUI();

//Enable the use of the response caching added above.
app.UseResponseCaching();

app.Run();