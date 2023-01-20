var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.AddAuthentication("UserSession").AddCookie("UserSession", config =>
{
    config.Cookie.Name = "UserSession";
    config.LoginPath = "/Account/Login";
    config.SlidingExpiration = true;
    config.ExpireTimeSpan = new TimeSpan(1, 0, 0, 0, 0);//1 day
    config.Cookie.MaxAge = config.ExpireTimeSpan;
});

//Load configuration settings into memory when the app starts (faster)
Ecommerce.Configuration.configuration = builder.Configuration;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
