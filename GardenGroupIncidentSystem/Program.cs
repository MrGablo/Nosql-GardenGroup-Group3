using MongoDB.Driver;
using GardenGroupIncidentSystem.Services;
using GardenGroupIncidentSystem.Services.Repositories;
using GardenGroupIncidentSystem.Services.Sorting;

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

builder.Services.AddControllersWithViews();
//creating session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// Register MongoClient as SINGLETON
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var conn = builder.Configuration["MongoDB:ConnectionString"];
    if (string.IsNullOrWhiteSpace(conn))
        throw new InvalidOperationException("MongoDB:ConnectionString not configured");

    return new MongoClient(conn);
});

// Register IMongoDatabase as SCOPED
builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var dbName = builder.Configuration["MongoDB:DatabaseName"];
    if (string.IsNullOrWhiteSpace(dbName))
        throw new InvalidOperationException("MongoDB:DatabaseName not configured");

    return client.GetDatabase(dbName);
});

// Register Repository (Data Access Layer)
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();


// Register Service (Business Logic Layer)
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ITicketSorter, TicketSorter>();   

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession(); // Enable session middleware

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();