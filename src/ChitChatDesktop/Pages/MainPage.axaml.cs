using System.Collections.Generic;
using Avalonia.Controls;

namespace ChitChatDesktop.Pages;

public partial class MainPage : UserControl
{
    public List<Chatroom> ChatList { get; }

    public MainPage()
    {
        InitializeComponent();

        HelloText.Text = "Hello, World!";

        ChatList = new List<Chatroom>
        {
            new Chatroom { Topic = "The roof is on firsfdgsdfgsdfgsdfgfsde", LastMessage = "24.11. 15:10" },
            new Chatroom { Topic = "Sebastian", LastMessage = "15.11. 08:30" },
            new Chatroom { Topic = "Lisa", LastMessage = "1.12. 10:30" },
            new Chatroom { Topic = "IT Helpdesk", LastMessage = "1.8. 17:23" }
        };

        DataContext = this;
    }

    public class Chatroom
    {
        public string Topic { get; set; }
        public string LastMessage { get; set; }
    }
}