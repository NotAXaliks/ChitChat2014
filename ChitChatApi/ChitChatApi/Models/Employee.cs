using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ChitChatApi.Models;

[Index(nameof(Username), IsUnique = true)]
public partial class Employee
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto increment
    public int Id { get; set; }

    public required string Name { get; set; }

    public int Department_Id { get; set; }

    [ForeignKey(nameof(Department_Id))]
    public virtual required Department Department { get; set; }

    public required string Username { get; set; }

    public required string Password { get; set; }

    public virtual ICollection<ChatMessage> Messages { get; set; } = [];
    public virtual ICollection<ChatMembers> ChatMember { get; set; } = [];
}