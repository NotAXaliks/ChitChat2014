using Avalonia.Controls;
using ChitChatDesktop.Pages;

namespace ChitChatDesktop;

public partial class ChatWindow : Window
{
    public ChatWindow()
    {
        InitializeComponent();
        ChatFrame.Content = new ChatPage();
    }
}