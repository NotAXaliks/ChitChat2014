using Avalonia.Controls;
using ChitChatDesktop.Pages;

namespace ChitChatDesktop;

public partial class EmployeeFinderWindow : Window
{
    public EmployeeFinderWindow()
    {
        InitializeComponent();
        EmployeeFinderFrame.Content = new EmployeeFinderPage();
    }
}