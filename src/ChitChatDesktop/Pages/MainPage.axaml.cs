using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ChitChatDesktop.Services;
using MsBox.Avalonia;

namespace ChitChatDesktop.Pages;

public partial class MainPage : UserControl
{
    private ObservableCollection<Chatroom> ChatList { get; set; } = [];

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
            await MessageBoxManager.GetMessageBoxStandard("Error", "An error occurred while fetching the user.")
                .ShowAsync();
            return;
        }

        HelloText.Text = $"Hello, {me.Name}!";

        var openChatsResponse = await ChatApi.GetOpen();
        var chats = openChatsResponse.Data;
        if (chats == null) return;

        foreach (var chat in chats)
        {
            ChatList.Add(new Chatroom
            {
                Topic = chat.Chatroom.Topic,
                LastMessageDate = chat.LastMessageDate.HasValue
                    ? DateTimeOffset.FromUnixTimeMilliseconds(chat.LastMessageDate.Value).ToString("MM.dd HH:mm")
                    : "Empty chat"
            });
        }
    }

    public class Chatroom
    {
        public string Topic { get; set; }
        public string LastMessageDate { get; set; }
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