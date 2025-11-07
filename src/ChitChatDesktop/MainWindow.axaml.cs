using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ChitChatDesktop.Pages;

namespace ChitChatDesktop;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainFrame.Content = new MainPage();
    }
}