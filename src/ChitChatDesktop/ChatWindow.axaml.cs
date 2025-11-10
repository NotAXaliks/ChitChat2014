using Avalonia.Controls;
using ChitChatDesktop.Dtos;
using ChitChatDesktop.Pages;

namespace ChitChatDesktop;

public partial class ChatWindow : Window
{
    public ChatPage ChatPage { get; }
    
    public ChatWindow(OpenableChat chat)
    {
        InitializeComponent();
        
        Title = $"Topic: {chat.Topic}";
        
        ChatPage = new ChatPage(chat.Id);
        ChatFrame.Content = ChatPage;
    }
}