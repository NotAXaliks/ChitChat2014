using Avalonia.Controls;
using ChitChatDesktop.Dtos;
using ChitChatDesktop.Pages;

namespace ChitChatDesktop;

public partial class ChatWindow : Window
{
    public ChatWindow(ChatroomDto chat, EmployeeDto[] employees)
    {
        InitializeComponent();
        
        Title = $"Topic: {chat.Topic}";
        
        ChatFrame.Content = new ChatPage(chat, employees);
    }
}