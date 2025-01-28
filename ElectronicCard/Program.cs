using ElectronicCard.Data;
using ElectronicCard.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add DbContext
builder.Services.AddDbContext<MyAppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true; // Security for session cookie
    options.Cookie.IsEssential = true; // Ensure session works under GDPR
});

// Add Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;

    // User settings
    options.User.RequireUniqueEmail = true;

    // SignIn settings
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<MyAppDbContext>()
.AddDefaultTokenProviders();

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login"; // Redirect here if the user isn't authenticated
    options.AccessDeniedPath = "/Ecard/Login"; // Redirect here if access is forbidden
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
});

// Build the app
var app = builder.Build();

// Seed roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "Chairman", "Secretary", "Treasurer" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // Enable session

// Authentication and authorization
app.UseAuthentication();
app.UseAuthorization();


// Middleware to handle unauthorized access globally
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value.ToLower();

    // Allow access to Login, Register, and other public pages without authentication
    if (!context.User.Identity.IsAuthenticated &&
        !path.StartsWith("/ecard/login", StringComparison.OrdinalIgnoreCase) &&
        !path.StartsWith("/ecard/register", StringComparison.OrdinalIgnoreCase) &&
        !path.StartsWith("/ecard/ResetPassword", StringComparison.OrdinalIgnoreCase)) // Add other public routes if necessary
    {
        // Redirect unauthenticated users to the login page
        context.Response.Redirect("/Ecard/Login");
        return;
    }

    // Allow the request to continue if the user is authenticated or the path is excluded
    await next();
});

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession(); // Enable session

// Authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map Razor Pages
app.MapRazorPages();

// Default route redirection to /Ecard/Login if no other route matches
app.MapGet("/", context =>
{
    context.Response.Redirect("/Ecard/Login", permanent: false);
    return Task.CompletedTask;
});

// Run the application
app.Run();
