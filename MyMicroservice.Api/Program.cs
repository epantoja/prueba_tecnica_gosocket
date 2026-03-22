using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using MyMicroservice.Api.Context;
using MyMicroservice.Api.Models;
using MyMicroservice.Api.Dto;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

//conexion a InMemory.
//builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("SolicitudesDb"));

//conexion a posgresql
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreConnection")));

builder.Services.AddSingleton(sp => 
{
    
    var config = sp.GetRequiredService<IConfiguration>();
    var connectionString = config.GetConnectionString("AzureServiceBus");

    if (string.IsNullOrEmpty(connectionString) || connectionString.Contains("Tu_Cadena"))
    {
        return new ServiceBusClient("Endpoint=sb://fake.servicebus.windows.net/;SharedAccessKeyName=Root;SharedAccessKey=fake");
    }
    
    return new ServiceBusClient(connectionString);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


/*
Crear solicitud
*/ 
app.MapPost("/api/v1/requests", async (RequestInputDto dto, AppDbContext db, ServiceBusClient busClient, IConfiguration config) =>
{   
    if (dto == null || string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Payload))
    {
        return Results.BadRequest(new { error = "El nombre y el payload son obligatorios." });
    }

    var solicitud = new Solicitud {
        Id = Guid.NewGuid(),
        Name = dto.Name,
        Payload = dto.Payload,
        CreatedAt = DateTime.UtcNow
    };
    db.Requests.Add(solicitud);
    await db.SaveChangesAsync();

    try {
        string queueName = config["ServiceBusConfig:QueueName"] ?? "solicitudes-queue";
        var sender = busClient.CreateSender(queueName);

        var messagePayload = new {
            Id = solicitud.Id,
            CreatedAt = solicitud.CreatedAt,
            EventType = "RequestCreated"
        };
        
        string messageBody = System.Text.Json.JsonSerializer.Serialize(messagePayload);
        var message = new ServiceBusMessage(messageBody);

        await sender.SendMessageAsync(message);
    }
    catch (Exception ex) {
        Console.WriteLine($">>> Error con Azure: {ex.Message}");
    }

    return Results.Created($"/api/v1/requests/{solicitud.Id}", solicitud);
});

/*
Obtener todas
*/
app.MapGet("/api/v1/requests", async (AppDbContext db) => 
    await db.Requests.ToListAsync());


/*
Obtener por Id
*/
app.MapGet("/api/v1/requests/{id}", async (Guid id, AppDbContext db) =>
{
    var solicitud = await db.Requests.FindAsync(id);

    if (solicitud is null)
    {
        return Results.NotFound(new { message = $"No se encontró la solicitud con ID: {id}" });
    }

    return Results.Ok(solicitud);
});

app.Run();





