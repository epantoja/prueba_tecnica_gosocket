# Microservicio de Gestión de Solicitudes - .NET 8

## Objetivo
Implementación de un microservicio robusto para la creación y consulta de solicitudes con integración asíncrona a Azure Service Bus.

## Decisiones Técnicas
- **Arquitectura:** Minimal APIs en .NET 8 para un servicio ligero y de alto rendimiento.
- **Persistencia:** Entity Framework Core con base de datos **InMemory** para facilitar la ejecución inmediata sin dependencias externas.
- **Mensajería:** Integración con **Azure Service Bus (Queues)** para notificaciones asíncronas.
- **Validación:** Uso de *Data Annotations* para asegurar la integridad de los datos (Requerimiento 1).
- **Concurrencia:** Implementación de patrones `async/await` en todo el flujo de I/O (Requerimiento 3).

## Integración con Azure (Nota Importante)
El microservicio está configurado para emitir eventos `RequestCreated`. 
> **Nota:** Debido a restricciones temporales en la activación del Tenant de Azure (Error de actividad inusual en Microsoft ID), la integración se entrega configurada y validada a nivel de código, pero requiere una `ConnectionString` válida en el archivo `appsettings.json` para el envío real de mensajes. El sistema cuenta con manejo de excepciones para garantizar que la persistencia en DB funcione independientemente del estado del bus de mensajes.

## Cómo ejecutar localmente
1. Clonar el repositorio.
2. Asegurarse de tener instalado el SDK de **.NET 8**.
3. En la terminal, situarse en la carpeta del proyecto MyMicroservice.Api y ejecutar:
   ```bash
   dotnet run