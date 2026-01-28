using Application.Hubs;
using Application.Managers.Chat;
using Application.Repositories;
using Application.Services.AuthService;
using Application.Services.ChatService;
using Application.Services.ClientUserService;
using Application.Services.CurrentUserService;
using Application.Services.DashboardService;
using Application.Services.FileService;
using Application.Services.FirebaseService;
using Application.Services.LookupService;
using Application.Services.NotificationService;
using Application.Services.OrderService;
using Application.Services.Service;
using Application.Services.ServiceProviderService;
using Application.Services.DashboardService;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Infrastructre.Context;
using Infrastructre.Data;
using Infrastructre.Repositories;
using Infrastructre.Services.CurrentUserService;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<BSFContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
);

var firebasePath = Path.Combine(AppContext.BaseDirectory, "Firebase", "bsfapp-12dd8-firebase-adminsdk-fbsvc-ab7f17b10d.json");
var firebaseApp = FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(firebasePath)
});

var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"])),
            ClockSkew = TimeSpan.Zero,
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BSF API",
        Version = "v1"
    });


    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Put **_ONLY_** your JWT Bearer token hera",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityReq = new OpenApiSecurityRequirement
    {
        {securityScheme, new string[] { } }
    };
    c.AddSecurityRequirement(securityReq);
});

builder.Services.AddSignalR();

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IAuthService), typeof(AuthService));
builder.Services.AddScoped(typeof(ICurrentUserService), typeof(CurrentUserService));
builder.Services.AddScoped(typeof(IServiceProviderService), typeof(ServiceProviderService));
builder.Services.AddScoped(typeof(IClientUserService), typeof(ClientUserService));
builder.Services.AddScoped(typeof(ILookupService), typeof(LookupService));
builder.Services.AddScoped(typeof(IServicesService), typeof(ServicesService));
builder.Services.AddScoped(typeof(IOrderService), typeof(OrderService));
builder.Services.AddScoped(typeof(INotificationService), typeof(NotificationService));
builder.Services.AddScoped(typeof(IChatService), typeof(ChatService));
builder.Services.AddScoped(typeof(IChatConnectionManager), typeof(ChatConnectionManager));
builder.Services.AddScoped(typeof(IFileService), typeof(FileService));
builder.Services.AddScoped(typeof(IFirebaseService), typeof(FirebaseService));
builder.Services.AddScoped(typeof(IDashboardService), typeof(DashboardService));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

app.UseCors("AllowAll");

UserSeedDate.UserSeed(app.Services);

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chatHub");


app.Run();
