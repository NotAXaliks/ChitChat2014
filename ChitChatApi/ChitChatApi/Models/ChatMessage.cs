using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChitChatApi.Models;

public partial class ChatMessage
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto increment
    public int Id { get; set; }

    public int Sender_Id { get; set; }

    public int Chatroom_Id { get; set; }

    public DateTime Date { get; set; }

    public required string Message { get; set; }

    [ForeignKey(nameof(Sender_Id))]
    public virtual required Employee Employee { get; set; }
    [ForeignKey(nameof(Chatroom_Id))]
    public virtual required Chatroom Chat { get; set; }
}