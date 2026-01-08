using Infrastructre.Context;
using Infrastructre.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<BSFContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
);


var app = builder.Build();

UserSeedDate.UserSeed(app.Services);

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
