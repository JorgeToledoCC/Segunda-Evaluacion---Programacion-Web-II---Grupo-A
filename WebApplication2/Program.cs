using Microsoft.AspNetCore.Authentication.Cookies;
using WebApplication2.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Cliente para la API + envio automatico del JWT
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<ApiTokenHandler>();
builder.Services.AddHttpClient("Api", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5082");
})
.AddHttpMessageHandler<ApiTokenHandler>();

// Sesion por cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Logout";
        options.AccessDeniedPath = "/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();
app.Run();
