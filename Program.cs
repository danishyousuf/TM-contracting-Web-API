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
builder.Services.AddHttpClient();

// Email configuration
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

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
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
            OnMessageReceived = context =>
            {
                // Allow OPTIONS requests to pass through without authentication
                if (context.Request.Method == "OPTIONS")
                {
                    context.NoResult();
                }
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

// CORS policy - Allow all origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(origin => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .WithExposedHeaders("Token-Expired");
    });
});

// Dependency Injection
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
builder.Services.AddScoped<IEmailService, EmailService>();

// Quartz Scheduler
builder.Services.AddQuartz(q =>
{
    var clientKey = new JobKey("ClientDocumentExpiryJob");
    q.AddJob<ClientDocumentExpiryJob>(opts => opts.WithIdentity(clientKey));
    q.AddTrigger(opts => opts.ForJob(clientKey).WithIdentity("ClientDocumentExpiryTrigger")
        .WithCronSchedule("0 0 11 * * ?"));

    var employeeKey = new JobKey("EmployeeDocumentExpiryJob");
    q.AddJob<EmployeeDocumentExpiryJob>(opts => opts.WithIdentity(employeeKey));
    q.AddTrigger(opts => opts.ForJob(employeeKey).WithIdentity("EmployeeDocumentExpiryTrigger")
        .WithCronSchedule("0 0 11 * * ?"));

    var companyKey = new JobKey("CompanyDocumentExpiryJob");
    q.AddJob<CompanyDocumentExpiryJob>(opts => opts.WithIdentity(companyKey));
    q.AddTrigger(opts => opts.ForJob(companyKey).WithIdentity("CompanyDocumentExpiryTrigger")
        .WithCronSchedule("0 0 12 * * ?"));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

// ==========================================
// MIDDLEWARE PIPELINE
// ORDER IS ABSOLUTELY CRITICAL FOR CORS!
// ==========================================

// Swagger (Development only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TMCC API v1");
        c.RoutePrefix = "swagger";
    });
}

// STEP 1: CORS FIRST - Must come before Authentication/Authorization
app.UseCors("AllowAll");

// STEP 2: HTTPS Redirection
app.UseHttpsRedirection();

// STEP 3: Authentication (JWT validation)
app.UseAuthentication();

// STEP 4: Authorization
app.UseAuthorization();

// STEP 5: Map Controllers
app.MapControllers();

Log.Information("TMCC API is starting...");
app.Run();
