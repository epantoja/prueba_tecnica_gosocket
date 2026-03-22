using Microsoft.EntityFrameworkCore;
using MyMicroservice.Api.Models;

namespace MyMicroservice.Api.Context;

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Solicitud> Requests => Set<Solicitud>();
}