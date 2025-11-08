using Avalonia.Controls;
using ChitChatDesktop.Dtos;

namespace ChitChatDesktop.Pages;

public partial class ChatPage : UserControl
{
    public ChatPage(ChatroomDto chat, EmployeeDto[] employees)
    {
        InitializeComponent();
    }
}