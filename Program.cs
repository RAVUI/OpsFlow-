using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpsFlow.Configuration;
using OpsFlow.Data;
using OpsFlow.Services.Auth;
using OpsFlow.Services.Claims;
using OpsFlow.Services.Interfaces.Auth;
using OpsFlow.Services.Interfaces.Claims;
using OpsFlow.Services.Interfaces.Notifications;
using OpsFlow.Services.Notifications;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

var supabaseOptions =
    builder.Configuration
        .GetSection("Supabase")
        .Get<SupabaseOptions>()
    ?? throw new InvalidOperationException(
        "Supabase configuration is missing.");

builder.Services.AddSingleton(supabaseOptions);

builder.Services.AddSingleton<Supabase.Client>(
    provider =>
    {
        var options =
            provider.GetRequiredService<SupabaseOptions>();

        var clientOptions =
            new Supabase.SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = false
            };

        var client = new Supabase.Client(
            options.Url,
            options.PublishableKey,
            clientOptions);

        client.InitializeAsync()
              .GetAwaiter()
              .GetResult();

        return client;
    });


// SUPABASE JWT PUBLIC KEYS
var httpClient = new HttpClient();

var jwksJson = await httpClient.GetStringAsync(
    supabaseOptions.JwksUrl);

var jwks =
    new JsonWebKeySet(jwksJson);


builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {

                ValidateIssuer = true,

                ValidIssuer =
                    supabaseOptions.Issuer,

                ValidateAudience = false,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,

                IssuerSigningKeyResolver =
                    (token, securityToken, kid, validationParameters) =>
                    {
                        return jwks.Keys
                            .Where(x => x.Kid == kid);
                    }
            };


        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine(
                    "====================================");

                Console.WriteLine(
                    "JWT AUTHENTICATION FAILED");

                Console.WriteLine(
                    context.Exception.ToString());

                Console.WriteLine(
                    "====================================");

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("OpsFlowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddHttpContextAccessor();

builder.Services.Configure<SmtpOptions>(
    builder.Configuration.GetSection("Smtp"));

builder.Services.AddDbContext<OpsFlowDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",

            Type =
                Microsoft.OpenApi.Models.SecuritySchemeType.Http,

            Scheme = "bearer",

            BearerFormat = "JWT",

            In =
                Microsoft.OpenApi.Models.ParameterLocation.Header,

            Description =
                "Enter your JWT Bearer token."
        });

    options.AddSecurityRequirement(
        new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference =
                        new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type =
                                Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,

                            Id = "Bearer"
                        }
                },

                Array.Empty<string>()
            }
        });
});


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("OpsFlowFrontend");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();