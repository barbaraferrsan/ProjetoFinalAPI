using AutoMapper;
using ProjetoFinalAPI.Infra.Data.Repository;
using ProjetoFinalAPI.Service.Dto;
using ProjetoFinalAPI.Service.Entity;
using ProjetoFinalAPI.Service.Interface;
using ProjetoFinalAPI.Service.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ProjetoFinalAPI.Service.Dto;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var chaveCripto = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("SECRET_KEY"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(chaveCripto),
                    ValidateIssuer = true,
                    ValidIssuer = "APIPessoa.com",
                    ValidateAudience = true,
                    ValidAudience = "CityEvents.com"
                };
            });

builder.Services.AddScoped<ICityEventService, CityEventService>();
builder.Services.AddScoped<ICityEventRepository, CityEventRepository>();

builder.Services.AddScoped<IEventReservationRepository, EventReservationRepository>();
builder.Services.AddScoped<IEventReservationService, EventReservationService>();

MapperConfiguration mapperConfig = new(mc =>
{
    mc.CreateMap<CityEventEntity, CityEventDto>().ReverseMap();

    mc.CreateMap<EventReservationEntity, EventReservationDto>().ReverseMap();
});

IMapper mapper = mapperConfig.CreateMapper();

builder.Services.AddSingleton(mapper);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();