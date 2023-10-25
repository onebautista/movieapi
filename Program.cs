using ApiPeliculas;

var builder = WebApplication.CreateBuilder(args);

//llamada clase Startup
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

// Add services to the container.
//builder.Services.AddControllers();

var app = builder.Build();

startup.Configure(app, app.Environment);


app.Run();
