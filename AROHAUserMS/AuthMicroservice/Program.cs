using AROHAUserMS.DataAccess.EFCore;
using AROHAUserMS.DataAccess.EFCore.Services;
using AuthDataAccess.Entities;
using DataAccess.EFCore.DBContext;
using DataAccess.EFCore.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var emailConfig = builder.Configuration
      .GetSection("EmailConfiguration")
      .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);

builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddControllers();
string connstring = builder.Configuration.GetConnectionString("AROHADBConnection");
builder.Services.AddDbContextPool<ArohaContext>(options=>options.UseSqlServer(connstring));
builder.Services.AddDbContextPool<AuthManagementDbContext>(options => {
    options.UseSqlServer(connstring ,b=>b.MigrationsAssembly("DataAccess.EFCore"));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddIdentity<CIdentityUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireDigit = true;
        options.User.RequireUniqueEmail = true;
    }).AddEntityFrameworkStores<AuthManagementDbContext>()
    .AddDefaultTokenProviders();
    


var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
