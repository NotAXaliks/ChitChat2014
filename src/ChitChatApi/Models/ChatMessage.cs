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
    public virtual Employee? Employee { get; set; }
    [ForeignKey(nameof(Chatroom_Id))]
    public virtual Chatroom? Chat { get; set; }
}