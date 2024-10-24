using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SigaDocIntegracao.Web.Persistence;
using SigaDocIntegracao.Web.Tasks;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<TaskModuloEmail>();
builder.Services.AddHostedService<TaskModuloEmail>();

builder.Services.AddSingleton<TaskModuloCarga>();
builder.Services.AddHostedService<TaskModuloCarga>();

builder.Services.AddEntityFrameworkNpgsql()
    .AddDbContext<Contexto>(options =>
    options.UseNpgsql((Environment.GetEnvironmentVariable("CONNECTION_STRING_SIGA_DOC_INTEG"))));
/*PARA RODAR MIGRATION
builder.Services.AddDbContext<Contexto>(options =>
   options.UseNpgsql(("Host=192.168.0.185;Port=5432;Database=sigadocintegracaov2;Username=postgres;Password=admin123;")));
*/
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";
        options.AccessDeniedPath = "/AcessoNegado";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}");

app.Run();
