using Avalonia.Controls;
using ChitChatDesktop.Pages;

namespace ChitChatDesktop;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        LoginFrame.Content = new LoginPage();
        App.loginWindow = this;
    }
}