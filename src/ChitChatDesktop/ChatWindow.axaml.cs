using Avalonia.Controls;
using ChitChatDesktop.Pages;

namespace ChitChatDesktop;

public partial class ChatWindow : Window
{
    public ChatPage ChatPage { get; }
    
    public ChatWindow(int chatId)
    {
        InitializeComponent();
        
        ChatPage = new ChatPage(chatId);
        ChatFrame.Content = ChatPage;
    }
}