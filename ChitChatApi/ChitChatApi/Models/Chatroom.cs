using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChitChatApi.Models;

public partial class Chatroom
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto increment
    public int Id { get; set; }

    public required string Topic { get; set; }

    public virtual List<ChatMessage> Messages { get; set; } = [];
    public virtual List<ChatMembers> Members { get; set; } = [];
}
