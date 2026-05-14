var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        // Caminho para a página de login
        options.LoginPath = "/Login/Login";

        // Caminho para logout
        options.LogoutPath = "/Login/Logout";

        // Opcional: define onde o usuário será redirecionado após login
        options.AccessDeniedPath = "/Login/Login";
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}")
    .WithStaticAssets();


app.Run();
