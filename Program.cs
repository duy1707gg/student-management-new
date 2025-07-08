var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5000", "http://localhost:81");

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 👇 Cấu hình chạy trong virtual directory
app.UsePathBase("/student-management-new");
app.Use(async (context, next) =>
{
    context.Request.PathBase = "/student-management-new";
    await next();
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();  // ✅ Load CSS/JS
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
