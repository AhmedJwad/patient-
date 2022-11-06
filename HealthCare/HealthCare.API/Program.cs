using Grpc.Net.Client.Configuration;
using HealthCare.API;
using HealthCare.API.Data;
using HealthCare.API.Data.Entities;
using HealthCare.API.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DataContext>(o =>
{
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddIdentity<User, IdentityRole>(x =>
{
    x.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
    x.SignIn.RequireConfirmedEmail = true;
    x.User.RequireUniqueEmail = false;
    x.Password.RequireDigit = false;
    x.Password.RequiredUniqueChars = 0;
    x.Password.RequireLowercase = false;
    x.Password.RequireNonAlphanumeric = false;
    x.Password.RequireUppercase = false;   
}).AddDefaultTokenProviders()
.AddEntityFrameworkStores<DataContext>();


builder.Services.AddAuthentication()
                .AddCookie()                
                .AddJwtBearer(cfg =>
                {
                    
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                   
                    ValidIssuer = builder.Configuration["Tokens:Issuer"],
                        ValidAudience = builder.Configuration["Tokens:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Tokens:Key"]))
                    };
                });
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/NotAuthorized";
    options.AccessDeniedPath = "/Account/NotAuthorized";
});
builder.Services.AddTransient<SeedDb>();
builder.Services.AddScoped<IuserHelper, UserHelper>();
builder.Services.AddScoped<ICombosHelper, Comboshelper>();
builder.Services.AddScoped<IBlobHelper, BlobHelper>();
builder.Services.AddScoped<IconverterHelper, ConverterHelper>();
builder.Services.AddScoped<IMailHelper, MailHelper>();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
var app = builder.Build();
SeedData();
void SeedData()
{
    IServiceScopeFactory? scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (IServiceScope? scope = scopedFactory.CreateScope())
    {
        SeedDb? service = scope.ServiceProvider.GetService<SeedDb>();
        service.SeedAsync().Wait();
    }
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    //The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseStatusCodePagesWithReExecute("/error/{0}");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
