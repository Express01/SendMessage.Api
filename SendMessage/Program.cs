using Microsoft.EntityFrameworkCore;
using SendMessage.SendMessage.Data.Context;
using SendMessage.SendMessage.Data.Repositories;
using SendMessage.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddDbContext<MSSqlContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("LogMessage")));
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IMailRepository,MailRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
