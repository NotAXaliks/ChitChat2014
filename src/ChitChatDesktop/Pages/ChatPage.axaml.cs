using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ChitChatDesktop.Dtos;

namespace ChitChatDesktop.Pages;

public partial class ChatPage : UserControl
{
    public record ChatMessage(string Time, string Sender, string Text);
    
    public ChatPage(ChatroomDto chat, EmployeeDto[] employees)
    {
        InitializeComponent();
     
        // Вставляем участников чата
        EmployeeList.ItemsSource = employees.Select(employee => employee.Name).ToList();
        
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
            new ChatMessage("10:50", "Saskia M", "fix it yourself!!"),
            new ChatMessage("10:51", "Mario M", "New message here")
        };
        
        foreach (var message in chatMessages)
        {
            MessagesPanel.Children.Add(new TextBlock
            {
                Text = $"[{message.Time}] {message.Sender}: {message.Text}",
                Margin = new Avalonia.Thickness(5, 2)
            });
        }
    }
    
    private void OnSendClick(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("Clicked on Send button");
    }
    
}