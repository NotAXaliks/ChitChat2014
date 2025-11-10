using Avalonia.Controls;
using ChitChatDesktop.Dtos;
using ChitChatDesktop.Pages;

namespace ChitChatDesktop;

public partial class ChatWindow : Window
{
    public ChatPage ChatPage { get; }
    
    public ChatWindow(int chatId, string chatTopic)
    {
        InitializeComponent();
        
        Title = $"Topic: {chatTopic}";
        
        ChatPage = new ChatPage(chatId);
        ChatFrame.Content = ChatPage;
    }
}