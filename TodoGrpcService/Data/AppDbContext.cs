using Microsoft.EntityFrameworkCore;
using TodoGrpcService.Models;

namespace TodoGrpcService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
       
    }

    DbSet<Todo> Todos => Set<Todo>();
}
