using ChitChatApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ChitChatApi.Context;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    
    public virtual DbSet<ChatMembers> ChatMembers { get; set; }
    public virtual DbSet<ChatMessage> ChatMessages { get; set; }
    public virtual DbSet<Chatroom> Chatrooms { get; set; }
    public virtual DbSet<Department> Departments { get; set; }
    public virtual DbSet<Employee> Employees { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatMessage>()
            .HasIndex(m => new { m.Chatroom_Id, m.Id })
            .IsDescending(false, true);
        
        modelBuilder.Entity<ChatMessage>()
            .Property(m => m.Date)
            .HasDefaultValueSql("now()");

        base.OnModelCreating(modelBuilder);
    }
}
