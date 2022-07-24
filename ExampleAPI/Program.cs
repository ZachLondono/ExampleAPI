using ExampleAPI.Common;
using ExampleAPI.Companies.Data;
using ExampleAPI.Companies.Domain;
using ExampleAPI.Orders.Data;
using ExampleAPI.Orders.Domain;
using MediatR;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(typeof(Program).Assembly);
builder.Services.AddTransient<IRepository<Order>, OrderRepository>();
builder.Services.AddTransient<IRepository<Company>, CompanyRepository>();
builder.Services.AddTransient<NpgsqlOrderConnectionFactory>();
builder.Services.AddTransient<INotificationHandler<DomainEvent>, DomainEventHandler<DomainEvent>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseForwardedHeaders(new ForwardedHeadersOptions {
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run("http://*:5000");
