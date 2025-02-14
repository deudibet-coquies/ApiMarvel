using ApiMarvel;
using ApiMarvel.Data;
using ApiMarvel.Modelos;
using ApiMarvel.Repositorio;
using ApiMarvel.Repositorio.IRepositorio;
using ApiMarvel.Servicios;
using ApiMarvel.Servicios.IServicios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


//add coneccion a la db
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
builder.Configuration.GetConnectionString("ConexionSql")));

//implementacion de identity
//builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddIdentity<AppUsuario, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
//soporte a cache
builder.Services.AddResponseCaching();

//add Repositories
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
builder.Services.AddScoped<IMarvelService, MarvelService>();


builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true,reloadOnChange:true);
builder.Services.Configure<Appsettings>(builder.Configuration.GetSection("Keys"));

//Dto
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//cors

builder.Services.AddCors(p => p.AddPolicy("PolicityCors", builder => 
{
        builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));


var key = builder.Configuration.GetValue<string>("ApiSettings:Secreta");
builder.Services.AddAuthentication(a =>
{
    a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});


builder.Services.AddControllers(opcion =>
{
    opcion.CacheProfiles.Add("PorDefecto30Segundos", new  CacheProfile() { Duration = 30 });
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
        "Autenticación JWT usando el esquema Bearer. \r\n\r\n " +
        "Ingresa la palabra 'Bearer' seguida de un [espacio] y despues su token en el campo de abajo \r\n\r\n" +
        "Ejemplo: \"Bearer tkdknkdllskd\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});


var app = builder.Build();

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
app.UseSwagger();
app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseCors("PolicityCors");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
