using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ChitChatDesktop.Services;
using MsBox.Avalonia;

namespace ChitChatDesktop.Pages;

public partial class MainPage : UserControl
{
    private ObservableCollection<OpenableChat> ChatList { get; set; } = [];

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
        
        ChatList.Clear();

        var openChatsResponse = await ChatApi.GetOpen();
        var chats = openChatsResponse.Data;
        if (chats == null) {
            await MessageBoxManager.GetMessageBoxStandard("Error", "An error occurred while fetching the chats.")
                .ShowAsync();
            return;
        }

        foreach (var chat in chats)
        {
            ChatList.Add(new OpenableChat(
                chat.Chatroom.Id,
                chat.Chatroom.Topic,
                chat.LastMessageDate.HasValue
                    ? DateTimeOffset.FromUnixTimeMilliseconds(chat.LastMessageDate.Value).ToString("MM.dd HH:mm")
                    : "Empty chat"
            ));
        }
    }

    private record OpenableChat(int Id, string Topic, string LastMessageDate);

    private async void OpenChat(OpenableChat chat)
    {
        var chatResponse = await ChatApi.Get(chat.Id);
        if (!string.IsNullOrWhiteSpace(chatResponse.Error))
        {
            await MessageBoxManager.GetMessageBoxStandard("Error", chatResponse.Error).ShowAsync();
            Refresh();
            return;
        }

        if (chatResponse.Data == null)
        {
            await MessageBoxManager.GetMessageBoxStandard("Error", "An error occurred while fetching the chat.")
                .ShowAsync();
            Refresh();
            return;
        }

        // TODO: Запретить создавать несколько окон. Хранить и затем переключаться на активное окно
        var chatWindow = new ChatWindow(chatResponse.Data.Chatroom, chatResponse.Data.Members);
        
        chatWindow.ChatPage.OnLeaveChat += () => Refresh();
        
        chatWindow.Show();
    }

    private void OnChatlistDoubleClick(object? sender, RoutedEventArgs e)
    {
        if (sender is DataGrid dataGrid && dataGrid.SelectedItem is OpenableChat selectedChat)
        {
            OpenChat(selectedChat);
        }
    }

    private void OnEmployeeFinderClick(object? sender, RoutedEventArgs e)
    {
        Refresh();
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        (VisualRoot as Window)?.Close();
    }
}