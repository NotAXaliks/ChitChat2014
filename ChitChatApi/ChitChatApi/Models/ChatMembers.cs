using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ChitChatApi.Models;

[PrimaryKey(nameof(Chatroom_Id), nameof(Employee_Id))]
public partial class ChatMembers
{
    public int Chatroom_Id { get; set; }

    public int Employee_Id { get; set; }

    [ForeignKey(nameof(Chatroom_Id))]
    public virtual required Chatroom Chatroom { get; set; }
    [ForeignKey(nameof(Employee_Id))]
    public virtual required Employee Employee { get; set; }
}
