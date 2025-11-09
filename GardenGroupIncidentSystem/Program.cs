using MongoDB.Driver;
using GardenGroupIncidentSystem.Services;
using GardenGroupIncidentSystem.Services.Repositories;

// Load .env file FIRST
try
{
    DotNetEnv.Env.TraversePath().Load();
}
catch
{
    // If .env not found, connection string should be in appsettings.json
}

var builder = WebApplication.CreateBuilder(args);

// Add MVC views
builder.Services.AddControllersWithViews();

// Session Configuration
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register MongoDB Client
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var conn = builder.Configuration["MongoDB:ConnectionString"];
    if (string.IsNullOrWhiteSpace(conn))
        throw new InvalidOperationException("MongoDB:ConnectionString not configured");

    return new MongoClient(conn);
});

// Register MongoDB Database
builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var dbName = builder.Configuration["MongoDB:DatabaseName"];
    if (string.IsNullOrWhiteSpace(dbName))
        throw new InvalidOperationException("MongoDB:DatabaseName not configured");

    return client.GetDatabase(dbName);
});

// Repositories
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();

// Services
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<AuthenticationService>(); // ? Inject auth service

var app = builder.Build();

// Error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Session comes before Authorization
app.UseSession();
app.UseAuthorization();

// Set default route to Login
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
