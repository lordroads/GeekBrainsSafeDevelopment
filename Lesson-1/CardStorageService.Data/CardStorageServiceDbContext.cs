using CardStorageService.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CardStorageService.Data;

public class CardStorageServiceDbContext : DbContext
{
    public virtual DbSet<Client> Clients { get; set; }
    public virtual DbSet<Card> Cards { get; set; }

    public CardStorageServiceDbContext(DbContextOptions options) : base(options) { }
}