using Avalonia.Controls;
using ChitChatDesktop.Pages;

namespace ChitChatDesktop;

public partial class EmployeeFinderWindow : Window
{
    public ChatPage? ChatPage { get; }

    public EmployeeFinderWindow(ChatPage? chatPage = null)
    {
        InitializeComponent();

        ChatPage = chatPage;

        EmployeeFinderFrame.Content = new EmployeeFinderPage(chatPage);
    }
}