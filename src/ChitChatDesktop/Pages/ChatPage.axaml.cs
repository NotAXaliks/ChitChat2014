using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ChitChatDesktop.Dtos;
using ChitChatDesktop.Services;

namespace ChitChatDesktop.Pages;

public partial class ChatPage : UserControl
{
    public event Action? OnLeaveChat;
    
    public record ChatMessage(string Time, string Sender, string Text);
    
    private readonly ChatroomDto _chat;
    private readonly EmployeeDto[] _employees;

    public ChatPage(ChatroomDto chat, EmployeeDto[] employees)
    {
        InitializeComponent();

        _chat = chat;
        _employees = employees;

        Refresh();
    }

    private void Refresh()
    {
        // Вставляем участников чата
        EmployeeList.ItemsSource = _employees;

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
        Console.WriteLine("Clicked on AddUser button");
    }

    private void OnChangeTopicClick(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("Clicked on ChangeTopic button");
    }

    private async void OnLeaveClick(object? sender, RoutedEventArgs e)
    {
        await ChatApi.Leave(_chat.Id);
        
        (VisualRoot as Window)?.Close();
        
        OnLeaveChat?.Invoke();
    }

    private void OnEmployeeSelectClick(object? sender, TappedEventArgs e)
    {
        var employee = EmployeeList.SelectedItem;
    }
}