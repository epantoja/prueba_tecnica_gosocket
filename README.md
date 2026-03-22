# Microservicio de Gestión de Solicitudes - .NET 8

## Objetivo
Implementación de un microservicio microservicio en .NET 8 que permita gestionar 
procesamiento de creación y consulta de solicitudes con integración asíncrona a Azure Service Bus.

## Decisiones Técnicas
- **Arquitectura:** APIs en .NET 8 para un servicio ligero y .
- **Persistencia:** Entity Framework Core con base de datos **InMemory** para facilitar la ejecución inmediata sin dependencias externas.
- **Mensajería:** Integración con **Azure Service Bus (Queues)** para notificaciones asíncronas.
- **Validación:** Uso de *Data Annotations* para asegurar la integridad de los datos, y validarlos antes de persistir en bae de atos.
- **Concurrencia:** Implementación de patrones `async/await` .

## Integración con Azure (Nota Importante)
El microservicio está configurado para emitir eventos `RequestCreated`. 
> **Nota:** Debido a restricciones temporales en la activación del Tenant de Azure (Error de actividad inusual en Microsoft ID), la integración se entrega configurada y validada a nivel de código, pero requiere una `ConnectionString` válida en el archivo `appsettings.json` para el envío real de mensajes. El sistema cuenta con manejo de excepciones para garantizar que la persistencia en DB funcione independientemente del estado del bus de mensajes.

## Requisitos
- SDK de .NET 8
- Postman o acceso a Swagger (incluido)

### Cómo ejecutar localmente
1. Clonar el repositorio.
2. Asegurarse de tener instalado el SDK de **.NET 8**.
3. En la terminal, situarse en la carpeta del proyecto MyMicroservice.Api y ejecutar:

### Ejecución
1. Sitúate en la carpeta raíz del proyecto:
   ```bash
   cd MyMicroservice.Api
2. Correr proyecto
   ```bash
   dotnet run