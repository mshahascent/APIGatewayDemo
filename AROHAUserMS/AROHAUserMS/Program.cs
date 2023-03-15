using AROHAUserMS.DataAccess.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

string connString = builder.Configuration.GetConnectionString("AROHADBConnection");
//builder.Services.AddDbContext<ApplicationDbContext>(
//        options => options.UseSqlServer(connString));

builder.Services.AddDbContextPool<ArohaContext>(options => options.UseSqlServer(connString));


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

//app.Run(async (context) => {
//    await context.Response.WriteAsync(connString);
//});

app.Run();
