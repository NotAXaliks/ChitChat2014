using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ChitChatDesktop.Dtos;
using ChitChatDesktop.Services;
using MsBox.Avalonia;

namespace ChitChatDesktop.Pages;

public partial class EmployeeFinderPage : UserControl
{
    public record DepartmentInList(int Id, string Name, bool IsChecked);

    private readonly ChatPage? _chatPage;

    private System.Timers.Timer? _debounceTimer;

    public EmployeeFinderPage(ChatPage? chatPage)
    {
        InitializeComponent();

        _chatPage = chatPage;

        Refresh();
    }

    private async void Refresh()
    {
        var departmentsResponse = await EmployeeApi.GetDepartments();
        var departments = departmentsResponse.Data;
        if (departments == null)
        {
            await MessageBoxManager.GetMessageBoxStandard("Error", "An error occurred while fetching the departments.")
                .ShowAsync();
            return;
        }

        DepartmentsList.ItemsSource = departments.Select(d => new DepartmentInList(d.Id, d.Name, true));

        UpdateEmployees();
    }

    private async void UpdateEmployees()
    {
        var selectedDepartmentsIds = DepartmentsList.Items
            .OfType<DepartmentInList>()
            .Where(d => d.IsChecked)
            .Select(d => d.Id)
            .ToArray();

        var searchEmployeesResponse =
            await EmployeeApi.SearchEmployees(selectedDepartmentsIds, SearchBox.Text);
        var employees = searchEmployeesResponse.Data;
        if (employees == null)
        {
            await MessageBoxManager.GetMessageBoxStandard("Error", "An error occurred while fetching the employees.")
                .ShowAsync();
            return;
        }

        EmployeeList.ItemsSource = employees;
    }

    private void OnDepartmentCheck(object? sender, RoutedEventArgs e)
    {
        _debounceTimer?.Stop();
        UpdateEmployees();
    }

    private async void OnEmployeeSelectClick(object? sender, TappedEventArgs e)
    {
        var selectedItemEmployee = EmployeeList.SelectedItem;
        if (selectedItemEmployee is not EmployeeDto employee) return;

        // Если мы вызвали это окно из чата, значит просто добавляем этого работника к нам в чат
        if (_chatPage != null)
        {
            var employeeAddResponse = await ChatApi.AddEmployee(_chatPage.ChatId, employee.Id);
            if (!employeeAddResponse.IsSuccess)
            {
                await MessageBoxManager.GetMessageBoxStandard("Error",
                        employeeAddResponse.Error ?? "An error occurred while adding employee.")
                    .ShowAsync();
                return;
            }

            _chatPage.Refresh();

            (VisualRoot as Window)?.Close();

            return;
        }

        var createChatResponse = await ChatApi.CreateChat(new[] { employee.Id }, "New chat");
        if (!createChatResponse.IsSuccess || createChatResponse.Data == null)
        {
            await MessageBoxManager.GetMessageBoxStandard("Error",
                    createChatResponse.Error ?? "An error occurred while adding employee.")
                .ShowAsync();
            return;
        }

        var chatWindow = new ChatWindow(createChatResponse.Data.Chatroom.Id);
        chatWindow.Show();

        (VisualRoot as Window)?.Close();
    }

    private void OnSearchInput(object? sender, TextChangedEventArgs e)
    {
        _debounceTimer?.Stop();
        _debounceTimer = new System.Timers.Timer(300) { AutoReset = false };
        _debounceTimer.Elapsed += (_, _) => { Dispatcher.UIThread.Post(UpdateEmployees); };
        _debounceTimer.Start();
    }
}