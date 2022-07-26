using ExampleAPI.Common.Data;
using ExampleAPI.Common.Domain;
using ExampleAPI.Sales;
using MediatR;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(typeof(Program).Assembly);
builder.Services.ConfigureSales();
builder.Services.AddTransient<NpgsqlOrderConnectionFactory>();
builder.Services.AddTransient<INotificationHandler<DomainEvent>, DomainEventHandler<DomainEvent>>();

Log.Logger = new LoggerConfiguration()
                .ReadFrom
                .Configuration(builder.Configuration)
                .CreateLogger();

builder.Host.UseSerilog();

try {

    Log.Information("Application Starting Up");

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

    app.UseSerilogRequestLogging();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.Run("http://*:5000");

} catch (Exception ex) {

    Log.Fatal("Application Failed to Start {Exception}", ex);

} finally {

    Log.Information("Shut Down Complete");
    Log.CloseAndFlush();

}