var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(
    "Configuration/GRMPBD.json",
    optional: false,
    reloadOnChange: true
);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Login}/{action=Login}/{id?}");

// Indo direto pro mapa para facilitar o desenvolvimento

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Mapa}/{action=Index}");

app.Run();