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
}
