# Microservicio de Gestión de Solicitudes - .NET 8

## Objetivo
Implementación de un microservicio microservicio en .NET 8 que permita gestionar 
procesamiento de creación y consulta de solicitudes con integración asíncrona a Azure Service Bus.

## Decisiones Técnicas
- **Arquitectura:** Minimal APIs en .NET 8 para un servicio ligero y robusto.
- **Persistencia:** Entity Framework Core con base de datos **InMemory** para facilitar la ejecución inmediata sin dependencias externas, pero de manera opcional se implemnto la persintencia en **Postgresql** base de datos relacional, todo esto para garantizar que los datos persistan localmente de forma segura, y no sean temporales como en **InMemory**.
- **Mensajería:** Integración con **Azure Service Bus (Queues)** para notificaciones asíncronas.
- **Validación:** Se implementó una lógica de control manual al inicio del endpoint para asegurar que campos obligatorios como Name y Payload sean requeridos. El sistema garantiza una respuesta 400 Bad Request con un mensaje descriptivo si la información es incompleta, manteniendo la integridad de la base de datos.
- **Concurrencia:** Implementación de patrones `async/await` para que el servicio sea totalmente **No Bloqueante**, esto se aplica en todos los metodos.

## Integración con Azure (Nota Importante)
El microservicio está configurado para emitir eventos `RequestCreated`. 
> **Nota:** Debido a restricciones temporales en la activación del Tenant de Azure (Error de actividad inusual en Microsoft ID), la integración se entrega configurada y validada a nivel de código, pero requiere una `ConnectionString` válida en el archivo `appsettings.json` para el envío real de mensajes. El sistema cuenta con manejo de excepciones para garantizar que la persistencia en DB funcione independientemente del estado del bus de mensajes.

## Requisitos
- SDK de .NET 8
- Postman o acceso a Swagger (incluido)

## Guía de Inicio para Evaluadores
1. **Configurar Base de Datos: --opcional**
   Asegúrese de tener PostgreSQL corriendo y actualice la cadena de conexión en `appsettings.json`, esto es muy opcional, ya que no logre conectarme a azure
2. **Ejecutar script de Base de Datos: --opcional**
   Como no estoy utilizando Migrations, para las tablas sql, entonces corremos en Postgres este script
   ```bash
   CREATE TABLE "Requests" (
      "Id" UUID PRIMARY KEY,
      "Name" TEXT NOT NULL,
      "Payload" TEXT NOT NULL,
      "Status" TEXT NOT NULL DEFAULT 'Pending',
      "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL
   );
3. **Restaurar Dependencias:**
   ```bash
   dotnet restore

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