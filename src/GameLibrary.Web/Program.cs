using GameLibrary.Data;
using GameLibrary.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration.GetConnectionString("GameLibrary")
    ?? throw new InvalidOperationException("Missing GameLibrary connection string.");
builder.Services.AddSingleton(new SqlConnectionFactory(connectionString));
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IFranchiseRepository, FranchiseRepository>();
builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.MapGet("/setup-status", () => Results.Text("Project 4 host is running. Build your MVC controllers and views next. This does not check the database."));
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
