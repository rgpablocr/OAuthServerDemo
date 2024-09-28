using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using OAuthServer.Endpoints;
using OAuthServer.Endpoints.OAuth;
using OAuthServer.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", //could be any name
        builder =>
        {
            builder.WithOrigins("http://localhost:5016").AllowAnyHeader().AllowAnyMethod();
            builder.WithOrigins("https://localhost:7037").AllowAnyHeader().AllowAnyMethod();
        });
});
//we use options pattern or IConfiguration
builder.Services.Configure<JWTKey>(builder.Configuration.GetSection("JWTKeysConfiguration"));
//var secretKey = builder.Configuration.GetValue<JWTKey>("JWTKeysConfiguration").SecretKey.ToString();

builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie", o =>
    {
        o.LoginPath = "/login";
    });

//builder.Services.Configure<AuthenticationOptions>(options => options.RequireAuthenticatedSignIn = true);

builder.Services.AddAuthorization();
builder.Services.AddSingleton<SecretKeys>();

var app = builder.Build();

app.UseCors("AllowSpecificOrigin");  

app.MapGet("/login", GetLogin.GetLoginHandler);
app.MapPost("/login", Login.LoginHandler);
app.MapGet("/oauth/authorize", Authorize.AuthorizeHandler).RequireAuthorization(); //valida que si no hay cookie dirige a loginpath especificado
app.MapPost("/oauth/token", Token.TokenHandler);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();


app.Run();


