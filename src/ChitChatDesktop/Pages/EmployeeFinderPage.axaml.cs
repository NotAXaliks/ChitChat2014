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
    
    private System.Timers.Timer? _debounceTimer;

    public EmployeeFinderPage()
    {
        InitializeComponent();

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

    private void OnSearchInput(object? sender, TextChangedEventArgs e)
    {
        Console.WriteLine("Input");
        _debounceTimer?.Stop();
        _debounceTimer = new System.Timers.Timer(300) { AutoReset = false };
        _debounceTimer.Elapsed += (_, _) =>
        {
            Dispatcher.UIThread.Post(UpdateEmployees);
        };
        _debounceTimer.Start();
    }
}
