using Avalonia.Controls;
using ChitChatDesktop.Dtos;
using ChitChatDesktop.Pages;

namespace ChitChatDesktop;

public partial class ChatWindow : Window
{
    public ChatPage ChatPage { get; }
    
    public ChatWindow(ChatroomDto chat, EmployeeDto[] employees)
    {
        InitializeComponent();
        
        Title = $"Topic: {chat.Topic}";
        
        ChatPage = new ChatPage(chat, employees);
        ChatFrame.Content = ChatPage;
    }
}