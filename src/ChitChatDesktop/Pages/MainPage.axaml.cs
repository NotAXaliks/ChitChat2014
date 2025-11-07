using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ChitChatDesktop.Services;
using MsBox.Avalonia;

namespace ChitChatDesktop.Pages;

public partial class MainPage : UserControl
{
    public ObservableCollection<Chatroom> ChatList { get; set; } = [];

    public MainPage()
    {
        InitializeComponent();
        DataContext = this;

        Refresh();
    }

    private async void Refresh()
    {
        var meResponse = await EmployeeApi.GetMe();
        var me = meResponse.Data;
        if (me == null)
        {
            await MessageBoxManager.GetMessageBoxStandard("Error", "An error occurred while fetching the user.").ShowAsync();
            return;
        }
        
        HelloText.Text = $"Hello, {me.Name}!";

        ChatList.Clear();
        ChatList.Add(new Chatroom { Topic = "The roof is on firsfdgsdfgsdfgsdfgfsde", LastMessage = "24.11. 15:10" });
        ChatList.Add(new Chatroom { Topic = "Sebastian", LastMessage = "15.11. 08:30" });
        ChatList.Add(new Chatroom { Topic = "Lisa", LastMessage = "1.12. 10:30" });
        ChatList.Add(new Chatroom { Topic = "IT Helpdesk", LastMessage = "1.8. 17:23" });
    }

    public class Chatroom
    {
        public string Topic { get; set; }
        public string LastMessage { get; set; }
    }

    private void OnEmployeeFinderClick(object? sender, RoutedEventArgs e)
    {
        (VisualRoot as Window)?.Close();
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        (VisualRoot as Window)?.Close();
    }
}