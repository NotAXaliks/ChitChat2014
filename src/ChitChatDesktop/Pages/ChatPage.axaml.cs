using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ChitChatDesktop.Dtos;
using ChitChatDesktop.Services;
using MsBox.Avalonia;

namespace ChitChatDesktop.Pages;

public partial class ChatPage : UserControl
{
    public event Action? OnLeaveChat;
    
    public record ChatMessage(string Time, string Sender, string Text);
    
    public readonly int ChatId;
    public ChatroomDto? Chat;
    public EmployeeDto?[] Employees;

    public ChatPage(int _chatId)
    {
        InitializeComponent();

        ChatId = _chatId;

        Refresh();
    }

    public async void Refresh()
    {
        // Вставляем участников чата
        var chatResponse = await ChatApi.Get(ChatId);
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
        
        Chat = chatResponse.Data.Chatroom;
        Employees = chatResponse.Data.Members;
        EmployeeList.ItemsSource = Employees;

        // TODO: Сделать вебсокеты. Пока что это кринж
        var chatMessages = new List<ChatMessage>
        {
            new ChatMessage("10:50", "Saskia M", "fix it yourself!!"),
            new ChatMessage("10:50", "Saskia M", "fix it yourself!!"),
            new ChatMessage("10:50", "Saskia M", "fix it yourself!!"),
            new ChatMessage("10:50", "Saskia M", "fix it yourself!!"),
            new ChatMessage("10:50", "Saskia M", "fix it yourself!!"),
            new ChatMessage("10:50", "Saskia M", "fix it yourself!!"),
            new ChatMessage("10:50", "Saskia M", "fix it yourself!!"),
            new ChatMessage("10:50", "Saskia M", "fix it yourself!!"),
            new ChatMessage("10:50", "Saskia M", "fix it yourself!!"),
            new ChatMessage("10:50", "Saskia M", "fix it yourself!!"),
            new ChatMessage("10:50", "Saskia M", "fix it yourself!!"),
            new ChatMessage("10:50", "Saskia M", "fix it yourself!!"),
            new ChatMessage("10:50", "Saskia M", "fix it yourself!!"),
            new ChatMessage("10:50", "Saskia M", "fix it yourself!!"),
            new ChatMessage("10:50", "Saskia M", "fix it yourself!!"),
            new ChatMessage("10:50", "Saskia M", "fix it yourself!!"),
            new ChatMessage("10:50", "Saskia M", "fix it yourself!!"),
            new ChatMessage("10:50", "Saskia M", "fix it yourself!!"),
            new ChatMessage("10:50", "Saskia M", "fix it yourself!!"),
            new ChatMessage("10:51", "Mario M", "New message here")
        };

        foreach (var message in chatMessages)
        {
            MessagesPanel.Children.Add(new TextBlock
            {
                Text = $"[{message.Time}] {message.Sender}: {message.Text}",
                Margin = new Avalonia.Thickness(5, 2),
            });
        }
    }

    private void OnSendClick(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("Clicked on Send button");
    }

    private void OnAddUserClick(object? sender, RoutedEventArgs e)
    {
         // TODO: Запретить создавать несколько окон. Хранить и затем переключаться на активное окно
        var employeeFinderWindow = new EmployeeFinderWindow(this);
        employeeFinderWindow.Show();
    }

    private void OnChangeTopicClick(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("Clicked on ChangeTopic button");
    }

    private async void OnLeaveClick(object? sender, RoutedEventArgs e)
    {
        await ChatApi.Leave(Chat.Id);
        
        (VisualRoot as Window)?.Close();
        
        OnLeaveChat?.Invoke();
    }

    private void OnEmployeeSelectClick(object? sender, TappedEventArgs e)
    {
        var employee = EmployeeList.SelectedItem;
    }
}