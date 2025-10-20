using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Security.Claims;
using System.Text;
using TMCC.Db_Helper;
using TMCC.Models;
using TMCC.Repository;
using TMCC.Repository.IRepository;
using TMCC.Services;
using TMCC.Services.IServices;
using Quartz; 

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("Logs/tmcc-log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Use Serilog
builder.Host.UseSerilog();

// Add services
builder.Services.AddControllers();

// Register HttpClient
builder.Services.AddHttpClient();

/// Email config
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings")
);

// JWT Settings
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"];
var jwtIssuer = jwtSettings["Issuer"];
var jwtAudience = jwtSettings["Audience"];

if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 32)
    throw new InvalidOperationException("Jwt:Key must be at least 32 characters long");

var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        var jwtSettings = builder.Configuration.GetSection("Jwt");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Serilog.Log.Error(context.Exception, "JWT Authentication failed: {Message}", context.Exception.Message);

                if (context.Exception is SecurityTokenExpiredException)
                {
                    Serilog.Log.Warning("Token expired at {Expiry}", context.Exception.Data["expiry"]);
                    context.Response.Headers.Append("Token-Expired", "true");
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Serilog.Log.Information("JWT Token validated successfully for user: {UserId}", userId);
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Serilog.Log.Warning("Authorization challenge: {Error} - {Description}", context.Error, context.ErrorDescription);
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (!string.IsNullOrEmpty(token))
                    Serilog.Log.Information("Token received in request: {Token}", token);
                else
                    Serilog.Log.Warning("No token found in request header.");

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();


// Swagger with JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TMCC API",
        Version = "v1",
        Description = "API for TMCC Contracting App"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token (without 'Bearer' prefix)"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});


// DI (your services)
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<DapperHelper>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IClientPaymentHistoryRepository, ClientPaymentHistoryRepository>();
builder.Services.AddScoped<IClientPaymentHistoryService, ClientPaymentHistoryService>();
builder.Services.AddScoped<IEmployeePaymentHistoryRepository, EmployeePaymentHistoryRepository>();
builder.Services.AddScoped<IEmployeePaymentHistoryService, EmployeePaymentHistoryService>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IEmployeeHistoryAndStatusRepository, EmployeeHistoryAndStatusRepository>();
builder.Services.AddScoped<IEmployeeHistoryAndStatusService, EmployeeHistoryAndStatusService>();

// ============================================
// NEW: Email Service & Quartz Scheduler
// ============================================

// Register Email Service
builder.Services.AddScoped<IEmailService, EmailService>();

// Configure Quartz Scheduler for automated document expiry checks

builder.Services.AddQuartz(q =>
{
    // Client Documents – 10:00 AM
    var clientKey = new JobKey("ClientDocumentExpiryJob");
    q.AddJob<ClientDocumentExpiryJob>(opts => opts.WithIdentity(clientKey));
    q.AddTrigger(opts => opts
        .ForJob(clientKey)
        .WithIdentity("ClientDocumentExpiryTrigger")
        .WithCronSchedule("0 0 11 * * ?")); // 10 AM daily

    // Employee Documents – 11:00 AM
    var employeeKey = new JobKey("EmployeeDocumentExpiryJob");
    q.AddJob<EmployeeDocumentExpiryJob>(opts => opts.WithIdentity(employeeKey));
    q.AddTrigger(opts => opts
        .ForJob(employeeKey)
        .WithIdentity("EmployeeDocumentExpiryTrigger")
        .WithCronSchedule("0 0 11 * * ?")); // 11 AM daily

    // Company Documents – 12:00 PM
    var companyKey = new JobKey("CompanyDocumentExpiryJob");
    q.AddJob<CompanyDocumentExpiryJob>(opts => opts.WithIdentity(companyKey));
    q.AddTrigger(opts => opts
        .ForJob(companyKey)
        .WithIdentity("CompanyDocumentExpiryTrigger")
        .WithCronSchedule("0 0 12 * * ?")); // 12 PM daily
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// ============================================

var app = builder.Build();

if (app.Environment.IsDevelopment())
{ 
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TMCC API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();

// Authentication before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

Log.Information("TMCC API is starting...");
app.Run();