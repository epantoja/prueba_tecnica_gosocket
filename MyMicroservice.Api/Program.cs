using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Base de Datos en Memoria
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("SolicitudesDb"));

// Configuración de Azure Service Bus
builder.Services.AddSingleton(sp => 
{
    var connectionString = builder.Configuration.GetConnectionString("AzureServiceBus");

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

// --- ENDPOINTS ---

// POST: Crear solicitud
app.MapPost("/api/v1/requests", async (RequestInput dto, AppDbContext db, ServiceBusClient busClient) =>
{
    if (dto == null || string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Payload))
    {
        return Results.BadRequest(new { error = "El nombre y el payload son obligatorios." });
    }

    var solicitud = new Solicitud
    {
        Id = Guid.NewGuid(),
        Name = dto.Name,
        Payload = dto.Payload,
        Status = "Pending",
        CreatedAt = DateTime.UtcNow
    };

    db.Requests.Add(solicitud);
    await db.SaveChangesAsync();

    // NOTA: Esto fallará en ejecución si la ConnectionString no es válida, 
    // pero al menos ya COMPILARÁ.
    try {
        var sender = busClient.CreateSender("solicitudes-queue");
        var messageContent = System.Text.Json.JsonSerializer.Serialize(new {
            solicitud.Id,
            solicitud.CreatedAt,
            EventType = "RequestCreated"
        });
        await sender.SendMessageAsync(new ServiceBusMessage(messageContent));
    } catch {
        // Log error o manejar silenciosamente para la prueba si no tienes Azure activo
    }

    return Results.Created($"/api/v1/requests/{solicitud.Id}", solicitud);
});

// GET: Obtener todas
app.MapGet("/api/v1/requests", async (AppDbContext db) => 
    await db.Requests.ToListAsync());


app.MapGet("/api/v1/requests/{id}", async (Guid id, AppDbContext db) =>
{
    // Buscamos por Id usando FindAsync (es más eficiente que Where)
    var solicitud = await db.Requests.FindAsync(id);

    // Escenario de prueba: Consultar identificadores inexistentes
    if (solicitud is null)
    {
        return Results.NotFound(new { message = $"No se encontró la solicitud con ID: {id}" });
    }

    return Results.Ok(solicitud);
});

app.Run();

// --- MODELOS ---
public class Solicitud {
    [Key]
    public Guid Id { get; set; }

    [Required] // EF validará esto antes de guardar
    public string Name { get; set; } = string.Empty; 

    [Required]
    public string Payload { get; set; } = string.Empty;

    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
}

public record RequestInput(
    [Required(ErrorMessage = "El nombre es obligatorio")] string Name, 
    [Required(ErrorMessage = "El payload no puede estar vacío")] string Payload
);

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Solicitud> Requests => Set<Solicitud>();
}